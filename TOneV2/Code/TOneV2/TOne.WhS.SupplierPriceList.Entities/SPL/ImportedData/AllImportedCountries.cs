using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class AllImportedCountries : IRuleTarget, IExclude
    {
        public IEnumerable<ImportedCountry> ImportedCountries { get; set; }
        #region IExclude Implementation
        public bool IsExcluded { get; set; }
        public void SetAsExcluded()
        {
            IsExcluded = true;
            foreach (var importedCountry in this.ImportedCountries)
            {
                importedCountry.SetAsExcluded();
            }
        }


        #endregion

        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllImportedCountries"; }
        }
    }
}
