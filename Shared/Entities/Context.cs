using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Context : DbContext
    {
        public Context() : base() { }

        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
