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
        [Key]
        public int Id { get; set; }
        public int FeedId { get; set; }
        public DateTime Published { get; set; }
        public String Url { get; set; }
        public String Guid { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String Tags { get; set; }

        [ForeignKey("FeedId")]
        public virtual Feed Feed { get; set; }
    }
}
