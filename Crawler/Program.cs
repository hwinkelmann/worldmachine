using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            using(Database.ModelContainer entity = new Database.ModelContainer())
            {
                try
                {
                    entity.Database.CreateIfNotExists();
                    Database.Feed feed = new Database.Feed() { Name = "bla", RssUrl = "test" };
                    entity.Feeds.Add(feed);
                    entity.SaveChanges();
                }
                catch( Exception ex)
                {

                }
            }
        }
    }
}
