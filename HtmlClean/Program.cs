
namespace HtmlClean
{
    using HtmlAgilityPack;
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    class Program
    {
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            var html = "<div class=WordSection1 ><p class=MsoNormal style='margin-top:12.0pt;margin-right:0cm;margin-bottom: 12.0pt;margin-left:0cm;mso-para-margin-top:1.0gd;mso-para-margin-right:0cm; mso-para-margin-bottom:1.0gd;mso-para-margin-left:0cm;text-align:justify; text-justify:inter-ideograph;line-height:normal' ><span lang=EN-GB style='font-size:12.0pt;mso-bidi-font-size:11.0pt;font-family:\"Arial\",\"sans-serif\"; color:black;mso-ansi-language:EN-GB'>Lorem Ipsum</span></p> </div>";

            doc.LoadHtml(html);

            clean(doc.DocumentNode);

            Console.WriteLine(doc.DocumentNode.InnerHtml);
            Console.ReadLine();
        }

        static void clean(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return;
            }

            if (node.NodeType == HtmlNodeType.Comment)
            {
                node.Remove();
                return;
            }
            
            if (node.Name.Contains(":"))
            {
                node.Remove();
                return;
            }

            CleanCss(node);

            if (node.Attributes.Contains("lang"))
            {
                node.Attributes.Remove("lang");
            }


            for (int i = node.Attributes.Count - 1; i >= 0; i--)
            {
                var child = node.ChildNodes[i];
                clean(child);
            }

            if(node.Name == "span" && node.InnerLength == 0)
            {
                node.Remove();
            }
        }

        static void CleanCss(HtmlNode node)
        {
            var style = node.GetAttributeValue("style", string.Empty);
            if (string.IsNullOrEmpty(style))
            {
                return;
            }

            var css = new Panel().Style;
            css.Value = style.Replace("&quot;", "\"");
            var keys = new string[css.Keys.Count];
            css.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                if ((key).StartsWith("mso-"))
                {
                    css.Remove(key);
                    continue;
                }
                var value = css[key];
                switch (key)
                {
                    case "font-size":
                        if(value == "12.0pt")
                        {
                            css.Remove(key);
                        }
                        break;
                    case "font-family":
                        if (value == "\"Arial\",\"sans-serif\""
                            || value == "\"Arial\",sans-serif"
                            || value == "Arial,sans-serif")
                        {
                            css.Remove(key);
                        }
                        break;
                    case "color":
                        if (value == "black")
                        {
                            css.Remove(key);
                        }
                        break;
                    case "text-justify":
                    case "text-align":
                    case "line-height":
                    case "margin":
                    case "margin-top":
                    case "margin-left":
                    case "margin-right":
                    case "margin-bottom":
                        css.Remove(key);
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(css.Value))
            {
                node.Attributes.Remove("style");
                return;
            }

            node.SetAttributeValue("style", css.Value ?? string.Empty);
        }
    }
}
