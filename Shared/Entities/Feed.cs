using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Feed
    {
        public Feed()
        {
            UpdateInterval = 60;
            FeedItem = new HashSet<FeedItem>();
            Enabled = true;
            ParserId = 0;
        }

        public Feed(float UpdateInterval = 60)
        {
            this.UpdateInterval = UpdateInterval;
            FeedItem = new HashSet<FeedItem>();
            Enabled = true;
        }
        
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        public String RssUrl { get; set; }
        public float UpdateInterval { get; set; }
        public DateTime? LastUpdate {get; set; }

        public virtual ICollection<FeedItem> FeedItem { get; set; }

        public bool Enabled { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public int ParserId { get; set; }

        public String ParserConfiguration { get; set; }
    }
}
