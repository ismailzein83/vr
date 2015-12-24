using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedRate : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
    {
        public ImportedRate()
        {
            this.ProcessInfo = new RateProcessInfo();
        }

        public string ZoneName { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

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

        public bool IsExcluded { get; set; }

        #region IRuleTarget Implementation

        public void SetExcluded()
        {
            this.IsExcluded = true;
        }

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
        public Decimal? RecentRate { get; set; }
    }

    public enum RateChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Increase")]
        Increase = 2,

        [Description("Increase")]
        Decrease = 3
    }
}
