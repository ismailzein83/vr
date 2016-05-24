using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedZone : IRuleTarget
    {
        public string ZoneName { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }

        public List<NewZone> NewZones { get; set; }

        public List<ExistingZone> ExistingZones { get; set; }

        public string RecentZoneName { get; set; }

        public DateTime BED { get; set; }
        
        public ZoneChangeType ChangeType { get; set; }

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.ZoneName; }
        }

        #endregion


        public string TargetType
        {
            get { return "Zone"; }
        }
    }

}
