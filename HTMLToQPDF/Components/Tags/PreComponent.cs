using System;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HTMLQuestPDF.Utils;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components.Tags
{
    /// <summary>
    /// Component for rendering preformatted text (pre) elements.
    /// Preserves whitespace and uses a monospace font.
    /// </summary>
    internal class PreComponent : IComponent
    {
        protected readonly HTMLComponentsArgs args;
        protected readonly HtmlNode node;

        public PreComponent(HtmlNode node, HTMLComponentsArgs args)
        {
            this.node = node;
            this.args = args;
        }

        public void Compose(IContainer container)
        {
            // Apply any inline styles
            var styleAttr = node.GetAttributeValue("style", null);
            var cssStyles = CssParser.ParseStyleAttribute(styleAttr);

            // Default styling for preformatted text
            var textStyle = TextStyle.Default
                .FontFamily("Courier New")
                .FontSize(10);

            // Apply CSS text styles
            textStyle = CssParser.ApplyTextStyles(textStyle, cssStyles);

            // Default container styling (light gray background with padding)
            IContainer styled = container
                .PaddingVertical(8)
                .Background(Colors.Grey.Lighten4)
                .Padding(10);

            // Apply CSS container styles
            styled = CssParser.ApplyContainerStyles(styled, cssStyles);

            // Get the inner text, preserving whitespace
            var text = GetPreformattedText(node);

            styled.Text(t =>
            {
                t.DefaultTextStyle(textStyle);
                t.Span(text);
            });
        }

        /// <summary>
        /// Extracts text from the node while preserving whitespace and newlines.
        /// </summary>
        private string GetPreformattedText(HtmlNode node)
        {
            // Get raw inner HTML and decode entities
            var html = node.InnerHtml;

            // Decode HTML entities
            html = WebUtility.HtmlDecode(html);

            // Replace <br> tags with newlines
            html = Regex.Replace(
                html,
                @"<br\s*/?>",
                "\n",
                RegexOptions.IgnoreCase);

            // Remove any other HTML tags but keep content
            html = Regex.Replace(
                html,
                @"<[^>]+>",
                "");

            return html;
        }
    }
}
