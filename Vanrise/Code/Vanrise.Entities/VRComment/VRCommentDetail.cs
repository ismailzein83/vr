using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRCommentDetail
    {
        public long VRCommentId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByDescription { get; set; }
        public string Content { get; set; }       
        public DateTime CreatedTime { get; set; }
    }
}
