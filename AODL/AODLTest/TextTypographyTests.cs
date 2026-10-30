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
	public class TextTypographyTests
	{
		private const string FoNs = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0";
		private static readonly ArrayList documents = new ArrayList();
		private string directory;

		[SetUp]
		public void SetUp()
		{
			directory = Path.Combine(Path.GetTempPath(), "aodl-text-" + Guid.NewGuid().ToString());
			Directory.CreateDirectory(directory);
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(directory, true);
		}

		[Test]
		public void TextAndParagraphStylesRoundTrip()
		{
			TextDocument document = NewDocument();
			FormatedText formatted = new FormatedText(document, "TestLocalText", "Ord för avstavning");
			TextStyle local = formatted.TextStyle;
			TextStyle common = new TextStyle(document, "TestCommonText");
			Paragraph content = new Paragraph(document, "TestParagraphText");
			ParagraphStyle paragraph = content.ParagraphStyle;
			SetProperties(local.TextProperties);
			SetProperties(common.TextProperties);
			SetProperties(paragraph.TextProperties);
			content.TextContent.Add(formatted);
			document.Content.Add(content);
			document.CommonStyles.Add(common);
			string first = Path.Combine(directory, "first.odt");
			string second = Path.Combine(directory, "second.odt");
			document.SaveTo(first);
			AssertXml(first, "content.xml", "TestLocalText");
			AssertXml(first, "content.xml", "TestParagraphText");
			AssertXml(first, "styles.xml", "TestCommonText");

			TextDocument loaded = NewDocument();
			loaded.Load(first);
			AssertProperties(((TextStyle)loaded.Styles.GetStyleByName("TestLocalText")).TextProperties);
			AssertProperties(((TextStyle)loaded.CommonStyles.GetStyleByName("TestCommonText")).TextProperties);
			AssertProperties(((ParagraphStyle)loaded.Styles.GetStyleByName("TestParagraphText")).TextProperties);
			Paragraph loadedContent = null;
			foreach (object item in loaded.Content)
				if (item is Paragraph && ((Paragraph)item).StyleName == "TestParagraphText")
					loadedContent = (Paragraph)item;
			Assert.IsNotNull(loadedContent);
			Assert.AreEqual("TestParagraphText", loadedContent.StyleName);
			Assert.IsInstanceOf(typeof(FormatedText), loadedContent.TextContent[0]);
			Assert.AreEqual("TestLocalText", ((FormatedText)loadedContent.TextContent[0]).StyleName);
			StringAssert.Contains("lang=\"sv-SE\"", loadedContent.GetHtml());
			StringAssert.Contains("lang=\"sv-SE\"", ((FormatedText)loadedContent.TextContent[0]).GetHtml());
			loaded.SaveTo(second);
			AssertXml(second, "content.xml", "TestLocalText");
			AssertXml(second, "content.xml", "TestParagraphText");
			AssertXml(second, "styles.xml", "TestCommonText");
		}

		[Test]
		public void AttributesCanBeUpdatedAndRemoved()
		{
			TextProperties properties = new TextStyle(NewDocument(), "Text").TextProperties;
			Assert.IsNull(properties.Language);
			Assert.IsNull(properties.Hyphenate);
			SetProperties(properties);
			properties.Language = "en";
			properties.Hyphenate = "false";
			Assert.AreEqual("en", properties.Language);
			Assert.AreEqual("false", properties.Hyphenate);
			properties.Language = null;
			properties.Country = null;
			properties.LetterSpacing = null;
			properties.Hyphenate = null;
			properties.HyphenationRemainCharCount = null;
			properties.HyphenationPushCharCount = null;
			foreach (string name in Names())
				Assert.IsNull(properties.Node.Attributes.GetNamedItem(name, FoNs), name);
		}

		[Test]
		public void HtmlExportsSupportedTypography()
		{
			FormatedText text = new FormatedText(NewDocument(), "Test", "text");
			SetProperties(text.TextStyle.TextProperties);
			AssertTypography(text.TextStyle.TextProperties.GetHtmlStyle(), "auto");
			AssertTypography(new HTMLStyleBuilder().GetTextStyleAsHtml(text.TextStyle.TextProperties), "auto");
			StringAssert.Contains("hyphenate-limit-chars: auto 3 2;", text.TextStyle.TextProperties.GetHtmlStyle());
			StringAssert.Contains("hyphenate-limit-chars: auto 3 2;",
				new HTMLStyleBuilder().GetTextStyleAsHtml(text.TextStyle.TextProperties));
			AssertTypography(text.GetHtml(), "auto");
			AssertTypography(new HTMLContentBuilder().GetFormatedTextAsHtml(text), "auto");
			StringAssert.Contains("hyphenate-limit-chars: auto 3 2;", text.GetHtml());
			StringAssert.Contains("hyphenate-limit-chars: auto 3 2;", new HTMLContentBuilder().GetFormatedTextAsHtml(text));
			StringAssert.Contains("lang=\"sv-SE\"", text.GetHtml());
			StringAssert.Contains("lang=\"sv-SE\"", new HTMLContentBuilder().GetFormatedTextAsHtml(text));
			foreach (string value in new string[] { "false", "0" })
			{
				text.TextStyle.TextProperties.Hyphenate = value;
				AssertTypography(text.TextStyle.TextProperties.GetHtmlStyle(), "manual");
			}
			text.TextStyle.TextProperties.Hyphenate = "1";
			AssertTypography(text.TextStyle.TextProperties.GetHtmlStyle(), "auto");
			text.TextStyle.TextProperties.Hyphenate = null;
			Assert.IsFalse(text.TextStyle.TextProperties.GetHtmlStyle().Contains("hyphens:"));
		}

		[Test]
		public void HtmlHyphenationLimitsRespectStyleAndContentInheritance()
		{
			TextDocument document = NewDocument();
			ParagraphStyle parent = new ParagraphStyle(document, "HyphenParent");
			parent.TextProperties.HyphenationRemainCharCount = "4";
			parent.TextProperties.HyphenationPushCharCount = "3";
			parent.TextProperties.Hyphenate = "true";
			document.CommonStyles.Add(parent);
			Paragraph paragraph = new Paragraph(document, "HyphenChild");
			paragraph.ParagraphStyle.ParentStyle = "HyphenParent";
			paragraph.ParagraphStyle.TextProperties.HyphenationPushCharCount = "5";
			FormatedText text = new FormatedText(document, "HyphenSpan", "text");
			text.TextStyle.TextProperties.HyphenationRemainCharCount = "6";
			paragraph.TextContent.Add(text);
			document.Content.Add(paragraph);
			AssertLimit(paragraph.GetHtml(), "auto 4 5", true);
			AssertLimit(new HTMLContentBuilder().GetParagraphAsHtml(paragraph), "auto 4 5", true);
			StringAssert.Contains("hyphens: auto;", paragraph.GetHtml());
			StringAssert.Contains("hyphens: auto;", new HTMLContentBuilder().GetParagraphAsHtml(paragraph));
			AssertLimit(text.GetHtml(), "auto 6 5", false);
			AssertLimit(new HTMLContentBuilder().GetFormatedTextAsHtml(text), "auto 6 5", false);

			text.TextStyle.TextProperties.HyphenationRemainCharCount = null;
			text.TextStyle.TextProperties.HyphenationPushCharCount = "7";
			AssertLimit(text.GetHtml(), "auto 4 7", false);
			text.TextStyle.TextProperties.HyphenationPushCharCount = null;
			Assert.IsFalse(text.GetHtml().Contains("hyphenate-limit-chars"));
		}

		[Test]
		public void HtmlHyphenationLimitsSupportPartialAndDefaultStyles()
		{
			TextDocument document = NewDocument();
			FormatedText text = new FormatedText(document, "PartialLimit", "text");
			text.TextStyle.TextProperties.HyphenationRemainCharCount = "3";
			AssertLimit(text.GetHtml(), "auto 3 auto", false);
			text.TextStyle.TextProperties.HyphenationRemainCharCount = null;
			text.TextStyle.TextProperties.HyphenationPushCharCount = "4";
			AssertLimit(text.GetHtml(), "auto auto 4", false);
			Paragraph paragraph = new Paragraph(document, ParentStyles.Standard, "text");
			AssertLimit(paragraph.GetHtml(), "auto 2 2", true);
			Header heading = new Header(document, Headings.Heading_20_1);
			AssertLimit(heading.GetHtml(), "auto 2 2", true);
			AssertLimit(new HTMLContentBuilder().GetHeadingAsHtml(heading), "auto 2 2", true);
		}

		[Test]
		public void HtmlRejectsUnrepresentableHyphenationLimits()
		{
			FormatedText text = new FormatedText(NewDocument(), "InvalidLimit", "text");
			foreach (string value in new string[] { "0", "-1", "3; color: red", "999999999999999999999" })
			{
				text.TextStyle.TextProperties.HyphenationRemainCharCount = value;
				try
				{
					text.GetHtml();
					Assert.Fail("Expected invalid limit to fail: " + value);
				}
				catch (FormatException)
				{
				}
			}
		}

		private static void AssertLimit(string html, string expected, bool paragraph)
		{
			int start = html.IndexOf(paragraph ? "<p " : "<span ");
			Assert.GreaterOrEqual(start, 0);
			int end = html.IndexOf('>', start);
			Assert.Greater(end, start);
			string tag = html.Substring(start, end - start);
			StringAssert.Contains("hyphenate-limit-chars: " + expected + ";", tag);
			Assert.AreEqual(tag.IndexOf("style=\""), tag.LastIndexOf("style=\""));
		}

		[Test]
		public void HtmlResolvesParagraphLanguagesAndNestedTextOverrides()
		{
			TextDocument document = NewDocument();
			ParagraphStyle parent = new ParagraphStyle(document, "LanguageParent");
			parent.TextProperties.Language = "sv";
			parent.TextProperties.Country = "SE";
			document.CommonStyles.Add(parent);
			Paragraph paragraph = new Paragraph(document, "LanguageChild");
			paragraph.ParagraphStyle.ParentStyle = "LanguageParent";
			FormatedText text = new FormatedText(document, "NestedLanguage", "text");
			text.TextStyle.TextProperties.Country = "FI";
			paragraph.TextContent.Add(text);
			document.Content.Add(paragraph);
			string html = paragraph.GetHtml();
			StringAssert.Contains("lang=\"sv-SE\"", html);
			StringAssert.Contains("lang=\"sv-FI\"", html);
			Assert.IsTrue(html.IndexOf(">\n") < html.IndexOf("<span "));
			StringAssert.Contains("lang=\"sv-SE\"", new HTMLContentBuilder().GetParagraphAsHtml(paragraph));
			StringAssert.Contains("lang=\"sv-FI\"", new HTMLContentBuilder().GetFormatedTextAsHtml(text));

			paragraph.ParagraphStyle.TextProperties.Language = "en";
			StringAssert.Contains("lang=\"en-SE\"", paragraph.GetHtml());
			StringAssert.Contains("lang=\"en-FI\"", text.GetHtml());
			text.TextStyle.TextProperties.Country = null;
			text.TextStyle.TextProperties.Language = "none";
			StringAssert.Contains("lang=\"\"", text.GetHtml());
			text.TextStyle.TextProperties.Language = null;
			Assert.IsFalse(text.GetHtml().Contains("lang="));
		}

		[Test]
		public void HtmlResolvesCommonTextStylesAndEscapesValues()
		{
			TextDocument document = NewDocument();
			TextStyle parent = new TextStyle(document, "TextParent");
			parent.TextProperties.Language = "fr";
			parent.TextProperties.Country = "CA";
			parent.TextProperties.HyphenationRemainCharCount = "4";
			parent.TextProperties.HyphenationPushCharCount = "3";
			document.CommonStyles.Add(parent);
			FormatedText text = new FormatedText(document, "TextChild", "text");
			((XmlElement)text.TextStyle.Node).SetAttribute("parent-style-name",
				"urn:oasis:names:tc:opendocument:xmlns:style:1.0", "TextParent");
			StringAssert.Contains("lang=\"fr-CA\"", text.GetHtml());
			AssertLimit(text.GetHtml(), "auto 4 3", false);
			text.TextStyle.TextProperties.Language = "en\" onmouseover=\"bad";
			StringAssert.Contains("lang=\"en&quot; onmouseover=&quot;bad-CA\"", text.GetHtml());

			XmlNode node = document.CreateNode("span", "text");
			XmlAttribute styleName = document.CreateAttribute("style-name", "text");
			styleName.Value = "TextParent";
			node.Attributes.Append(styleName);
			node.InnerText = "text";
			FormatedText commonText = new FormatedText(document, node);
			StringAssert.Contains("lang=\"fr-CA\"", commonText.GetHtml());
			StringAssert.Contains("lang=\"fr-CA\"", new HTMLContentBuilder().GetFormatedTextAsHtml(commonText));
			AssertLimit(commonText.GetHtml(), "auto 4 3", false);
		}

		[Test]
		public void HtmlRejectsCircularStyleInheritance()
		{
			TextDocument document = NewDocument();
			TextStyle style = new TextStyle(document, "Cycle");
			((XmlElement)style.Node).SetAttribute("parent-style-name",
				"urn:oasis:names:tc:opendocument:xmlns:style:1.0", "Cycle");
			document.CommonStyles.Add(style);
			XmlNode node = document.CreateNode("span", "text");
			XmlAttribute name = document.CreateAttribute("style-name", "text");
			name.Value = "Cycle";
			node.Attributes.Append(name);
			FormatedText text = new FormatedText(document, node);
			try
			{
				text.GetHtml();
				Assert.Fail("Expected circular inheritance to fail.");
			}
			catch (InvalidOperationException)
			{
			}
		}

		[Test]
		public void HtmlExportsHeadingAndDefaultLanguage()
		{
			TextDocument document = NewDocument();
			Header heading = new Header(document, Headings.Heading_20_1);
			heading.StyleName = "TestHeadingLanguage";
			ParagraphStyle headingStyle = new ParagraphStyle(document, heading.StyleName);
			headingStyle.TextProperties.Language = "sv";
			headingStyle.TextProperties.Country = "SE";
			document.CommonStyles.Add(headingStyle);
			StringAssert.Contains("lang=\"sv-SE\"", heading.GetHtml());
			StringAssert.Contains("lang=\"sv-SE\"", new HTMLContentBuilder().GetHeadingAsHtml(heading));
			Paragraph paragraph = new Paragraph(document, ParentStyles.Standard, "text");
			StringAssert.Contains("lang=\"de-DE\"", paragraph.GetHtml());
			StringAssert.Contains("lang=\"de-DE\"", new HTMLContentBuilder().GetParagraphAsHtml(paragraph));
		}

		[Test]
		public void HtmlPreservesParentAndChildTypography()
		{
			TextDocument document = NewDocument();
			ParagraphStyle parent = new ParagraphStyle(document, "TypographyParent");
			SetProperties(parent.TextProperties);
			ParagraphStyle child = new ParagraphStyle(document, "TypographyChild");
			child.ParentStyle = "TypographyParent";
			child.TextProperties.LetterSpacing = "1pt";
			child.TextProperties.Hyphenate = "false";
			XmlNode styles = document.DocumentStyles.Styles.SelectSingleNode(
				"/office:document-styles/office:styles", document.NamespaceManager);
			styles.AppendChild(document.DocumentStyles.Styles.ImportNode(parent.Node, true));
			styles.AppendChild(document.DocumentStyles.Styles.ImportNode(child.Node, true));
			Paragraph paragraph = new Paragraph(document);
			paragraph.StyleName = "TypographyChild";
			string html = paragraph.GetHtml();
			AssertTypography(html, "auto");
			StringAssert.Contains("letter-spacing: 1pt;", html);
			Assert.IsTrue(html.IndexOf("hyphens: auto;") < html.IndexOf("hyphens: manual;"));
		}

		private static TextDocument NewDocument()
		{
			TextDocument document = new TextDocument();
			document.New();
			documents.Add(document);
			return document;
		}

		private static void SetProperties(TextProperties properties)
		{
			properties.Language = "sv";
			properties.Country = "SE";
			properties.LetterSpacing = "0.5pt";
			properties.Hyphenate = "true";
			properties.HyphenationRemainCharCount = "3";
			properties.HyphenationPushCharCount = "2";
		}

		private static void AssertProperties(TextProperties properties)
		{
			Assert.AreEqual("sv", properties.Language);
			Assert.AreEqual("SE", properties.Country);
			Assert.AreEqual("0.5pt", properties.LetterSpacing);
			Assert.AreEqual("true", properties.Hyphenate);
			Assert.AreEqual("3", properties.HyphenationRemainCharCount);
			Assert.AreEqual("2", properties.HyphenationPushCharCount);
		}

		private static void AssertTypography(string html, string hyphens)
		{
			StringAssert.Contains("letter-spacing: 0.5pt;", html);
			StringAssert.Contains("hyphens: " + hyphens + ";", html);
		}

		private static string[] Names()
		{
			return new string[] { "language", "country", "letter-spacing", "hyphenate",
				"hyphenation-remain-char-count", "hyphenation-push-char-count" };
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
					ns.AddNamespace("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0");
					XmlElement element = (XmlElement)xml.SelectSingleNode(
						"//style:style[@style:name='" + name + "']/style:text-properties", ns);
					Assert.IsNotNull(element, name);
					string[] names = Names();
					string[] values = { "sv", "SE", "0.5pt", "true", "3", "2" };
					for (int i = 0; i < names.Length; i++)
						Assert.AreEqual(values[i], element.GetAttribute(names[i], FoNs), names[i]);
					return;
				}
			}
			Assert.Fail("Missing ZIP entry: " + part);
		}
	}
}
