using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedDataByZone : IRuleTarget
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


        private List<ImportedRate> _importedNormalRates = new List<ImportedRate>();

        public List<ImportedRate> ImportedNormalRates
        {
            get
            {
                return this._importedNormalRates;
            }
        }

        private List<ImportedRate> _importedOtherRates = new List<ImportedRate>();

        public List<ImportedRate> ImportedOtherRates
        {
            get
            {
                return this._importedOtherRates;
            }
        }


        private List<ImportedZoneService> _importedZonesServices = new List<ImportedZoneService>();
        public List<ImportedZoneService> ImportedZonesServices
        {
            get
            {
                return this._importedZonesServices;
            }
        }

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
