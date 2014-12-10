using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;
using Crawler.Retrieval;
using Shared.Entities;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using Crawler.Parsers;

namespace Crawler
{
    class Program
    {
        #region Properties
        internal static Logging Logger = new Logging();

        internal static DownloadManager<Feed> feedDownloadManager = new DownloadManager<Feed>();
        internal static DownloadManager<FeedItem> itemDownloadManager = new DownloadManager<FeedItem>();
        #endregion

        static void Main(string[] args)
        {
            using (Context context = new Context())
                context.Database.CreateIfNotExists();

            itemDownloadManager.OnItemDownloaded += onItemDownloaded;
            feedDownloadManager.OnItemDownloaded += onFeedDownloaded;

            // Request items still waiting to be fetched
            using (Context context = new Context())
                foreach (var item in context.FeedItems.Where(i => i.State == FeedItem.States.Discovered && i.Feed.Enabled))
                    try
                    {
                        itemDownloadManager.Enqueue(new Uri(item.Url), item);
                    }
                    catch { }

            Console.WriteLine("Worldmachine Crawler. Terminate with enter.");

            checkFeeds();

            Timer timer = new Timer((state) =>
            {
                checkFeeds();
            }, null, 60000, 60000);

            Console.ReadLine();

            if (timer != null)
                try { timer.Dispose(); }
                finally { timer = null; }

            Dispose();
        }

        public static void Dispose()
        {
            if (itemDownloadManager != null)
                try { itemDownloadManager.Dispose(); }
                finally { itemDownloadManager = null; }

            if (feedDownloadManager != null)
                try { feedDownloadManager.Dispose(); }
                catch { }
                finally { feedDownloadManager = null; } 
        }

        #region Update Feeds
        static void onFeedDownloaded(Uri uri, Feed entity, byte[] data)
        {
            using (Context context = new Context())
            {
                nJupiter.Web.Syndication.IFeed feed;

                try
                {
                    using (MemoryStream ms = new MemoryStream(data))
                        feed = nJupiter.Web.Syndication.FeedReader.GetFeed(ms, uri);
                }
                catch (Exception exc)
                {
                    Logger.Log(LogLevels.Error, "Could not parse " + uri, exc);
                    return;
                }

                var parsedItemGuids = feed.Items.Select(f => getGuid(f)).Distinct().ToArray();
                var existingItemGuids = context.FeedItems.Where(i => parsedItemGuids.Contains(i.Guid)).Select(i => i.Guid);
                var newItems = feed.Items.Where(i => !existingItemGuids.Contains(getGuid(i)));

                if (newItems.Any())
                    Logger.Log(LogLevels.Notice, entity.Name + ": " + newItems.Count() + " new items");
                else
                    Logger.Log(LogLevels.Informative, entity.Name + ": No new items");

                foreach (var item in newItems)
                {
                    // Create Entity
                    FeedItem itemEntity = new FeedItem()
                    {
                        FeedId = entity.Id,
                        Guid = getGuid(item),
                        Published = (item.PublishDate == DateTime.MinValue) ? DateTime.UtcNow : item.PublishDate,
                        Title = item.Title,
                        State = FeedItem.States.Discovered,
                        Author = (item.Author != null)? item.Author.Name : null,
                        Url = item.Uri.ToString()
                    };

                    context.FeedItems.Add(itemEntity);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception exc)
                    {
                        Logger.Log(LogLevels.Error, "Could not write new item to db, skipping. Feed: " + entity.Name + " (" + entity.Id + "), Title: " + itemEntity.Title + " ( " + itemEntity.Guid + ")", exc);
                        continue;
                    }

                    // Request download of item
                    itemDownloadManager.Enqueue(new Uri(itemEntity.Url), itemEntity);
                }
            }
        }

        private static void checkFeeds()
        {
            using (Context context = new Context())
            {
                var feeds = context.Feeds.Where(feed => feed.Enabled && (feed.LastUpdate == null || DbFunctions.DiffMinutes(feed.LastUpdate.Value, DateTime.UtcNow) > feed.UpdateInterval)).ToArray();

                foreach (var feed in feeds)
                {
                    Uri uri;
                    try
                    {
                        uri = new Uri(feed.RssUrl);
                    }
                    catch
                    {
                        Program.Logger.Log(LogLevels.Warning, String.Format("Feed {0}: Invalid RssUrl '{1}'. Disabling feed!", feed.Id, feed.RssUrl));

                        feed.Enabled = false;
                        context.SaveChanges();
                        continue;
                    }
                    feedDownloadManager.Enqueue(uri, feed, 1);
                    feed.LastUpdate = DateTime.UtcNow;
                }

                context.SaveChanges();
            }
        }
        #endregion

        #region Process Feed Items
        static void onItemDownloaded(Uri uri, FeedItem _item, byte[] data)
        {
            //Program.Logger.Log(LogLevels.Informative, "Download complete: " + uri);

            // Grab a fresh entity
            using (Context context = new Context())
            {
                var item = context.FeedItems.Where(i => i.Id == _item.Id).FirstOrDefault();
                item.State = FeedItem.States.Fetched;

                try
                {
                    System.Diagnostics.Debug.Assert(item != null);

                    // Parse html document
                    HtmlAgilityPack.HtmlDocument doc;
                    try
                    {
                        doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(Encoding.UTF8.GetString(data));
                    }
                    catch (Exception exc)
                    {
                        Logger.Log(LogLevels.Error, "Invalid HTML parsing " + item.Title + " / " + item.Feed.Name, exc);
                        item.State = FeedItem.States.FailedInvalidHtml;
                        return;
                    }

                    // Create parser and use it to extract tags and content
                    var parser = Parsers.ParserBase.CreateParser(item.Feed.ParserId, item.Feed.ParserConfiguration);
                    parser.Parse(doc, item);

                    String[] tags = (item.Tags == null)? new String[0] : Newtonsoft.Json.JsonConvert.DeserializeObject<String[]>(item.Tags);
                    int numWords = (item.Content == null) ? 0 : item.Content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    String log = String.Format("{3}: {0} ({1} tags, {2} words)", item.Title, tags.Length, numWords, item.Feed.Name);
                    
                    Logger.Log(LogLevels.Informative, log);
                }
                finally
                {
                    // Anyway, we want to change our changes
                    context.SaveChanges();
                }
            }
        }
        #endregion

        #region Helpers
        private static string getGuid(nJupiter.Web.Syndication.IFeedItem item)
        {
            if (!String.IsNullOrEmpty(item.Id))
                return item.Id;

            return item.Uri.ToString();
        }
        #endregion
    }
}
