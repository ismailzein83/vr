using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SaleEntityZoneRateSource { ProductZone = 0, CustomerZone = 1 }

    public class SaleRateHistoryBySource
    {
        #region Fields
        private Dictionary<SaleEntityZoneRateSource, List<SaleRateHistoryRecord>> _rateHistoryBySource;
        #endregion

        #region Constructors
        public SaleRateHistoryBySource()
        {
            _rateHistoryBySource = new Dictionary<SaleEntityZoneRateSource, List<SaleRateHistoryRecord>>();
        }
        #endregion

        public void AddSaleRateHistoryRange(SaleEntityZoneRateSource rateSource, IEnumerable<SaleRateHistoryRecord> rateHistoryRange)
        {
            List<SaleRateHistoryRecord> rateHistory = _rateHistoryBySource.GetOrCreateItem(rateSource, () => { return new List<SaleRateHistoryRecord>(); });
            rateHistory.AddRange(rateHistoryRange);
        }
        public IEnumerable<SaleRateHistoryRecord> GetSaleRateHistory(SaleEntityZoneRateSource rateSource)
        {
            return _rateHistoryBySource.GetRecord(rateSource);
        }
        public int GetCount()
        {
            return _rateHistoryBySource.Count;
        }
    }
}
