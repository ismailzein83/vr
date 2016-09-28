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
        public ImportedDataByZone()
        {
            ImportedZoneServices = new List<ImportedZoneService>();
        }
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

        private Dictionary<int, List<ImportedRate>> _importedOtherRates = new Dictionary<int, List<ImportedRate>>();

        public Dictionary<int, List<ImportedRate>> ImportedOtherRates
        {
            get
            {
                return this._importedOtherRates;
            }
        }


        private List<ImportedZoneService> _importedZoneServices = new List<ImportedZoneService>();
        public List<ImportedZoneService> ImportedZoneServices
        { get; set;
           
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
