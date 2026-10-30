aodl
====

ZanyAnts' fork of AODL, the .net library for creating/editing documents in the OpenDocument Format (ODF).

Writer paragraph styles expose margins, first-line indentation and pagination
controls through `ParagraphStyle.ParagraphProperties`. Lengths use ODF values
such as `"1cm"` or `"-0.5cm"`; pagination values use `"always"` or `"auto"`,
and widow/orphan counts are strings.

```csharp
ParagraphProperties properties = paragraph.ParagraphStyle.ParagraphProperties;
properties.MarginTop = "0.2cm";
properties.MarginBottom = "0.3cm";
properties.TextIndent = "0.5cm";
properties.KeepWithNext = "always";
properties.Widows = "2";
properties.Orphans = "2";
```

Set a new property to `null` to remove its explicit attribute and allow style
inheritance. HTML export emits corresponding CSS where available.

Text and paragraph styles also expose western-script language, country, letter
spacing, and automatic hyphenation via `TextProperties`:

```csharp
TextProperties text = paragraph.ParagraphStyle.TextProperties;
text.Language = "sv";
text.Country = "SE";
text.LetterSpacing = "0.5pt";
text.Hyphenate = "true";
text.HyphenationRemainCharCount = "3";
text.HyphenationPushCharCount = "2";
```

These properties use ODF string values and can be removed with `null`. HTML
export includes letter spacing and hyphenation CSS and emits `lang` attributes
for paragraph, heading and text styles. Hyphenation character limits use
`hyphenate-limit-chars`, which is not supported by every browser. HTML export
requires positive integer limits; browser hyphenation also depends on language support.