using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class OrderedZoneRateHistories
    {
        #region Fields
        private List<IEnumerable<SaleRateHistoryRecord>> _orderedZoneRateHistories;
        #endregion

        #region Constructors
        public OrderedZoneRateHistories()
        {
            _orderedZoneRateHistories = new List<IEnumerable<SaleRateHistoryRecord>>();
        }
        #endregion

        public void AddRateHistory(IEnumerable<SaleRateHistoryRecord> zoneRateHistoryOfSource)
        {
            _orderedZoneRateHistories.Add(zoneRateHistoryOfSource);
        }
        public IEnumerable<SaleRateHistoryRecord> GetZoneRateHistory(int index)
        {
            return _orderedZoneRateHistories.ElementAt(index);
        }
        public int GetCount()
        {
            return _orderedZoneRateHistories.Count;
        }
    }
}
