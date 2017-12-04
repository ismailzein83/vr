using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductHistoryBySource
    {
        #region Fields
        private Dictionary<SaleEntityZoneRoutingProductSource, List<SaleEntityZoneRoutingProductHistoryRecord>> _routingProductHistoryBySource;
        #endregion

        #region Constructors
        public RoutingProductHistoryBySource()
        {
            _routingProductHistoryBySource = new Dictionary<SaleEntityZoneRoutingProductSource, List<SaleEntityZoneRoutingProductHistoryRecord>>();
        }
        #endregion

        public void AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource routingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> routingProductHistoryRange)
        {
            List<SaleEntityZoneRoutingProductHistoryRecord> routingProductHistory = _routingProductHistoryBySource.GetOrCreateItem(routingProductSource, () =>
            {
                return new List<SaleEntityZoneRoutingProductHistoryRecord>();
            });
            routingProductHistory.AddRange(routingProductHistoryRange);
        }
        public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetRoutingProductHistory(SaleEntityZoneRoutingProductSource routingProductSource)
        {
            return _routingProductHistoryBySource.GetRecord(routingProductSource);
        }
        public int GetCount()
        {
            return _routingProductHistoryBySource.Count;
        }
    }
}
