using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using ICSharpCode.SharpZipLib.Zip;
using AODL.Document;
using AODL.Document.TextDocuments;
using AODL.Document.SpreadsheetDocuments;

namespace AODLTest
{
	[TestFixture]
	public class OpenDocumentPackageTests
	{
		private const string OfficeNs = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
		private const string ManifestNs = "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";
		// TextDocument finalizers delete shared import/export directories.
		private static readonly ArrayList documents = new ArrayList();
		private string directory;

		[SetUp]
		public void SetUp()
		{
			directory = Path.Combine(Path.GetTempPath(), "aodl-test-" + Guid.NewGuid().ToString());
			Directory.CreateDirectory(directory);
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(directory, true);
		}

		[Test]
		public void MissingOptionalPartsDoNotReusePreviousImports()
		{
			CheckMissingParts(false);
			CheckMissingParts(true);
		}

		[Test]
		public void NewPackagesHaveStoredFirstMimetypeAndMatchingVersions()
		{
			CheckPackage(false);
			CheckPackage(true);
		}

		[Test]
		public void ImportRejectsEntriesOutsideExtractionDirectory()
		{
			foreach (string name in new string[] { "../escape.txt", "..\\escape.txt", "/escape.txt" })
			{
				string file = Path.Combine(directory, Guid.NewGuid().ToString() + ".odt");
				using (ZipOutputStream output = new ZipOutputStream(File.Create(file)))
				{
					output.PutNextEntry(new ZipEntry(name));
					output.WriteByte(1);
					output.CloseEntry();
				}
				try
				{
					NewDocument(false).Load(file);
					Assert.Fail("Expected invalid entry to be rejected: " + name);
				}
				catch (InvalidDataException)
				{
				}
			}
		}

		[Test]
		public void ExistingVersionsArePreserved()
		{
			string[] versions = { "1.0", "1.1", "1.2", "1.3", "1.4" };
			foreach (bool spreadsheet in new bool[] { false, true })
			{
				foreach (string version in versions)
				{
					IDocument document = NewDocument(spreadsheet);
					document.XmlDoc.DocumentElement.SetAttribute("version", OfficeNs, version);
					string file = Path.Combine(directory, "version-" + version + (spreadsheet ? ".ods" : ".odt"));
					document.SaveTo(file);
					Assert.AreEqual(version, ReadXml(file, "content.xml").DocumentElement.GetAttribute("version", OfficeNs));
					if (version != "1.0" && version != "1.1")
					{
						Assert.AreEqual(version, ReadXml(file, "styles.xml").DocumentElement.GetAttribute("version", OfficeNs));
						Assert.AreEqual(version, ReadXml(file, "meta.xml").DocumentElement.GetAttribute("version", OfficeNs));
						Assert.AreEqual(version, ReadXml(file, "settings.xml").DocumentElement.GetAttribute("version", OfficeNs));
					}
					else
						Assert.AreEqual("1.0", ReadXml(file, "styles.xml").DocumentElement.GetAttribute("version", OfficeNs));
				}
			}
		}

		private void CheckMissingParts(bool spreadsheet)
		{
			string extension = spreadsheet ? ".ods" : ".odt";
			string complete = Path.Combine(directory, "complete" + extension);
			IDocument source = NewDocument(spreadsheet);
			source.DocumentMetadata.Meta.DocumentElement.SetAttribute("test-marker", "previous");
			GetStyles(source).DocumentElement.SetAttribute("test-marker", "previous");
			GetSettings(source).DocumentElement.SetAttribute("test-marker", "previous");
			source.SaveTo(complete);
			string[] optional = { "meta.xml", "settings.xml", "styles.xml" };
			for (int mask = 1; mask < 8; mask++)
			{
				string minimal = Path.Combine(directory, "minimal-" + mask + extension);
				string saved = Path.Combine(directory, "saved-" + mask + extension);
				RemoveOptionalParts(complete, minimal, optional, mask);
				IDocument first = NewDocument(spreadsheet);
				first.Load(complete);
				IDocument second = NewDocument(spreadsheet);
				second.Load(minimal);
				XmlDocument[] parts = { second.DocumentMetadata.Meta, GetSettings(second), GetStyles(second) };
				for (int i = 0; i < optional.Length; i++)
					Assert.AreEqual((mask & (1 << i)) != 0 ? "" : "previous",
						parts[i].DocumentElement.GetAttribute("test-marker"), optional[i]);
				second.SaveTo(saved);

				XmlDocument manifest = ReadXml(saved, "META-INF/manifest.xml");
				XmlNamespaceManager ns = new XmlNamespaceManager(manifest.NameTable);
				ns.AddNamespace("manifest", ManifestNs);
				foreach (string part in new string[] { "content.xml", "styles.xml", "meta.xml", "settings.xml" })
				{
					Assert.IsNotNull(ReadXml(saved, part));
					XmlNodeList entries = manifest.SelectNodes("/manifest:manifest/manifest:file-entry[@manifest:full-path='" + part + "']", ns);
					Assert.AreEqual(1, entries.Count, part);
					Assert.AreEqual("text/xml", ((XmlElement)entries[0]).GetAttribute("media-type", ManifestNs));
				}
			}
		}

