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

        private List<ImportedCode> _importedCodes = new List<ImportedCode>();

        public List<ImportedCode> ImportedCodes
        {
            get
            {
                return this._importedCodes;
            }
        }

        public ImportedZoneService ImportedZoneService { get; set; }

        public ImportedRate ImportedNormalRate { get; set; }

        private Dictionary<int, ImportedRate> _importedOtherRates = new Dictionary<int, ImportedRate>();

        public Dictionary<int, ImportedRate> ImportedOtherRates
        {
            get
            {
                return this._importedOtherRates;
            }
        }


        private List<NotImportedRate> _notImportedOtherRates = new List<NotImportedRate>();

        public List<NotImportedRate> NotImportedOtherRates
        {
            get
            {
                return this._notImportedOtherRates;
            }
        }

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
