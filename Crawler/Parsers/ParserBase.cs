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
    public abstract class ParserBase
    {
        /// <summary>
        /// Parses the HTML document and updates both feed and item accordingly
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="item"></param>
        public abstract void Parse(HtmlDocument doc, FeedItem item);

        /// <summary>
        /// Adds the given Tag to the entity. Some encoding/decoding overhead here,
        /// but optimizing this is probably not worthwhile...
        /// </summary>
        /// <param name="tag"></param>
        protected void AddTag(FeedItem item, String tag)
        {
            List<String> tags;
            if (item.Tags == null) 
                tags = new List<string>();
            else
                tags = JsonConvert.DeserializeObject<List<String>>(item.Tags);

            tags.Add(tag);

            item.Tags = JsonConvert.SerializeObject(tags);
        }
    }
}
