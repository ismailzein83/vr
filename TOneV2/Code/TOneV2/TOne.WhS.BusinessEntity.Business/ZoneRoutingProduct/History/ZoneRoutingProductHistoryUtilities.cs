using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneRoutingProductHistoryUtilities
    {
        #region GetZoneRoutingProductHistory
        public static IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetZoneRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, IEnumerable<SaleEntityZoneRoutingProductSource> orderedRoutingProductSources)
        {
            if (routingProductHistoryBySource.GetCount() == 0)
                return null;

            OrderedRoutingProductHistories orderedRoutingProductHistories = GetOrderedRoutingProductHistories(routingProductHistoryBySource, orderedRoutingProductSources);

            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> tList;
            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> qList;
            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> rList = orderedRoutingProductHistories.GetRoutingProductHistory(0);

            for (int i = 0; (i + 1) < orderedRoutingProductHistories.GetCount(); i++)
            {
                tList = rList;
                qList = orderedRoutingProductHistories.GetRoutingProductHistory(i + 1);

                List<SaleEntityZoneRoutingProductHistoryRecord> tAsList = (tList != null) ? tList.ToList() : null;
                List<SaleEntityZoneRoutingProductHistoryRecord> qAsList = (qList != null) ? qList.ToList() : null;

                rList = Utilities.MergeUnionWithQForce(tAsList, qAsList, RoutingProductHistoryRecordMapperAction, RoutingProductHistoryRecordMapperAction);
            }

            return rList;
        }
        private static OrderedRoutingProductHistories GetOrderedRoutingProductHistories(RoutingProductHistoryBySource routingProductHistoryBySource, IEnumerable<SaleEntityZoneRoutingProductSource> orderedRoutingProductSources)
        {
            var orderedRoutingProductHistories = new OrderedRoutingProductHistories();

            foreach (SaleEntityZoneRoutingProductSource routingProductSource in orderedRoutingProductSources)
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> routingProductHistory = routingProductHistoryBySource.GetRoutingProductHistory(routingProductSource);
                if (routingProductHistory != null)
                    orderedRoutingProductHistories.AddRoutingProductHistory(routingProductHistory);
            }

            return orderedRoutingProductHistories;
        }
        #endregion

        #region Mappers
        public static Action<DefaultRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> DefaultRoutingProductMapperAction = (defaultRoutingProduct, routingProductHistoryRecord) =>
        {
            routingProductHistoryRecord.RoutingProductId = defaultRoutingProduct.RoutingProductId;
            routingProductHistoryRecord.SaleZoneId = null;
            routingProductHistoryRecord.Source = (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductDefault : SaleEntityZoneRoutingProductSource.CustomerDefault;
            routingProductHistoryRecord.SaleEntityId = defaultRoutingProduct.OwnerId;
        };
        public static Action<SaleZoneRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> ZoneRoutingProductMapperAction = (zoneRoutingProduct, routingProductHistoryRecord) =>
        {
            routingProductHistoryRecord.RoutingProductId = zoneRoutingProduct.RoutingProductId;
            routingProductHistoryRecord.SaleZoneId = zoneRoutingProduct.SaleZoneId;
            routingProductHistoryRecord.Source = (zoneRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductZone : SaleEntityZoneRoutingProductSource.CustomerZone;
            routingProductHistoryRecord.SaleEntityId = zoneRoutingProduct.OwnerId;
        };
        public static Func<DefaultRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> DefaultRoutingProductMapperFunc = (defaultRoutingProduct) =>
        {
            return new SaleEntityZoneRoutingProductHistoryRecord()
            {
                RoutingProductId = defaultRoutingProduct.RoutingProductId,
                SaleZoneId = null,
                Source = (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductDefault : SaleEntityZoneRoutingProductSource.CustomerDefault,
                SaleEntityId = defaultRoutingProduct.OwnerId,
                BED = defaultRoutingProduct.BED,
                EED = defaultRoutingProduct.EED
            };
        };
        public static Func<SaleZoneRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> ZoneRoutingProductMapperFunc = (zoneRoutingProduct) =>
        {
            return new SaleEntityZoneRoutingProductHistoryRecord()
            {
                RoutingProductId = zoneRoutingProduct.RoutingProductId,
                SaleZoneId = zoneRoutingProduct.SaleZoneId,
                Source = (zoneRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductZone : SaleEntityZoneRoutingProductSource.CustomerZone,
                SaleEntityId = zoneRoutingProduct.OwnerId,
                BED = zoneRoutingProduct.BED,
                EED = zoneRoutingProduct.EED
            };
        };
        private static Action<SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecord> RoutingProductHistoryRecordMapperAction = (routingProductHistoryRecord, targetRoutingProductHistoryRecord) =>
        {
            targetRoutingProductHistoryRecord.RoutingProductId = routingProductHistoryRecord.RoutingProductId;
            targetRoutingProductHistoryRecord.SaleZoneId = routingProductHistoryRecord.SaleZoneId;
            targetRoutingProductHistoryRecord.Source = routingProductHistoryRecord.Source;
            targetRoutingProductHistoryRecord.SaleEntityId = routingProductHistoryRecord.SaleEntityId;
        };
        #endregion

        #region Private Classes
        private class OrderedRoutingProductHistories
        {
            #region Field
            private List<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> _orderedRoutingProductHistories;
            #endregion

            #region Constructors
            public OrderedRoutingProductHistories()
            {
                _orderedRoutingProductHistories = new List<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();
            }
            #endregion

            public void AddRoutingProductHistory(IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> routingProductHistory)
            {
                _orderedRoutingProductHistories.Add(routingProductHistory);
            }
            public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetRoutingProductHistory(int index)
            {
                return _orderedRoutingProductHistories.ElementAt(index);
            }
            public int GetCount()
            {
                return _orderedRoutingProductHistories.Count;
            }
        }
        #endregion
    }
}
