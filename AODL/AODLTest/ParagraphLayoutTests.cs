using System;
using System.Collections;
using System.IO;
using System.Xml;
using NUnit.Framework;
using ICSharpCode.SharpZipLib.Zip;
using AODL.Document.TextDocuments;
using AODL.Document.Styles;
using AODL.Document.Styles.Properties;
using AODL.Document.Export.Html;
using AODL.Document.Content.Text;

namespace AODLTest
{
	[TestFixture]
	public class ParagraphLayoutTests
	{
		private const string FoNs = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0";
		private const string StyleNs = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";
		private static readonly ArrayList documents = new ArrayList();
		private string directory;

		[SetUp]
		public void SetUp()
		{
			directory = Path.Combine(Path.GetTempPath(), "aodl-layout-" + Guid.NewGuid().ToString());
			Directory.CreateDirectory(directory);
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(directory, true);
		}

		[Test]
		public void PropertiesRoundTripInAutomaticAndCommonStyles()
		{
			TextDocument document = NewDocument();
			ParagraphStyle local = new ParagraphStyle(document, "TestLocalLayout");
			ParagraphStyle common = new ParagraphStyle(document, "TestCommonLayout");
			SetProperties(local.ParagraphProperties);
			SetProperties(common.ParagraphProperties);
			document.Styles.Add(local);
			document.CommonStyles.Add(common);
			string first = Path.Combine(directory, "first.odt");
			string second = Path.Combine(directory, "second.odt");
			document.SaveTo(first);
			AssertXml(first, "content.xml", "TestLocalLayout");
			AssertXml(first, "styles.xml", "TestCommonLayout");

			TextDocument loaded = NewDocument();
			loaded.Load(first);
			AssertProperties(((ParagraphStyle)loaded.Styles.GetStyleByName("TestLocalLayout")).ParagraphProperties);
			AssertProperties(((ParagraphStyle)loaded.CommonStyles.GetStyleByName("TestCommonLayout")).ParagraphProperties);
			loaded.SaveTo(second);
			AssertXml(second, "content.xml", "TestLocalLayout");
			AssertXml(second, "styles.xml", "TestCommonLayout");
		}

		[Test]
		public void PropertiesCanBeUpdatedAndCleared()
		{
			ParagraphProperties properties = new ParagraphStyle(NewDocument(), "Test").ParagraphProperties;
			Assert.IsNull(properties.MarginRight);
			Assert.IsNull(properties.TextIndent);
			Assert.IsNull(properties.KeepTogether);
			SetProperties(properties);
			properties.MarginRight = "4cm";
			properties.KeepTogether = "auto";
			Assert.AreEqual("4cm", properties.MarginRight);
			Assert.AreEqual("auto", properties.KeepTogether);
			properties.MarginRight = null;
			properties.MarginTop = null;
			properties.MarginBottom = null;
			properties.TextIndent = null;
			properties.KeepWithNext = null;
			properties.KeepTogether = null;
			properties.Widows = null;
			properties.Orphans = null;
			foreach (string name in new string[] { "margin-right", "margin-top", "margin-bottom", "text-indent",
				"keep-with-next", "keep-together", "widows", "orphans" })
				Assert.IsNull(properties.Node.Attributes.GetNamedItem(name, FoNs), name);
		}

		[Test]
		public void HtmlUsesDistinctMarginsAndIndent()
		{
			TextDocument document = NewDocument();
			Paragraph paragraph = new Paragraph(document, "Test");
			ParagraphStyle style = paragraph.ParagraphStyle;
			SetProperties(style.ParagraphProperties);
			AssertHtml(style.ParagraphProperties.GetHtmlStyle());
			AssertHtml(new HTMLStyleBuilder().GetParagraphStyleAsHtml(style));
			AssertHtml(paragraph.GetHtml());
			AssertHtml(new HTMLContentBuilder().GetParagraphAsHtml(paragraph));
			style.ParagraphProperties.KeepWithNext = "auto";
			style.ParagraphProperties.KeepTogether = "auto";
			string html = style.ParagraphProperties.GetHtmlStyle();
			StringAssert.Contains("break-after: auto;", html);
			StringAssert.Contains("break-inside: auto;", html);
		}

