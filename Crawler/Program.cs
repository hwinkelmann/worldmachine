using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;
using Crawler.Retrieval;

namespace Crawler
{
    class Program
    {
        internal static Logging Logger = new Logging();

        internal static DownloadManager<String> manager = new DownloadManager<String>();

        static void Main(string[] args)
        {
            manager.OnItemDownloaded += manager_OnItemDownloaded;
            manager.OnItemFailed += manager_OnItemFailed;
            manager.OnItemFailedPermanently += manager_OnItemFailedPermanently;

            manager.Enqueue(new Uri("http://spiegel.de"), "ein spiegel request");

            System.Console.ReadLine();
        }

        static void manager_OnItemFailedPermanently(Uri uri, string context, Exception exc)
        {
            System.Console.WriteLine("Download permanently failed: " + exc.Message);
        }

        static void manager_OnItemFailed(Uri uri, string context, Exception exc)
        {
            System.Console.WriteLine("Download failed: " + exc.Message);
        }

        static void manager_OnItemDownloaded(Uri uri, string context, string body)
        {
            System.Console.WriteLine("Downloaded");
        }

    }
}
