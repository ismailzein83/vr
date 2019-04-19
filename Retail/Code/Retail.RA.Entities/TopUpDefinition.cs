using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class TopUpDefinition
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TopUpDefinitionSettings Settings { get; set; }
    }

    public class TopUpDefinitionSettings
    {
        public string SourceID { get; set; }
        public long OperatorID { get; set; }
        public bool DoesCreditExpire { get; set; }
        public decimal Amount { get; set; }
        public int DuePeriod { get; set; }
    }
}
