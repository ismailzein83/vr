using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedCountry: IRuleTarget,IExclude

    {
        public int CountryId { get; set; }
        public List<ImportedZone> ImportedZones { get; set; }
        public List<ImportedCode> ImportedCodes { get; set; }
        public List<ImportedRate> ImportedRates { get; set; }
    

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.CountryId; }
        }

        #endregion

        #region IExclude Implementation
        public bool IsExcluded { get; set; }
        public void SetAsExcluded ()
        {
            IsExcluded = true;
            foreach (var importedZone in this.ImportedZones)
            {
                importedZone.SetAsExcluded();
            }
        }

     
        #endregion

        public string TargetType
        {
            get { return "Country"; }
        }

    }
}
