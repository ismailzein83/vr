using System;
using System.Collections.Generic;
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

        public bool IsExcluded { get; set; }


        #region IRuleTarget Implementation

        public string Message { get; set; }

        public void SetExcluded()
        {
            this.IsExcluded = true;

            foreach (ImportedCode code in this.ImportedCodes)
            {
                code.SetExcluded();
            }

            foreach (ImportedRate rate in ImportedRates)
            {
                rate.SetExcluded();
            }
        }

        public object Key
        {
            get { return this.ZoneName; }
        }

        #endregion
    }

}
