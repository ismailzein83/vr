using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerRequestHeaderDetail
    {
        public Guid CustomerRequestId { get; set; }

        public Guid RequestTypeId { get; set; }

        public CustomerObjectType CustomerObjectType { get; set; }

        public Guid AccountOrContactId { get; set; }

        public string Title { get; set; }

        public CustomerRequestStatus Status { get; set; }

        public long SequenceNumber { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}