		private void CheckPackage(bool spreadsheet)
		{
			string file = Path.Combine(directory, spreadsheet ? "new.ods" : "new.odt");
			string mime = "application/vnd.oasis.opendocument." + (spreadsheet ? "spreadsheet" : "text");
			NewDocument(spreadsheet).SaveTo(file);
			using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
			{
				Assert.AreEqual(0x04034b50, reader.ReadInt32());
				reader.ReadBytes(4);
				Assert.AreEqual(0, reader.ReadUInt16(), "mimetype compression method");
				reader.ReadBytes(16);
				int nameLength = reader.ReadUInt16();
				int extraLength = reader.ReadUInt16();
				Assert.AreEqual("mimetype", Encoding.ASCII.GetString(reader.ReadBytes(nameLength)));
				Assert.AreEqual(0, extraLength);
				Assert.AreEqual(mime, Encoding.ASCII.GetString(reader.ReadBytes(mime.Length)));
			}
			foreach (string part in new string[] { "content.xml", "styles.xml", "meta.xml", "settings.xml" })
				Assert.AreEqual("1.4", ReadXml(file, part).DocumentElement.GetAttribute("version", OfficeNs), part);
			XmlDocument manifest = ReadXml(file, "META-INF/manifest.xml");
			Assert.AreEqual("1.4", manifest.DocumentElement.GetAttribute("version", ManifestNs));
			XmlNamespaceManager ns = new XmlNamespaceManager(manifest.NameTable);
			ns.AddNamespace("manifest", ManifestNs);
			XmlElement root = (XmlElement)manifest.SelectSingleNode("/manifest:manifest/manifest:file-entry[@manifest:full-path='/']", ns);
			Assert.AreEqual("1.4", root.GetAttribute("version", ManifestNs));
		}

		private static IDocument NewDocument(bool spreadsheet)
		{
			if (spreadsheet)
			{
				SpreadsheetDocument document = new SpreadsheetDocument();
				document.New();
				documents.Add(document);
				return document;
			}
			TextDocument text = new TextDocument();
			text.New();
			documents.Add(text);
			return text;
		}

		private static XmlDocument GetStyles(IDocument document)
		{
			return document is TextDocument ? ((TextDocument)document).DocumentStyles.Styles :
				((SpreadsheetDocument)document).DocumentStyles.Styles;
		}

		private static XmlDocument GetSettings(IDocument document)
		{
			return document is TextDocument ? ((TextDocument)document).DocumentSetting.Settings :
				((SpreadsheetDocument)document).DocumentSetting.Settings;
		}

		private static XmlDocument ReadXml(string file, string part)
		{
			using (ZipInputStream zip = new ZipInputStream(File.OpenRead(file)))
			{
				ZipEntry entry;
				while ((entry = zip.GetNextEntry()) != null)
				{
					if (entry.Name == part)
					{
						XmlDocument xml = new XmlDocument();
						xml.Load(zip);
						return xml;
					}
				}
			}
			Assert.Fail("Missing ZIP entry: " + part);
			return null;
		}

		private static void RemoveOptionalParts(string source, string destination, string[] optional, int mask)
		{
			using (ZipInputStream input = new ZipInputStream(File.OpenRead(source)))
			using (ZipOutputStream output = new ZipOutputStream(File.Create(destination)))
			{
				ZipEntry entry;
				while ((entry = input.GetNextEntry()) != null)
				{
					int index = Array.IndexOf(optional, entry.Name);
					if (index >= 0 && (mask & (1 << index)) != 0)
						continue;
					output.PutNextEntry(new ZipEntry(entry.Name));
					if (entry.Name == "META-INF/manifest.xml")
					{
						XmlDocument manifest = new XmlDocument();
						manifest.Load(input);
						XmlNamespaceManager ns = new XmlNamespaceManager(manifest.NameTable);
						ns.AddNamespace("manifest", ManifestNs);
						for (int i = 0; i < optional.Length; i++)
						{
							if ((mask & (1 << i)) == 0) continue;
							XmlNode node = manifest.SelectSingleNode("/manifest:manifest/manifest:file-entry[@manifest:full-path='" + optional[i] + "']", ns);
							node.ParentNode.RemoveChild(node);
						}
						manifest.Save(output);
					}
					else
					{
						byte[] buffer = new byte[8192];
						int count;
						while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
							output.Write(buffer, 0, count);
					}
					output.CloseEntry();
				}
			}
		}
	}
}
