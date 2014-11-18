namespace Shared.Migrations
{
    using Shared.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Shared.Entities.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Shared.Entities.Context";
        }

        protected override void Seed(Shared.Entities.Context context)
        {
            //  This method will be called after migrating to the latest version.
            context.Feeds.AddOrUpdate(p => p.RssUrl,
                            new Feed() { RssUrl = "http://www.theregister.co.uk/headlines.atom", Name = "The Register / UK" },
                            new Feed() { RssUrl = "http://www.huffingtonpost.com/feeds/index.xml", Name = "Huffington Post / US" },
                            new Feed() { RssUrl = "http://www.globalmeatnews.com/feed/view/583698", Name = "Global Meat News" },
                            new Feed() { RssUrl = "http://www.aljazeera.com/Services/Rss/?PostingId=2007731105943979989", Name = "Al Jazeera / US" },
                            new Feed() { RssUrl = "http://www.ipsnews.net/feed/", Name = "Inter Press Service" },
                            new Feed() { RssUrl = "http://www.abc.net.au/news/feed/51120/rss.xml", Name = "ABC News / AU", UpdateInterval = 30 }
                         );
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
