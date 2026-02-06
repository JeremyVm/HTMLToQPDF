using HtmlAgilityPack;
using HTMLQuestPDF.Utils;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components.Tags
{
    /// <summary>
    /// Component for rendering horizontal rule (hr) elements.
    /// </summary>
    internal class HrComponent : IComponent
    {
        private readonly HtmlNode node;
        private readonly HTMLComponentsArgs args;

        public HrComponent(HtmlNode node, HTMLComponentsArgs args)
        {
            this.node = node;
            this.args = args;
        }

        public void Compose(IContainer container)
        {
            // Apply any inline styles
            var styleAttr = node.GetAttributeValue("style", null);
            var cssStyles = CssParser.ParseStyleAttribute(styleAttr);

            // Default styling
            float lineHeight = 1;
            var color = Colors.Grey.Medium;

            // Check for CSS overrides for the line itself
            if (cssStyles.TryGetValue("height", out var heightValue) ||
                cssStyles.TryGetValue("border-width", out heightValue))
            {
                var parsedHeight = CssParser.ParseLength(heightValue);
                if (parsedHeight.HasValue)
                    lineHeight = parsedHeight.Value;
            }

            if (cssStyles.TryGetValue("background-color", out var bgColor) ||
                cssStyles.TryGetValue("border-color", out bgColor) ||
                cssStyles.TryGetValue("color", out bgColor))
            {
                var parsedColor = CssParser.ParseColor(bgColor);
                if (parsedColor.HasValue)
                    color = parsedColor.Value;
            }

            // Use a column to properly structure: spacing + line + spacing
            container.Column(column =>
            {
                // Top spacing
                column.Item().Height(8);

                // The actual horizontal line
                column.Item().Height(lineHeight).Background(color);

                // Bottom spacing
                column.Item().Height(8);
            });
        }
    }
}
