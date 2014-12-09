namespace Shared.Migrations
{
    using Newtonsoft.Json;
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
                            new Feed() { RssUrl = "http://www.theregister.co.uk/headlines.atom", Name = "The Register / UK", ParserId = 1 },
                            new Feed() { RssUrl = "http://www.vice.com/rss", Name = "The Vice", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//*[@class=\"topics\"]/a", XpathContent = "//div[contains(@class, \"article-content\")]/p[not(@class)]" }) },
                            new Feed() { RssUrl = "http://feeds.wired.com/wired/index", Name = "Wired - Top Stories / US", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//div[@class=\"entry-tags\"]/a", XpathContent = "//span[@itemprop=\"articleBody\"]/p" }) },
                            new Feed() { RssUrl = "http://feeds.feedburner.com/IeeeSpectrum?format=xml", Name = "IEEE Spectrum / US", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//section[@class=\"learn-more\"]/span/a", XpathContent = "//div[@class=\"entry-content\"]/p" }) },
                            new Feed() { RssUrl = "http://www.huffingtonpost.com/feeds/index.xml", Name = "Huffington Post / US", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//div[@class=\"follow bottom-tags\"]/span[@class=\"group\"]/span", XpathContent = "//div[@id=\"mainentrycontent\"]/p" }) },
                            new Feed() { RssUrl = "http://www.globalmeatnews.com/feed/view/583698", Name = "Global Meat News", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//p[@class=\"topics\"]/*", XpathContent = "//div[@class=\"story\"]/p" }) },
                            new Feed() { RssUrl = "http://www.aljazeera.com/Services/Rss/?PostingId=2007731105943979989", Name = "Al Jazeera / US", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//table[@class=\"OCalaisBox\"]//li/a", XpathContent = "//td[@id=\"ctl00_cphBody_tdTextContent\"]/p[text()]" }) },
                            new Feed() { RssUrl = "http://www.ipsnews.net/feed/", Name = "Inter Press Service", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//div[@id=\"rel_tags\"]/ul/li/a", XpathContent = "//div[contains(@id, \"post-\")]/p" }) },
                            new Feed() { RssUrl = "http://www.abc.net.au/news/feed/51120/rss.xml", Name = "ABC News / AU", UpdateInterval = 30, ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//*[@class=\"topics\"]/a", XpathContent = "//div[contains(@class,\"article\")]/p[not(@class)]" }) },
                            new Feed() { RssUrl = "http://www.salon.com/feed/rss/", Name = "Salon.com / US", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//*[@class=\\\"topics\\\"]/a", XpathContent = "//div[contains(@title, 'Page ')]//p" }) },
                            new Feed() { RssUrl = "http://www.spectator.co.uk/feed/", Name = "The Spectator (UK)", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//span[@class=\"all-taxonomies\"]/a", XpathContent = "//div[@class=\"article-body\"]/p" }) },
                            new Feed() { RssUrl = "http://www.business-standard.com/rss/management-columns-10705.rss", Name = "Business Standard (IN)", ParserConfiguration = "{\"XpathTags\":\"//div[@class=\\\"article bdrBNone mT15\\\"]/div[@class=\\\"readmore_tagBG fLt\\\"]/a\",\"XpathContent\":\"//p[@itemprop=\\\"articleBody\\\"]\"}" },
                            new Feed() { RssUrl = "http://feeds.mashable.com/Mashable", Name = "Mashable", ParserConfiguration = JsonConvert.SerializeObject(new { XpathTags = "//footer[@class=\"article-topics\"]/a", XpathContent = "//section[@class=\"article-content\"]/*" }) }
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
