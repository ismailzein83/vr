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
        public ImportedZone()
        {
            this.ImportedCodes = new List<ImportedCode>();
            this.ImportedRates = new List<ImportedRate>();
        }

        public string ZoneName { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }

        private List<NewZone> _newZones = new List<NewZone>();

        public List<NewZone> NewZones
        { 
            get
            {
                return this._newZones;
            }
        }

        private List<ExistingZone> _existingZones = new List<ExistingZone>();

        public List<ExistingZone> ExistingZones 
        { 
            get
            {
                return this._existingZones;
            }
        }

        public string RecentZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        
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
