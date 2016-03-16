using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedCountry: IRuleTarget
    {
        public int CountryId { get; set; }

        public List<ImportedCode> ImportedCodes { get; set; }

        public List<ImportedRate> ImportedRates { get; set; }

        public bool IsExcluded { get; set; }

        #region IRuleTarget Implementation

        public void SetExcluded()
        {
            this.IsExcluded = true;
        }

        public object Key
        {
            get { return this.CountryId; }
        }

        #endregion


        public string TargetType
        {
            get { return "Country"; }
        }

    }
}
