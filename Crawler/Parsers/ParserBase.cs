﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.Parsers
{
    public abstract class ParserBase
    {
        public abstract void Parse(HtmlDocument doc, FeedItem item);

        protected void SetArticleText(FeedItem item, HtmlNodeCollection nodes)
        {
            System.Diagnostics.Debug.Assert(item != null, "item is null, but must not!");
            if (nodes == null)
                return;

            // The following line of code
            //  - sort nodes by their position in the HTML document,
            //  - grabs inner text and
            //  - joins it to a big string containing the whole article body
            String content = String.Join("\n", nodes.OrderBy(n => n.StreamPosition).Select(n => n.InnerText));

            // Sanitize article text
            content = SanitizeText(content);

            item.Content = content;
        }

        /// <summary>
        /// Adds tags contained in the nodes provided. Strings will be sanitized!
        /// </summary>
        /// <param name="item">The item to add the tags to</param>
        /// <param name="nodes">Nodes containing the tag text</param>
        protected void AddTags(FeedItem item, HtmlNodeCollection nodes)
        {
            if (nodes == null)
                return;

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
            item.Tags = JsonConvert.SerializeObject(tags.Distinct());
            item.TagCount = tags.Distinct().Count();
        }

        /// <summary>
        /// Sanitizes text retrieved from the web
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String SanitizeText(String text)
        {
            text = WebUtility.HtmlDecode(text.Trim());

            while (text != text.Replace("\n\n", "\n"))
                text = text.Replace("\n\n", "\n");

            // Get rid of HTML comments
            // https://stackoverflow.com/questions/10656123/regular-expression-remove-html-comment-spanning-multiple-line-breaks
            text = Regex.Replace(text, "<!--.*?-->", "", RegexOptions.Singleline);

            return text;
        }

        public static ParserBase CreateParser(int parserId, String jsonConfiguration)
        {
            switch (parserId)
            {
                case 0:
                    return new XpathParser(jsonConfiguration);

                case 1:
                    return new TheRegisterParser();

                default:
                    throw new Exception("Invalid Parser ID");
            }

        }
    }
}
