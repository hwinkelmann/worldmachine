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
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace Crawler
{
    class Program
    {
        internal static Logging Logger = new Logging();

        internal static DownloadManager<Feed> feedDownloadManager = new DownloadManager<Feed>();
        internal static DownloadManager<FeedItem> itemDownloadManager = new DownloadManager<FeedItem>();

        static void Main(string[] args)
        {
            itemDownloadManager.OnItemDownloaded += downloadManager_OnItemDownloaded;
            feedDownloadManager.OnItemDownloaded += feedDownloadManager_OnItemDownloaded;
            checkFeeds();

            Console.ReadLine();
        }

        static void feedDownloadManager_OnItemDownloaded(Uri uri, Feed entity, string body)
        {
            Logger.Log(LogLevels.Informative, "Feed " + entity.Name + " updated");

            using (Context context = new Context())
            {
                var feed = SyndicationFeed.Load(XDocument.Parse(body).CreateReader());

                var parsedItemGuids = feed.Items.Select(f => getGuid(f)).Distinct().ToArray();
                var existingItemGuids = context.FeedItems.Where(i => parsedItemGuids.Contains(i.Guid)).Select(i => i.Guid);
                var newItems = feed.Items.Where(i => !existingItemGuids.Contains(getGuid(i)));

                foreach (var item in newItems)
                    itemDownloadManager.Enqueue(item.Links.FirstOrDefault().Uri, new FeedItem()
                    {
                        FeedId = entity.Id,
                        Guid = getGuid(item),
                        Url = item.Links.FirstOrDefault().Uri.ToString()
                    });
            }
        }

        private static void checkFeeds()
        {
            using (Context context = new Context())
            {
                var feeds = context.Feeds.Where(feed => feed.Enabled && feed.LastUpdate == null || DbFunctions.DiffMinutes(DateTime.Now, feed.LastUpdate.Value) > feed.UpdateInterval).ToArray();

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
                    feedDownloadManager.Enqueue(uri, feed);
                }
            }
        }

        //private static void checkFeeds()
        //{
        //    using (Context context = new Context())
        //    {
        //        var feeds = context.Feeds.Where(feed => feed.Enabled && feed.LastUpdate == null || DbFunctions.DiffMinutes(DateTime.Now, feed.LastUpdate.Value) > feed.UpdateInterval).ToArray();
                
        //        foreach (var entity in feeds)
        //        {
        //            Uri uri;
        //            try
        //            {
        //                uri = new Uri(entity.RssUrl);
        //            }
        //            catch
        //            {
        //                Program.Logger.Log(LogLevels.Warning, String.Format("Feed {0}: Invalid RssUrl '{1}'. Disabling feed!", entity.Id, entity.RssUrl));
                        
        //                entity.Enabled = false;
        //                context.SaveChanges();
        //                continue;
        //            }
                    
        //            var parser = new QDFeedParser.HttpFeedFactory();
        //            var feed = parser.CreateFeed(uri);

        //            var parsedItemGuids = feed.Items.Select(f => getGuid(f)).Distinct().ToArray();
        //            var existingItemGuids = context.FeedItems.Where(i => parsedItemGuids.Contains(i.Guid)).Select(i => i.Guid);
        //            var newItems = feed.Items.Where(i => !existingItemGuids.Contains(getGuid(i)));

        //            foreach (var item in newItems)
        //                downloadManager.Enqueue(new Uri(item.Link), new FeedItem()
        //                {
        //                    FeedId = entity.Id,
        //                    Guid = getGuid(item),
        //                    Url = item.Link
        //                });
        //        }
        //    }
        //}

        static void downloadManager_OnItemDownloaded(Uri uri, FeedItem context, string body)
        {
            Program.Logger.Log(LogLevels.Informative, "Download complete: " + uri);
        }


        private static string getGuid(SyndicationItem item)
        {
            if (!String.IsNullOrEmpty(item.Id))
                return item.Id;

            return item.Links.OrderBy(l=>l.Length).FirstOrDefault().ToString();
        }
    }
}
