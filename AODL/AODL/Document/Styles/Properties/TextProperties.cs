/*
 * $Id: TextProperties.cs,v 1.1 2006/01/29 11:28:23 larsbm Exp $
 */

/*
 * License: 
 * GNU Lesser General Public License. You should recieve a
 * copy of this within the library. If not you will find
 * a whole copy at http://www.gnu.org/licenses/lgpl.html .
 * 
 * Author:
 * Copyright 2006, Lars Behrmann, lb@OpenDocument4all.com
 * 
 * Last changes:
 * 
 */

using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using AODL.Document;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.Styles.Properties
{
	/// <summary>
	/// Represent access to the possible attributes of of an text-propertie element.
	/// </summary>
	public class TextProperties : IProperty, IHtmlStyle
	{
		/// <summary>
		/// Set font-weight bold object.Bold = "bold";
		/// </summary>
		public string Bold
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@fo:font-weight",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@fo:font-weight",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("font-weight", value, "fo");
				this._node.SelectSingleNode("@fo:font-weight",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text position, could be sub- ("sub 58%") or superscript ("super 58%")
		/// See TextPropertieHelper for possible settings
		/// </summary>
		public string Position
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-position",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-position",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-position", value, "style");
				this._node.SelectSingleNode("@style:text-position",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text shadow e.g. "1pt 1pt"
		/// See TextPropertieHelper for possible settings
		/// </summary>
		public string Shadow
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-shadow",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-shadow",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-shadow", value, "style");
				this._node.SelectSingleNode("@style:text-shadow",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text outline e.g. "true"
		/// </summary>
		public string Outline
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-outline",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-outline",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-outline", value, "style");
				this._node.SelectSingleNode("@style:text-outline",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text to line through
		/// See LineStyles for possible settings
		/// </summary>
		public string TextLineThrough
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-line-through-style",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-line-through-style",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-line-through-style", value, "style");
				this._node.SelectSingleNode("@style:text-line-through-style",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set the font color
		/// Use Colors.GetColor(Color color) to set 
		/// on of the available .net colors
		/// </summary>
		public string FontColor
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@fo:color",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@fo:color",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("color", value, "fo");
				this._node.SelectSingleNode("@fo:color",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set the background color
		/// Use Colors.GetColor(Color color) to set 
		/// on of the available .net colors
		/// </summary>
		public string BackgroundColor
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@fo:background-color",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@fo:background-color",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("background-color", value, "fo");
				this._node.SelectSingleNode("@fo:background-color",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set font-style italic object.Italic = "italic";
		/// </summary>
		public string Italic
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@fo:font-style",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@fo:font-style",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("font-style", value, "fo");
				this._node.SelectSingleNode("@fo:font-style",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text-underline-style Underline object.Underline = "dotted";
		/// </summary>
		public string Underline
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-style",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-style",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-underline-style", value, "style");
				this._node.SelectSingleNode("@style:text-underline-style",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text-underline-style Underline object.Underline = "dotted";
		/// </summary>
		public string UnderlineWidth
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-width",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-width",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-underline-width", value, "style");
				this._node.SelectSingleNode("@style:text-underline-width",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set text-underline-color - object.UnderlineColor = "font-color";
		/// </summary>
		public string UnderlineColor
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-color",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@style:text-underline-color",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("text-underline-color", value, "style");
				this._node.SelectSingleNode("@style:text-underline-color",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set font-name you will find all available fonts in class FontFamilies
		/// </summary>
		public string FontName
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@style:font-name",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				this.Style.Document.FontList.Add(value);
				XmlNode xn = this._node.SelectSingleNode("@style:font-name",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("font-name", value, "style");
				this._node.SelectSingleNode("@style:font-name",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>
		/// Set font-name - object.FontSize = "10pt";
		/// </summary>
		public string FontSize
		{
			get 
			{ 
				XmlNode xn = this._node.SelectSingleNode("@fo:font-size",
					this.Style.Document.NamespaceManager);
				if(xn != null)
					return xn.InnerText;
				return null;
			}
			set
			{
				XmlNode xn = this._node.SelectSingleNode("@fo:font-size",
					this.Style.Document.NamespaceManager);
				if(xn == null)
					this.CreateAttribute("font-size", value, "fo");
				this._node.SelectSingleNode("@fo:font-size",
					this.Style.Document.NamespaceManager).InnerText = value;
			}
		}

		/// <summary>Gets or sets the western text language, for example "sv" or "none". Set to null to inherit.</summary>
		public string Language
		{
			get { return this.GetFoAttribute("language"); }
			set { this.SetFoAttribute("language", value); }
		}

		/// <summary>Gets or sets the western text country, for example "SE" or "none". Set to null to inherit.</summary>
		public string Country
		{
			get { return this.GetFoAttribute("country"); }
			set { this.SetFoAttribute("country", value); }
		}

		/// <summary>Gets or sets the letter spacing, for example "0.5pt" or "normal". Set to null to inherit.</summary>
		public string LetterSpacing
		{
			get { return this.GetFoAttribute("letter-spacing"); }
			set { this.SetFoAttribute("letter-spacing", value); }
		}

		/// <summary>Gets or sets automatic hyphenation as an XML boolean. Set to null to inherit.</summary>
		public string Hyphenate
		{
			get { return this.GetFoAttribute("hyphenate"); }
			set { this.SetFoAttribute("hyphenate", value); }
		}

		/// <summary>Gets or sets the minimum characters before a hyphen. Set to null to inherit.</summary>
		public string HyphenationRemainCharCount
		{
			get { return this.GetFoAttribute("hyphenation-remain-char-count"); }
			set { this.SetFoAttribute("hyphenation-remain-char-count", value); }
		}

		/// <summary>Gets or sets the minimum characters after a hyphen. Set to null to inherit.</summary>
		public string HyphenationPushCharCount
		{
			get { return this.GetFoAttribute("hyphenation-push-char-count"); }
			set { this.SetFoAttribute("hyphenation-push-char-count", value); }
		}

		/// <summary>
		/// Create a new TextProperties object that belongs to the given TextStyle object.
		/// </summary>
		/// <param name="style">The TextStyle object.</param>
		public TextProperties(IStyle style)
		{
			this.Style				= style;
			this.NewXmlNode();
		}

		/// <summary>
		/// Use to set all possible underline styles. You can use this method
		/// instead to set all necessary properties by hand.
		/// </summary>
		/// <param name="style">The style. Sess enum LineStyles.</param>
		/// <param name="width">The width. Sess enum LineWidths.</param>
		/// <param name="color">The color. "font-color" or as hex #[0-9a-fA-F]{6}</param>
		public void SetUnderlineStyles(string style, string width, string color)
		{
			this.Underline		= style;
			this.UnderlineWidth	= width;
			this.UnderlineColor	= color;
		}

		/// <summary>
		/// Create the XmlNode which represent the propertie element.
		/// </summary>
		private void NewXmlNode()
		{
			this.Node		= this.Style.Document.CreateNode("text-properties", "style");
		}

		/// <summary>
		/// Create a XmlAttribute for propertie XmlNode.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <param name="text">The attribute value.</param>
		/// <param name="prefix">The namespace prefix.</param>
		private void CreateAttribute(string name, string text, string prefix)
		{
			XmlAttribute xa = this.Style.Document.CreateAttribute(name, prefix);
			xa.Value		= text;
			this.Node.Attributes.Append(xa);
		}

		private string GetFoAttribute(string name)
		{
			XmlNode attribute = this.Node.SelectSingleNode("@fo:" + name, this.Style.Document.NamespaceManager);
			return attribute == null ? null : attribute.Value;
		}

		private void SetFoAttribute(string name, string value)
		{
			XmlAttribute attribute = this.Node.SelectSingleNode("@fo:" + name,
				this.Style.Document.NamespaceManager) as XmlAttribute;
			if (value == null)
			{
				if (attribute != null)
					this.Node.Attributes.Remove(attribute);
			}
			else if (attribute == null)
				this.CreateAttribute(name, value, "fo");
			else
				attribute.Value = value;
		}

		internal static string GetTypographyHtml(XmlNode node, XmlNamespaceManager namespaces)
		{
			XmlNode spacing = node.SelectSingleNode("@fo:letter-spacing", namespaces);
			XmlNode hyphenate = node.SelectSingleNode("@fo:hyphenate", namespaces);
			return FormatTypographyHtml(spacing == null ? null : spacing.Value,
				hyphenate == null ? null : hyphenate.Value);
		}

		internal static string GetInheritedTypographyHtml(IDocument document, IStyle style, string styleName,
			string family, bool includeDefault, XmlNode contentNode)
		{
			string spacing;
			string hyphenate;
			ResolveHtmlValues(document, style, styleName, family, includeDefault, contentNode,
				"letter-spacing", "hyphenate", out spacing, out hyphenate);
			return FormatTypographyHtml(spacing, hyphenate);
		}

		private static string FormatTypographyHtml(string spacing, string hyphenate)
		{
			string html = "";
			if (spacing != null)
				html += "letter-spacing: " + spacing + "; ";
			if (hyphenate != null)
			{
				if (hyphenate == "true" || hyphenate == "1")
					html += "hyphens: auto; ";
				else if (hyphenate == "false" || hyphenate == "0")
					html += "hyphens: manual; ";
			}
			return html;
		}

		internal static string GetHtmlLanguage(IDocument document, IStyle style, string styleName,
			string family, bool includeDefault, XmlNode contentNode)
		{
			string language;
			string country;
			ResolveHtmlValues(document, style, styleName, family, includeDefault, contentNode,
				"language", "country", out language, out country);
			if (language == null) return "";
			if (language == "none") language = "";
			else if (!String.IsNullOrEmpty(country) && country != "none") language += "-" + country;
			return " lang=\"" + System.Security.SecurityElement.Escape(language) + "\"";
		}

		internal static string GetHtmlHyphenationLimits(IDocument document, IStyle style, string styleName,
			string family, bool includeDefault, XmlNode contentNode)
		{
			string before;
			string after;
			ResolveHtmlValues(document, style, styleName, family, includeDefault, contentNode,
				"hyphenation-remain-char-count", "hyphenation-push-char-count", out before, out after);
			if (before == null && after == null) return "";
			return "hyphenate-limit-chars: auto " + HtmlHyphenationCount(before) + " " +
				HtmlHyphenationCount(after) + "; ";
		}

		internal string GetHtmlHyphenationLimits()
		{
			XmlAttribute family = this.Style.Node.Attributes["family",
				"urn:oasis:names:tc:opendocument:xmlns:style:1.0"];
			if (family == null || (family.Value != "paragraph" && family.Value != "text")) return "";
			return GetHtmlHyphenationLimits(this.Style.Document, this.Style, this.Style.StyleName,
				family.Value, family.Value == "paragraph", null);
		}

		private static string HtmlHyphenationCount(string value)
		{
			if (value == null) return "auto";
			int count;
			if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out count) || count < 1)
				throw new FormatException("HTML hyphenation character limits must be positive integers: " + value);
			return count.ToString(CultureInfo.InvariantCulture);
		}

		internal static string AddHtmlCss(string tag, string css)
		{
			if (css.Length == 0) return tag;
			int tagStart = tag.LastIndexOf('<');
			int styleStart = tag.IndexOf("style=\"", tagStart < 0 ? 0 : tagStart, StringComparison.Ordinal);
			if (styleStart < 0) return tag + " style=\"" + css + "\"";
			int styleEnd = tag.IndexOf('"', styleStart + 7);
			if (styleEnd < 0) throw new InvalidOperationException("Unterminated HTML style attribute.");
			return tag.Insert(styleEnd, css);
		}

		private static void ResolveHtmlValues(IDocument document, IStyle style, string styleName,
			string family, bool includeDefault, XmlNode contentNode, string firstName, string secondName,
			out string first, out string second)
		{
			ResolveStyleValues(document, style, styleName, family, includeDefault, firstName, secondName,
				out first, out second);
			if (family == "text" && (first != null || second != null) &&
				(first == null || second == null))
			{
				const string textNs = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
				for (XmlNode parent = contentNode == null ? null : contentNode.ParentNode;
					parent != null && (first == null || second == null); parent = parent.ParentNode)
				{
					if (parent.NamespaceURI != textNs || parent.Attributes == null) continue;
					bool isParagraph = parent.LocalName == "p" || parent.LocalName == "h";
					if (!isParagraph && parent.LocalName != "span") continue;
					XmlAttribute parentStyleName = parent.Attributes["style-name", textNs];
					string parentFirst;
					string parentSecond;
					ResolveStyleValues(document, null, parentStyleName == null ? null : parentStyleName.Value,
						isParagraph ? "paragraph" : "text", isParagraph, firstName, secondName,
						out parentFirst, out parentSecond);
					if (first == null) first = parentFirst;
					if (second == null) second = parentSecond;
				}
			}
		}

		private static void ResolveStyleValues(IDocument document, IStyle style, string styleName,
			string family, bool includeDefault, string firstName, string secondName, out string first, out string second)
		{
			const string styleNs = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";
			const string foNs = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0";
			XmlDocument styles = null;
			TextDocument text = document as TextDocument;
			if (text != null && text.DocumentStyles != null)
				styles = text.DocumentStyles.Styles;

			XmlNode current = style == null ? FindStyle(document, styles, styleName, family, true) : style.Node;
			first = null;
			second = null;
			Hashtable visited = new Hashtable();
			while (current != null)
			{
				if (visited.ContainsKey(current))
					throw new InvalidOperationException("Circular style inheritance while resolving HTML text properties.");
				visited[current] = true;
				XmlNode properties = current.SelectSingleNode("style:text-properties", document.NamespaceManager);
				if (properties != null)
				{
					XmlAttribute firstAttribute = properties.Attributes[firstName, foNs];
					XmlAttribute secondAttribute = properties.Attributes[secondName, foNs];
					if (first == null && firstAttribute != null)
						first = firstAttribute.Value;
					if (second == null && secondAttribute != null)
						second = secondAttribute.Value;
				}
				XmlAttribute parent = current.Attributes["parent-style-name", styleNs];
				current = parent == null ? null : FindStyle(document, styles, parent.Value, family, false);
			}

			if (includeDefault && styles != null && (first == null || second == null))
			{
				XmlNodeList defaults = styles.SelectNodes("/office:document-styles/office:styles/style:default-style",
					document.NamespaceManager);
				foreach (XmlNode defaultStyle in defaults)
				{
					XmlAttribute defaultFamily = defaultStyle.Attributes["family", styleNs];
					if (defaultFamily == null || defaultFamily.Value != family) continue;
					XmlNode properties = defaultStyle.SelectSingleNode("style:text-properties", document.NamespaceManager);
					if (properties != null)
					{
						XmlAttribute firstAttribute = properties.Attributes[firstName, foNs];
						XmlAttribute secondAttribute = properties.Attributes[secondName, foNs];
						if (first == null && firstAttribute != null) first = firstAttribute.Value;
						if (second == null && secondAttribute != null) second = secondAttribute.Value;
					}
					break;
				}
			}

		}

		private static XmlNode FindStyle(IDocument document, XmlDocument styles, string name, string family, bool automatic)
		{
			if (String.IsNullOrEmpty(name)) return null;
			if (automatic)
			{
				XmlNode node = FindStyleInCollection(document.Styles, name, family);
				if (node != null) return node;
			}
			XmlNode common = FindStyleInCollection(document.CommonStyles, name, family);
			if (common != null) return common;
			if (styles != null)
			{
				XmlNodeList nodes = styles.SelectNodes("/office:document-styles/office:styles/style:style",
					document.NamespaceManager);
				const string styleNs = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";
				foreach (XmlNode node in nodes)
				{
					XmlAttribute nodeName = node.Attributes["name", styleNs];
					XmlAttribute nodeFamily = node.Attributes["family", styleNs];
					if (nodeName != null && nodeFamily != null && nodeName.Value == name && nodeFamily.Value == family)
						return node;
				}
			}
			return null;
		}

		private static XmlNode FindStyleInCollection(IStyleCollection styles, string name, string family)
		{
			if (styles == null) return null;
			const string styleNs = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";
			foreach (IStyle style in styles)
			{
				if (style.Node == null || style.Node.Attributes == null) continue;
				XmlAttribute nodeFamily = style.Node.Attributes["family", styleNs];
				if (style.StyleName == name && nodeFamily != null && nodeFamily.Value == family)
					return style.Node;
			}
			return null;
		}

		#region IProperty Member
		private XmlNode _node;
		/// <summary>
		/// The XmlNode which represent the property element.
		/// </summary>
		/// <value>The node</value>
		public System.Xml.XmlNode Node
		{
			get
			{
				return this._node;
			}
			set
			{
				this._node = value;
			}
		}

		private IStyle _style;
		/// <summary>
		/// The style object to which this property object belongs
		/// </summary>
		/// <value></value>
		public IStyle Style
		{
			get { return this._style; }
			set { this._style = value; }
		}
		#endregion

		#region IHtmlStyle Member

		/// <summary>
		/// Get the css style fragement
		/// </summary>
		/// <returns>The css style attribute</returns>
		public string GetHtmlStyle()
		{
			string style		= "style=\"";

			if(this.Italic != null)
				if(this.Italic != "normal")
					style	+= "font-style: italic; ";
			if(this.Bold != null)
				style	+= "font-weight: "+this.Bold+"; ";
			if(this.Underline != null)
				style	+= "text-decoration: underline; ";
			if(this.TextLineThrough != null)
				style	+= "text-decoration: line-through; ";
			if(this.FontName != null)
				style	+= "font-family: "+FontFamilies.HtmlFont(this.FontName)+"; ";
			if(this.FontSize != null)
				style	+= "font-size: "+FontFamilies.PtToPx(this.FontSize)+"; ";
			if(this.FontColor != null)
				style	+= "color: "+this.FontColor+"; ";
			if(this.BackgroundColor != null)
				style	+= "background-color: "+this.BackgroundColor+"; ";
			style += GetTypographyHtml(this.Node, this.Style.Document.NamespaceManager);
			style += this.GetHtmlHyphenationLimits();

			if(!style.EndsWith("; "))
				style	= "";
			else
				style	+= "\"";

			return style;
		}

		#endregion
	}

	/// <summary>
	/// Propertie Helpers
	/// </summary>
	public class TextPropertieHelper
	{
		/// <summary>
		/// Subscript use within Postion
		/// </summary>
		public static readonly string Subscript		= "sub 58%";
		/// <summary>
		/// Superscript use within Postion
		/// </summary>
		public static readonly string Superscript	= "super 58%";
		/// <summary>
		/// Light shadow use within Shadow
		/// </summary>
		public static readonly string Shadowlight	= "1pt 1pt";
		/// <summary>
		/// Middle shadow use within Shadow
		/// </summary>
		public static readonly string Shadowmidlle	= "3pt 3pt";
		/// <summary>
		/// Heavy shadow use within Shadow
		/// </summary>
		public static readonly string Shadowheavy	= "6pt 6pt";
	}
}

/*
 * $Log: TextProperties.cs,v $
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.9  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.8  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.7  2005/11/23 19:18:17  larsbm
 * - New Textproperties
 * - New Paragraphproperties
 * - New Border Helper
 * - Textproprtie helper
 *
 * Revision 1.6  2005/10/23 16:47:48  larsbm
 * - Bugfix ListItem throws IStyleInterface not implemented exeption
 * - now. build the document after call saveto instead prepare the do at runtime
 * - add remove support for IText objects in the paragraph class
 *
 * Revision 1.5  2005/10/22 15:52:10  larsbm
 * - Changed some styles from Enum to Class with statics
 * - Add full support for available OpenOffice fonts
 *
 * Revision 1.4  2005/10/08 09:01:15  larsbm
 * --- uncommented ---
 *
 * Revision 1.3  2005/10/08 08:07:45  larsbm
 * - added cvs tags
 *
 * Revision 1.2  2005/10/08 07:55:35  larsbm
 * - added cvs tags
 *
 */
