using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class FeedItem
    {
        public FeedItem()
        {
            State = States.Discovered;
        }

        public enum States
        {
            Discovered = 0,
            Fetched = 1,
            Failed = 2,
            FailedInvalidHtml = 3
        };

        [Key]
        public int Id { get; set; }

        [Index]
        public int FeedId { get; set; }

        public DateTime Published { get; set; }
        public String Url { get; set; }
        public String Guid { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        
        public String Tags { get; set; }

        public int TagCount { get; set; }

        [Index]
        public States State { get; set; }
        public String Author { get; set; }

        [ForeignKey("FeedId")]
        public virtual Feed Feed { get; set; }
    }
}
