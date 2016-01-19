using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDRDetail
    {
        public CDR Entity { get; set; }
        public string CallClassName { get; set; }
        public string CallTypeName { get; set; }
        public string SubscriberTypeName { get; set; }
    }
}
