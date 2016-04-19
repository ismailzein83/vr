using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
namespace TOne.WhS.Routing.Business
{
    public class TrafficStatsQualityMeasureManager
    {
        public decimal? GetTrafficStatsQualityMeasure(long supplierZoneId)
        {
            return GetCachedQualityMeasurementBySupplierZone().GetRecord(supplierZoneId);
        }

        public decimal? GetTrafficStatsQualityMeasure(long saleZoneId, int supplierId)
        {
            return GetCachedQualityMeasurementBySaleZoneSupplier().GetRecord(string.Format("{0}_{1}", supplierId, saleZoneId));
        }

        private Dictionary<string, decimal?> GetCachedQualityMeasurementBySaleZoneSupplier()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetQualityMeasurementGroupBySaleZone_Supplier",
               () =>
               {
                   Dictionary<string, decimal?> result = new Dictionary<string, decimal?>();
                   ITrafficStatsMeasureDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ITrafficStatsMeasureDataManager>();
                   List<SaleZoneSupplierTrafficStatsMeasure> items = dataManager.GetQualityMeasurementsGroupBySaleZoneSupplier(new TimeSpan(5, 0, 0));
                   if (items != null && items.Count > 0)
                   {
                       foreach (SaleZoneSupplierTrafficStatsMeasure item in items)
                       {
                           result.Add(string.Format("{0}_{1}", item.SupplierId, item.SaleZoneId), CalculateTQI(item));
                       }
                   }
                   return result;
               });
        }

        private Dictionary<long, decimal?> GetCachedQualityMeasurementBySupplierZone()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetQualityMeasurementGroupBySupplierZone",
               () =>
               {
                   Dictionary<long, decimal?> result = new Dictionary<long, decimal?>();
                   ITrafficStatsMeasureDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ITrafficStatsMeasureDataManager>();
                   List<SupplierZoneTrafficStatsMeasure> items = dataManager.GetQualityMeasurementsGroupBySupplierZone(new TimeSpan(5000, 0, 0));
                   if (items != null && items.Count > 0)
                   {
                       foreach (SupplierZoneTrafficStatsMeasure item in items)
                       {
                           result.Add(item.SupplierZoneId, CalculateTQI(item));
                       }
                   }
                   return result;
               });
        }

        private decimal CalculateTQI(TrafficStatsMeasure item)
        {
            decimal ner = item.TotalNumberOfAttempts != 0 ? item.TotalDeliveredAttempts * 100 / item.TotalNumberOfAttempts : 0;
            decimal acd = item.TotalSuccesfulAttempts != 0 ? (item.TotalDurationInSeconds / 60) / item.TotalSuccesfulAttempts : 0;
            decimal tqi = ner * acd * 100;
            return tqi;
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }
    }
}
