using HtmlAgilityPack;
using Newtonsoft.Json;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Parsers
{
    public class XpathParser : ParserBase
    {
        public class XpathParserConfiguration
        {
            public String XpathTags;
            public String XpathContent;

            public XpathParserConfiguration(String xpathTags, String xpathContent)
            {
                this.XpathContent = xpathContent;
                this.XpathTags = xpathTags;
            }
        }

        XpathParserConfiguration config;

        public XpathParser(String configuration)
            : base()
        {
            config = JsonConvert.DeserializeObject<XpathParserConfiguration>(configuration);
        }

        public override void Parse(HtmlDocument doc, FeedItem item)
        {
            var storyNodes = doc.DocumentNode.SelectNodes(config.XpathContent);
            SetArticleText(item, storyNodes);

            var tagNodes = doc.DocumentNode.SelectNodes(config.XpathTags);
            AddTags(item, tagNodes);
        }
    }
}
