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
        public PreparedRecordsBatch()
        {
            this.BatchRecords = new List<dynamic>();
        }
    }
}
