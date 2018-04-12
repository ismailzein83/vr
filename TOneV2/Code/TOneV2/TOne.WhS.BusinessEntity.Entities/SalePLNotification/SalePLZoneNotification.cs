using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePLZoneNotification : IBaseRates
    {
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string Increment { get; set; }

        private List<SalePLCodeNotification> _codes = new List<SalePLCodeNotification>();
        public List<SalePLCodeNotification> Codes { get { return this._codes; } }
        public SalePLRateNotification Rate { get; set; }

        public Dictionary<int, SalePLOtherRateNotification> OtherRateByRateTypeId { get; set; }

        #region IBaseRates Implementation

        public void SetNormalRateBED(DateTime beginEffectiveDate)
        {
            if (Rate == null)
                return;
            Rate.BED = beginEffectiveDate;
        }

        public void SetOtherRateBED(int rateTypeId, DateTime beginEffectiveDate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
