using HtmlAgilityPack;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Parsers
{
    public class TestParser : ParserBase
    {
        public override void Parse(HtmlDocument doc, FeedItem item)
        {
            if (!item.Feed.Name.Contains("Huffington"))
                return;


        }
    }
}
