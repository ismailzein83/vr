using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedRate : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget,IExclude
    {
        public ImportedRate()
        {
            this.ProcessInfo = new RateProcessInfo();
        }

        public string ZoneName { get; set; }

        public Decimal Rate { get; set; }

        public SystemRate SystemRate { get; set; }

        public int? CurrencyId { get; set; }

        public int? RateTypeId { get; set; } 
         
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public RateChangeType ChangeType { get; set; }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();
        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }

        public RateProcessInfo ProcessInfo { get; set; }

        #region IExclude Implementation
        public bool IsExcluded { get; set; }
        public void SetAsExcluded()
        {
            IsExcluded = true;


            foreach (var newRate in NewRates)
                newRate.IsExcluded = true;

            foreach (var changedExistingRate in ChangedExistingRates)
                changedExistingRate.ChangedRate.IsExcluded = true;
        }
        #endregion

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.ZoneName; }
        }

        #endregion


        public string TargetType
        {
            get { return "Rate"; }
        }
    }

    public class RateProcessInfo
    {
        public ExistingRate RecentExistingRate { get; set; }
    }

}
