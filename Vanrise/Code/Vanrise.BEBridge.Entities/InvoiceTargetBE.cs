using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class InvoiceTargetBE : ITargetBE
    {
        public InvoiceTargetBE()
        {
            TargetObjects = new Dictionary<string, object>();

        }
        public Dictionary<string, object> TargetObjects { get; set; }
        public object TargetBEId
        {
            get;
            set;
        }

        public object SourceBEId
        {
            get;
            set;
        }
    }
}
