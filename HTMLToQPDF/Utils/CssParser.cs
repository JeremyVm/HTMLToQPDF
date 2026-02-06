using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Utils
{
    /// <summary>
    /// Parses inline CSS style attributes and applies them to QuestPDF styles.
    /// </summary>
    public static class CssParser
    {
        /// <summary>
        /// Parses a CSS style string into a dictionary of property-value pairs.
        /// </summary>
        public static Dictionary<string, string> ParseStyleAttribute(string? styleAttribute)
        {
            var styles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(styleAttribute))
                return styles;

            var declarations = styleAttribute.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var declaration in declarations)
            {
                var colonIndex = declaration.IndexOf(':');
                if (colonIndex > 0)
                {
                    var property = declaration.Substring(0, colonIndex).Trim().ToLowerInvariant();
                    var value = declaration.Substring(colonIndex + 1).Trim();
                    if (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(value))
                    {
                        styles[property] = value;
                    }
                }
            }

            return styles;
        }

        /// <summary>
        /// Applies CSS styles to a TextStyle object.
        /// </summary>
        public static TextStyle ApplyTextStyles(TextStyle baseStyle, Dictionary<string, string> cssStyles)
        {
            var style = baseStyle;

            foreach (var css in cssStyles)
            {
                style = ApplyTextProperty(style, css.Key, css.Value);
            }

            return style;
        }

        /// <summary>
        /// Applies a single CSS property to a TextStyle.
        /// </summary>
        private static TextStyle ApplyTextProperty(TextStyle style, string property, string value)
        {
            switch (property)
            {
                case "color":
                    var textColor = ParseColor(value);
                    if (textColor.HasValue)
                        style = style.FontColor(textColor.Value);
                    break;

                case "background-color":
                case "background":
                    var bgColor = ParseColor(value);
                    if (bgColor.HasValue)
                        style = style.BackgroundColor(bgColor.Value);
                    break;

                case "font-size":
                    var fontSize = ParseLength(value);
                    if (fontSize.HasValue)
                        style = style.FontSize(fontSize.Value);
                    break;

                case "font-weight":
                    style = ApplyFontWeight(style, value);
                    break;

                case "font-style":
                    if (value.Equals("italic", StringComparison.OrdinalIgnoreCase) ||
                        value.Equals("oblique", StringComparison.OrdinalIgnoreCase))
                        style = style.Italic();
                    break;

                case "font-family":
                    var fontFamily = ParseFontFamily(value);
                    if (!string.IsNullOrEmpty(fontFamily))
                        style = style.FontFamily(fontFamily);
                    break;

                case "text-decoration":
                case "text-decoration-line":
                    style = ApplyTextDecoration(style, value);
                    break;

                case "letter-spacing":
                    var letterSpacing = ParseLength(value);
                    if (letterSpacing.HasValue)
                        style = style.LetterSpacing(letterSpacing.Value);
                    break;

                case "line-height":
                    var lineHeight = ParseLineHeight(value);
                    if (lineHeight.HasValue)
                        style = style.LineHeight(lineHeight.Value);
                    break;
            }

            return style;
        }

        /// <summary>
        /// Applies CSS styles to a container.
        /// </summary>
        public static IContainer ApplyContainerStyles(IContainer container, Dictionary<string, string> cssStyles)
        {
            foreach (var css in cssStyles)
            {
                container = ApplyContainerProperty(container, css.Key, css.Value);
            }

            return container;
        }

        /// <summary>
        /// Applies a single CSS property to a container.
        /// </summary>
        private static IContainer ApplyContainerProperty(IContainer container, string property, string value)
        {
            switch (property)
            {
                case "background-color":
                case "background":
                    var bgColor = ParseColor(value);
                    if (bgColor.HasValue)
                        container = container.Background(bgColor.Value);
                    break;

                case "padding":
                    var padding = ParseLength(value);
                    if (padding.HasValue)
                        container = container.Padding(padding.Value);
                    break;

                case "padding-top":
                    var paddingTop = ParseLength(value);
                    if (paddingTop.HasValue)
                        container = container.PaddingTop(paddingTop.Value);
                    break;

                case "padding-bottom":
                    var paddingBottom = ParseLength(value);
                    if (paddingBottom.HasValue)
                        container = container.PaddingBottom(paddingBottom.Value);
                    break;

                case "padding-left":
                    var paddingLeft = ParseLength(value);
                    if (paddingLeft.HasValue)
                        container = container.PaddingLeft(paddingLeft.Value);
                    break;

                case "padding-right":
                    var paddingRight = ParseLength(value);
                    if (paddingRight.HasValue)
                        container = container.PaddingRight(paddingRight.Value);
                    break;

                case "margin":
                    // QuestPDF doesn't have direct margin support, use padding as approximation
                    var margin = ParseLength(value);
                    if (margin.HasValue)
                        container = container.Padding(margin.Value);
                    break;

                case "margin-top":
                    var marginTop = ParseLength(value);
                    if (marginTop.HasValue)
                        container = container.PaddingTop(marginTop.Value);
                    break;

                case "margin-bottom":
                    var marginBottom = ParseLength(value);
                    if (marginBottom.HasValue)
                        container = container.PaddingBottom(marginBottom.Value);
                    break;

                case "margin-left":
                    var marginLeft = ParseLength(value);
                    if (marginLeft.HasValue)
                        container = container.PaddingLeft(marginLeft.Value);
                    break;

                case "margin-right":
                    var marginRight = ParseLength(value);
                    if (marginRight.HasValue)
                        container = container.PaddingRight(marginRight.Value);
                    break;

                case "border":
                    var borderWidth = ParseBorderWidth(value);
                    if (borderWidth.HasValue)
                        container = container.Border(borderWidth.Value);
                    break;

                case "border-color":
                    var borderColor = ParseColor(value);
                    if (borderColor.HasValue)
                        container = container.BorderColor(borderColor.Value);
                    break;

                case "width":
                    var width = ParseLength(value);
                    if (width.HasValue)
                        container = container.Width(width.Value);
                    break;

                case "max-width":
                    var maxWidth = ParseLength(value);
                    if (maxWidth.HasValue)
                        container = container.MaxWidth(maxWidth.Value);
                    break;

                case "min-width":
                    var minWidth = ParseLength(value);
                    if (minWidth.HasValue)
                        container = container.MinWidth(minWidth.Value);
                    break;

                case "height":
                    var height = ParseLength(value);
                    if (height.HasValue)
                        container = container.Height(height.Value);
                    break;

                case "max-height":
                    var maxHeight = ParseLength(value);
                    if (maxHeight.HasValue)
                        container = container.MaxHeight(maxHeight.Value);
                    break;

                case "min-height":
                    var minHeight = ParseLength(value);
                    if (minHeight.HasValue)
                        container = container.MinHeight(minHeight.Value);
                    break;

                case "text-align":
                    container = ApplyTextAlign(container, value);
                    break;
            }

            return container;
        }

        /// <summary>
        /// Parses a CSS color value to a QuestPDF Color.
        /// Supports: hex (#RGB, #RRGGBB, #RRGGBBAA), rgb(), rgba(), and named colors.
        /// </summary>
        public static Color? ParseColor(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToLowerInvariant();

            // Handle hex colors
            if (value.StartsWith("#"))
            {
                return ParseHexColor(value);
            }

            // Handle rgb() and rgba()
            if (value.StartsWith("rgb"))
            {
                return ParseRgbColor(value);
            }

            // Handle named colors
            return GetNamedColor(value);
        }

        private static Color? ParseHexColor(string hex)
        {
            hex = hex.TrimStart('#');

            try
            {
                if (hex.Length == 3)
                {
                    // #RGB -> #RRGGBB
                    hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
                }

                if (hex.Length == 6)
                {
                    return Color.FromHex(hex);
                }

                if (hex.Length == 8)
                {
                    // #RRGGBBAA - QuestPDF Color.FromHex supports this
                    return Color.FromHex(hex);
                }
            }
            catch
            {
                // Invalid hex color
            }

            return null;
        }

        private static Color? ParseRgbColor(string value)
        {
            var match = Regex.Match(value, @"rgba?\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)");
            if (match.Success)
            {
                if (byte.TryParse(match.Groups[1].Value, out byte r) &&
                    byte.TryParse(match.Groups[2].Value, out byte g) &&
                    byte.TryParse(match.Groups[3].Value, out byte b))
                {
                    return Color.FromRGB(r, g, b);
                }
            }

            return null;
        }

        private static Color? GetNamedColor(string name)
        {
            return name switch
            {
                "black" => Colors.Black,
                "white" => Colors.White,
                "red" => Colors.Red.Medium,
                "green" => Colors.Green.Medium,
                "blue" => Colors.Blue.Medium,
                "yellow" => Colors.Yellow.Medium,
                "orange" => Colors.Orange.Medium,
                "purple" => Colors.Purple.Medium,
                "pink" => Colors.Pink.Medium,
                "brown" => Colors.Brown.Medium,
                "grey" or "gray" => Colors.Grey.Medium,
                "lightgrey" or "lightgray" => Colors.Grey.Lighten3,
                "darkgrey" or "darkgray" => Colors.Grey.Darken3,
                "cyan" or "aqua" => Colors.Cyan.Medium,
                "magenta" or "fuchsia" => Colors.Pink.Medium,
                "lime" => Colors.LightGreen.Medium,
                "navy" => Colors.Blue.Darken4,
                "teal" => Colors.Teal.Medium,
                "olive" => Colors.Lime.Darken3,
                "maroon" => Colors.Red.Darken4,
                "silver" => Colors.Grey.Lighten2,
                "transparent" => Colors.Transparent,
                _ => (Color?)null
            };
        }

        /// <summary>
        /// Parses a CSS length value to points.
        /// Supports: px, pt, em, rem, cm, mm, in, %.
        /// </summary>
        public static float? ParseLength(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToLowerInvariant();

            // Handle "auto" and similar non-numeric values
            if (value == "auto" || value == "inherit" || value == "initial")
                return null;

            // Try to extract number and unit
            var match = Regex.Match(value, @"^(-?\d*\.?\d+)\s*(px|pt|em|rem|cm|mm|in|%)?$");
            if (!match.Success)
                return null;

            if (!float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float number))
                return null;

            var unit = match.Groups[2].Value;

            return unit switch
            {
                "px" => number * 0.75f,          // 1px ≈ 0.75pt
                "pt" => number,                   // points (default)
                "em" or "rem" => number * 12f,   // Assume 12pt base font
                "cm" => number * 28.3465f,       // 1cm ≈ 28.35pt
                "mm" => number * 2.83465f,       // 1mm ≈ 2.835pt
                "in" => number * 72f,            // 1in = 72pt
                "%" => number * 0.12f,           // Rough approximation for percentages
                "" => number,                     // No unit, assume points
                _ => number
            };
        }

        private static float? ParseLineHeight(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToLowerInvariant();

            // Handle unitless values (multiplier)
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float multiplier))
            {
                return multiplier;
            }

            // Handle normal keyword
            if (value == "normal")
                return 1.2f;

            // Handle percentage
            if (value.EndsWith("%"))
            {
                var percentStr = value.TrimEnd('%');
                if (float.TryParse(percentStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float percent))
                {
                    return percent / 100f;
                }
            }

            return null;
        }

        private static float? ParseBorderWidth(string value)
        {
            // Simple border parsing - extract width from "1px solid black" format
            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                return ParseLength(parts[0]);
            }
            return null;
        }

        private static TextStyle ApplyFontWeight(TextStyle style, string value)
        {
            value = value.ToLowerInvariant();

            if (value == "bold" || value == "bolder" || value == "700" || value == "800" || value == "900")
            {
                return style.Bold();
            }

            if (value == "normal" || value == "400")
            {
                return style.NormalWeight();
            }

            if (value == "lighter" || value == "100" || value == "200" || value == "300")
            {
                return style.Light();
            }

            // Try numeric weight
            if (int.TryParse(value, out int weight))
            {
                if (weight >= 600)
                    return style.Bold();
                if (weight <= 300)
                    return style.Light();
            }

            return style;
        }

        private static TextStyle ApplyTextDecoration(TextStyle style, string value)
        {
            value = value.ToLowerInvariant();

            if (value.Contains("underline"))
                style = style.Underline();

            if (value.Contains("line-through"))
                style = style.Strikethrough();

            return style;
        }

        private static IContainer ApplyTextAlign(IContainer container, string value)
        {
            return value.ToLowerInvariant() switch
            {
                "center" => container.AlignCenter(),
                "right" => container.AlignRight(),
                "left" => container.AlignLeft(),
                _ => container
            };
        }

        /// <summary>
        /// Parses the first font family from a CSS font-family declaration.
        /// </summary>
        private static string? ParseFontFamily(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Split by comma and take first font
            var fonts = value.Split(',');
            if (fonts.Length == 0)
                return null;

            var font = fonts[0].Trim().Trim('"', '\'');

            // Map generic families to actual fonts
            return font.ToLowerInvariant() switch
            {
                "monospace" => "Courier New",
                "serif" => "Times New Roman",
                "sans-serif" => "Arial",
                "cursive" => "Comic Sans MS",
                "fantasy" => "Impact",
                _ => font
            };
        }
    }
}
