using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class AllImportedDataByZone : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public IEnumerable<ImportedDataByZone> ImportedDataByZoneList { get; set; }
        public object Key
        {
            get { return default(object); }
        }
        public string TargetType
        {
            get { return "Pricelist"; }
        }
    }
}
