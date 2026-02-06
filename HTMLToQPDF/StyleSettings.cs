using System.Collections.Generic;
using HtmlAgilityPack;
using HTMLQuestPDF.Components;
using HTMLQuestPDF.Components.Tags;
using HTMLToQPDF.Components;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF
{
    internal static class HTMLMapSettings
    {
        public static readonly string[] LineElements = new string[] {
            "a",
            "abbr",
            "b",
            "br",
            "code",
            "del",
            "em",
            "i",
            "kbd",
            "mark",
            "q",
            "s",
            "samp",
            "small",
            "space",
            "span",
            "strike",
            "strong",
            "sub",
            "sup",
            "tbody",
            "td",
            "th",
            "thead",
            "tr",
            "u",
            "var",
            "img",
            "text"
        };

        public static readonly string[] BlockElements = new string[] {
            "#document",
            "blockquote",
            "div",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "hr",
            "li",
            "ol",
            "p",
            "pre",
            "table",
            "ul",
            "section",
            "header",
            "footer",
            "head",
            "html"
        };

        public static IComponent GetComponent(this HtmlNode node, HTMLComponentsArgs args)
        {
            return node.Name.ToLower() switch
            {
                "#text" or "h1" or "h2" or "h3" or "h4" or "h5" or "h6" or "b" or "s" or "strike" or "i" or "small" or "u" or "del" or "em" or "strong" or "sub" or "sup"
                or "span" or "code" or "mark" or "abbr" or "kbd" or "samp" or "var" or "q"
                    => new ParagraphComponent(new List<HtmlNode>() { node }, args),
                "br" => new BrComponent(node, args),
                "hr" => new HrComponent(node, args),
                "a" => new AComponent(node, args),
                "div" => new BaseHTMLComponent(node, args),
                "blockquote" => new BlockquoteComponent(node, args),
                "pre" => new PreComponent(node, args),
                "table" => new TableComponent(node, args),
                "ul" or "ol" => new ListComponent(node, args),
                "img" => new ImgComponent(node, args),
                _ => new BaseHTMLComponent(node, args),
            };
        }
    }
}