using HtmlAgilityPack;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Parsers
{
    public class TheRegisterParser : ParserBase
    {
        public override void Parse(HtmlDocument doc, FeedItem item)
        {
            var storyNodes = doc.DocumentNode.SelectNodes("//div[@id=\"body\"]/p[not(@class)]");
            SetArticleText(item, storyNodes);

            var tagNodes = doc.DocumentNode.SelectNodes("//script[@type=\"text/javascript\"]");

            foreach (var node in tagNodes)
                if (node.InnerText.Contains("kw:[["))
                {
                    var js = node.InnerText.Trim().Substring(15);
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(js);

                    foreach (var element in data["kw"])
                        AddTag(item, element[1].ToString());
                }
        }
    }
}
