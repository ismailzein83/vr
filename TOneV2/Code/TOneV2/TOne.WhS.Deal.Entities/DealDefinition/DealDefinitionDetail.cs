using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealDefinitionDetail 
    {
        public DealDefinition Entity { set; get; }

        public string CarrierAccountName { get; set; }

        public bool IsEffective { get; set; }

        public string CurrencySymbole { get; set; }

        public string TypeDescription { get; set; }
        public string StatusDescription { get; set; }
    }
}
