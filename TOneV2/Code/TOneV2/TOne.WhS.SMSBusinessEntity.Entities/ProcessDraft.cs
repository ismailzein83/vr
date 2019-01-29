using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public enum ProcessStatus
    {
        Draft = 0,
        Completed = 1,
        Cancelled = 2
    }
    public enum ProcessEntityType
    {
        Customer = 0,
        Supplier = 1
    }

    public class ProcessDraft
    {
        public int ID { get; set; }

        public string EntityID { get; set; }
        
        public ProcessEntityType ProcessType { get; set; }

        public string Changes { get; set; }

        public ProcessStatus Status { get; set; }
    }

}
