using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Retrieval
{
    /// <summary>
    /// It's good practice to limit requests per host and time. This class takes care of that.
    /// Use <code>enqueue</code> to issue a request, and subscribe to the callbacks for progress
    /// notifications.
    /// </summary>
    /// <typeparam name="T">Type of the user context</typeparam>
    internal class DownloadManager<T>
    {
        #region Properties
        int maxRetryCount = 3;
        int checkIntervalSeconds = 1;
        int requestIdleTime = 60;

        List<ItemDownload<T>> itemsToFetch = new List<ItemDownload<T>>();

        static Dictionary<String, DateTime> lastDomainRequest = new Dictionary<string, DateTime>();

        Timer timer = null;
        #endregion

        #region Interface
        internal delegate void ItemDownloadHandler(Uri uri, T context, byte[] body);
        internal event ItemDownloadHandler OnItemDownloaded;

        internal delegate void ItemFailedHandler(Uri uri, T context, Exception exc);
        internal event ItemFailedHandler OnItemFailed;
        internal event ItemFailedHandler OnItemFailedPermanently;

        internal void Enqueue(Uri uri, T context, int priority = 0)
        {
            lock (itemsToFetch)
                if (itemsToFetch.Where(i => i.Uri.Equals(uri)).Any())
                    return;

            itemsToFetch.Add(new ItemDownload<T>()
            {
                Uri = uri,
                UserContext = context,
                Priority = priority
            });
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Asynchroneously downloads files and triggers a callback once it succeeded.
        /// </summary>
        /// <typeparam name="T">Type of the user context associated with a download</typeparam>
        /// <param name="requestIdleTime">Allow 1 request per this many seconds</param>
        /// <param name="checkIntervalSeconds">Perform check for downloadable items every checkIntervalSeconds seconds</param>
        internal DownloadManager(int requestIdleTime = 60, int maxRetryCount = 3, int checkIntervalSeconds = 1)
        {
            this.requestIdleTime = requestIdleTime;
            this.checkIntervalSeconds = checkIntervalSeconds;
            this.maxRetryCount = maxRetryCount;

            timer = new Timer(callback, null, checkIntervalSeconds * 1000, Timeout.Infinite);
        }

        internal void Dispose()
        {
            if (timer != null)
                try { timer.Dispose(); }
                catch { }
                finally { timer = null; }
        }

        void callback(object state)
        {
            try
            {
                downloadItems();
            }
            catch (Exception exc)
            {
                Program.Logger.Log(Shared.LogLevels.Error, "Uncaught Exception", exc);
            }
            finally
            {
                if (timer != null)
                    timer.Change(checkIntervalSeconds * 1000, Timeout.Infinite);
            }
        }
        #endregion

        private void downloadItems()
        {
            while (true)
            {
                ItemDownload<T> item;
                lock (itemsToFetch)
                    lock (lastDomainRequest)
                        item = itemsToFetch.Where(i => !lastDomainRequest.ContainsKey(i.Uri.Host.ToLowerInvariant()) || (DateTime.UtcNow - lastDomainRequest[i.Uri.Host]).TotalSeconds > requestIdleTime).OrderByDescending(p => p.Priority).FirstOrDefault();

                if (item == null)
                    break;

                if (lastDomainRequest.ContainsKey(item.Uri.Host.ToLowerInvariant()) &&
                    (DateTime.UtcNow - lastDomainRequest[item.Uri.Host.ToLowerInvariant()]).TotalSeconds < requestIdleTime)
                    continue;

                // Try to fetch file
                byte[] data;

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        data = client.DownloadData(item.Uri);
                    }
                }
                catch (Exception exc)
                {
                    item.Retry++;
                    if (item.Retry >= maxRetryCount)
                    {
                        if (OnItemFailedPermanently != null)
                            OnItemFailedPermanently(item.Uri, item.UserContext, exc);

                        Program.Logger.Log(Shared.LogLevels.Notice, "Retry count exceeded for URL, dropping: " + item.Uri.ToString());
                    }
                    else
                        if (OnItemFailed != null)
                            OnItemFailed(item.Uri, item.UserContext, exc);

                    continue;
                }

                // Update host request timestamp
                lock (lastDomainRequest)
                    if (!lastDomainRequest.ContainsKey(item.Uri.Host.ToLowerInvariant()))
                        lastDomainRequest.Add(item.Uri.Host.ToLowerInvariant(), DateTime.UtcNow);
                    else
                        lastDomainRequest[item.Uri.Host.ToLowerInvariant()] = DateTime.UtcNow;

                try
                {
                    if (OnItemDownloaded != null)
                        OnItemDownloaded(item.Uri, item.UserContext, data);
                }
                catch (Exception exc)
                {
                    Program.Logger.Log(Shared.LogLevels.Error, "Uncaught Exception", exc);
                }

                lock (itemsToFetch)
                    itemsToFetch = itemsToFetch.Where(i => i.Retry < maxRetryCount && i.Uri != item.Uri).ToList();
            }
        }
    }
}
