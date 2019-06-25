using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class RequestHeaderDetail
    {
        public Guid RequestId { get; set; }

        public string status { get; set; }

        public string step { get; set; }

        public string technicalStep { get; set; }

        public string contractId { get; set; }

        public string RequestTypeName { get; set; }

        public string EntityName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string SequenceNumber { get; set; }
    }
}