		[Test]
		public void HtmlIncludesParentAndChildGlobalLayout()
		{
			TextDocument document = NewDocument();
			ParagraphStyle parent = new ParagraphStyle(document, "LayoutParent");
			SetProperties(parent.ParagraphProperties);
			ParagraphStyle child = new ParagraphStyle(document, "LayoutChild");
			child.ParentStyle = "LayoutParent";
			child.ParagraphProperties.TextIndent = "2.5cm";
			XmlNode styles = document.DocumentStyles.Styles.SelectSingleNode(
				"/office:document-styles/office:styles", document.NamespaceManager);
			styles.AppendChild(document.DocumentStyles.Styles.ImportNode(parent.Node, true));
			styles.AppendChild(document.DocumentStyles.Styles.ImportNode(child.Node, true));
			Paragraph paragraph = new Paragraph(document);
			paragraph.StyleName = "LayoutChild";
			string html = paragraph.GetHtml();
			AssertHtml(html);
			Assert.IsTrue(html.IndexOf("text-indent: -0.5cm;") < html.IndexOf("text-indent: 2.5cm;"));
		}

		private static void AssertHtml(string html)
		{
			foreach (string css in new string[] { "margin-left: 1cm;", "margin-right: 2cm;",
				"margin-top: 3pt;", "margin-bottom: 4pt;", "text-indent: -0.5cm;",
				"break-after: avoid-page;", "break-inside: avoid-page;", "widows: 3;", "orphans: 2;" })
				StringAssert.Contains(css, html);
			Assert.IsFalse(html.Contains("text-indent: 1cm;"));
		}

		private static TextDocument NewDocument()
		{
			TextDocument document = new TextDocument();
			document.New();
			documents.Add(document);
			return document;
		}

		private static void SetProperties(ParagraphProperties properties)
		{
			properties.MarginLeft = "1cm";
			properties.MarginRight = "2cm";
			properties.MarginTop = "3pt";
			properties.MarginBottom = "4pt";
			properties.TextIndent = "-0.5cm";
			properties.KeepWithNext = "always";
			properties.KeepTogether = "always";
			properties.Widows = "3";
			properties.Orphans = "2";
		}

		private static void AssertProperties(ParagraphProperties properties)
		{
			Assert.AreEqual("1cm", properties.MarginLeft);
			Assert.AreEqual("2cm", properties.MarginRight);
			Assert.AreEqual("3pt", properties.MarginTop);
			Assert.AreEqual("4pt", properties.MarginBottom);
			Assert.AreEqual("-0.5cm", properties.TextIndent);
			Assert.AreEqual("always", properties.KeepWithNext);
			Assert.AreEqual("always", properties.KeepTogether);
			Assert.AreEqual("3", properties.Widows);
			Assert.AreEqual("2", properties.Orphans);
		}

		private static void AssertXml(string file, string part, string name)
		{
			using (ZipInputStream zip = new ZipInputStream(File.OpenRead(file)))
			{
				ZipEntry entry;
				while ((entry = zip.GetNextEntry()) != null)
				{
					if (entry.Name != part) continue;
					XmlDocument xml = new XmlDocument();
					xml.Load(zip);
					XmlNamespaceManager ns = new XmlNamespaceManager(xml.NameTable);
					ns.AddNamespace("style", StyleNs);
					XmlElement element = (XmlElement)xml.SelectSingleNode(
						"//style:style[@style:name='" + name + "']/style:paragraph-properties", ns);
					Assert.IsNotNull(element, name);
					string[] names = { "margin-left", "margin-right", "margin-top", "margin-bottom",
						"text-indent", "keep-with-next", "keep-together", "widows", "orphans" };
					string[] values = { "1cm", "2cm", "3pt", "4pt", "-0.5cm", "always", "always", "3", "2" };
					for (int i = 0; i < names.Length; i++)
						Assert.AreEqual(values[i], element.GetAttribute(names[i], FoNs), names[i]);
					return;
				}
			}
			Assert.Fail("Missing ZIP entry: " + part);
		}
	}
}
