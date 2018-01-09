using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class SOMRequestDetail
    {
        public long SOMRequestId { get; set; }

        public string EntityId { get; set; }

        public string Title { get; set; }

        public long? ProcessInstanceId { get; set; }

        public Guid RequestTypeId { get; set; }

        public SOMRequestStatus Status { get; set; }

        public string StatusDescription { get; set; }

        public string WaitingInfo { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
