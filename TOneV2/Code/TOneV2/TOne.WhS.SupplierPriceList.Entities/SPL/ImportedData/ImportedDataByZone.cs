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
            this.ImportedCodes = new List<ImportedCode>();
            this.ImportedRates = new List<ImportedRate>();
        }

        public string ZoneName { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }


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
