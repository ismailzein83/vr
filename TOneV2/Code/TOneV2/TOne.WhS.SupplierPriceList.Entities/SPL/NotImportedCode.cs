using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NotImportedCode : IRuleTarget
    {
        public string Code { get; set; }

        public string ZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public bool HasChanged { get; set; }

        public object Key
        {
            get { return Code; }
        }

        public string TargetType
        {
            get { return "Code"; }
        }
    }
}
