using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{

    public class AllImportedZones : IRuleTarget
    {
        public IEnumerable<ImportedZone> Zones { get; set; }

        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllImportedZones"; }
        }
    }
}
