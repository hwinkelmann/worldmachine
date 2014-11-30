using HtmlAgilityPack;
using Newtonsoft.Json;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Parsers
{
    public abstract class ParserBase
    {
        public abstract void Parse(HtmlDocument doc, FeedItem item);

        protected void SetArticleText(FeedItem item, HtmlNodeCollection nodes)
        {
            // The following line of code
            //  - sort nodes by their position in the HTML document,
            //  - grabs inner text and
            //  - joins it to a big string containing the whole article body
            String content = String.Join("\n", nodes.OrderBy(n => n.StreamPosition).Select(n => n.InnerText));

            // Sanitize article text
            content = WebUtility.HtmlDecode(content);

            item.Content = content;
        }

        /// <summary>
        /// Adds tags contained in the nodes provided. Strings will be sanitized!
        /// </summary>
        /// <param name="item">The item to add the tags to</param>
        /// <param name="nodes">Nodes containing the tag text</param>
        protected void AddTags(FeedItem item, HtmlNodeCollection nodes)
        {
            foreach (var node in nodes)
                AddTag(item, node.InnerText.Trim().ToLowerInvariant());

        }

        /// <summary>
        /// Adds a tag to the item's tag list. A bit inefficient, but still not worthwhile
        /// to optimize
        /// </summary>
        /// <param name="item">The item to which the tag should be added to</param>
        /// <param name="tag">The tag to add</param>
        protected void AddTag(FeedItem item, String tag)
        {
            List<String> tags = (item.Tags != null) ? JsonConvert.DeserializeObject<List<String>>(item.Tags) : new List<String>();
            tags.Add(tag);
            item.Tags = JsonConvert.SerializeObject(tags);
        }
    }
}
