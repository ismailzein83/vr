using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mediation.Generic.Entities
{
    public class PreparedRecordsBatch
    {
        public List<dynamic> BatchRecords { get; set; }

        public PreparedRecordsBatchProxy Proxy { get; set; }

        public PreparedRecordsBatch()
        {
            this.BatchRecords = new List<dynamic>();
        }
    }

    public class PreparedRecordsBatchProxy
    {
        public List<string> SessionIdToDelete { get; set; }
        public List<long> EventIdsToDelete { get; set; }
        public long LastCommittedId { get; set; }
        public int NbOfHandlersToExecute { get; set; }
        public int NbOfExecutedHandlers
        {
            get;
            set;
        }
    }
}
