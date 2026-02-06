using HtmlAgilityPack;
using HTMLQuestPDF.Utils;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components.Tags
{
    /// <summary>
    /// Component for rendering blockquote elements.
    /// Displays with left border and indentation.
    /// </summary>
    internal class BlockquoteComponent : BaseHTMLComponent
    {
        public BlockquoteComponent(HtmlNode node, HTMLComponentsArgs args) : base(node, args)
        {
        }

        protected override IContainer ApplyStyles(IContainer container)
        {
            // Apply any inline styles
            var styleAttr = node.GetAttributeValue("style", null);
            var cssStyles = CssParser.ParseStyleAttribute(styleAttr);

            // Default blockquote styling: left border, padding, and slight background
            container = container
                .PaddingVertical(8)
                .BorderLeft(3)
                .BorderColor(Colors.Grey.Medium)
                .Background(Colors.Grey.Lighten5)
                .PaddingLeft(16)
                .PaddingVertical(8);

            // Apply CSS container styles
            container = CssParser.ApplyContainerStyles(container, cssStyles);

            // Also check for tag-based styles
            if (args.ContainerStyles.TryGetValue("blockquote", out var style))
            {
                container = style(container);
            }

            return container;
        }
    }
}
