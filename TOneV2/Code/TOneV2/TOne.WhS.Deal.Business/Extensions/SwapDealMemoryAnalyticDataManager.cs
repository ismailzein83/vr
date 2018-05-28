using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> records = new List<RawMemoryRecord>();
            List<int> filteredDealIds = null;
            if(query.Filters != null)
            {
                var dealDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Deal");
                if (dealDimensionFilter != null)
                    filteredDealIds = dealDimensionFilter.FilterValues.Select(itm => Convert.ToInt32(itm)).ToList();
            }
            var swapDealManager = new SPDealManager();
            var filteredDeals = swapDealManager.GetAllSwapDeals().FindAllRecords(deal => (filteredDealIds == null || filteredDealIds.Contains(deal.DealId))
                && Utilities.AreTimePeriodsOverlapped(deal.Settings.BeginDate, deal.Settings.EndDate, query.FromTime, query.ToTime));
            bool estimationPerZone = query.DimensionFields.Contains("Zone") || query.DimensionFields.Contains("ZoneGroup")
                || (query.Filters != null && (query.Filters.Any(filter => filter.Dimension == "Zone" || filter.Dimension == "ZoneGroup")));
            
            if(filteredDeals != null && filteredDeals.Count() > 0)
            {
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
                foreach(var deal in filteredDeals)
                {                    
                    if(!deal.Settings.EndDate.HasValue)
                        throw new NullReferenceException(String.Format("deal.Settings.EndDate '{0}'", deal.DealId));
                    int nbOfDealDays = (int)(deal.Settings.EndDate.Value - deal.Settings.BeginDate).TotalDays;
                    List<DateTime> dealDays = new List<DateTime>();

                    DateTime queryToDate = query.ToTime.HasValue ? query.ToTime.Value.Date.AddDays(1) : DateTime.Today.AddDays(1);
                    for (DateTime d = Utilities.Max(deal.Settings.BeginDate, query.FromTime.Date); d < Utilities.Min(deal.Settings.EndDate.Value, queryToDate); d = d.AddDays(1))
                    {
                        dealDays.Add(d);
                    }
                    SwapDealSettings swapDealSettings = deal.Settings.CastWithValidate<SwapDealSettings>("deal.Settings", deal.DealId);
                    if (swapDealSettings.Inbounds != null)
                    {
                        int totalVolume = 0;
                        foreach (var inbound in swapDealSettings.Inbounds)
                        {
                            totalVolume += inbound.Volume;
                        }
                        Decimal dailyTotalVolume = (decimal)totalVolume / nbOfDealDays;
                        foreach (var inbound in swapDealSettings.Inbounds)
                        {
                            Decimal dailyGroupVolume = (decimal)inbound.Volume / nbOfDealDays;
                            inbound.SaleZones.ThrowIfNull("inbound.SaleZones", deal.DealId);
                            foreach (var saleZone in inbound.SaleZones)
                            {
                                string saleZoneName = saleZoneManager.GetSaleZoneName(saleZone.ZoneId);
                                saleZoneName.ThrowIfNull("saleZoneName", saleZone.ZoneId);

                                foreach (var day in dealDays)
                                {
                                    RawMemoryRecord record = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
                                    record.FieldValues.Add("Day", day);
                                    record.FieldValues.Add("Deal", deal.DealId);
                                    record.FieldValues.Add("IsSale", true);
                                    record.FieldValues.Add("ZoneGroup", inbound.Name);
                                    record.FieldValues.Add("Zone", saleZoneName);
                                    record.FieldValues.Add("DailyEstimatedVolume", estimationPerZone ? dailyGroupVolume : dailyTotalVolume);
                                    records.Add(record);
                                }
                            }
                        }
                    }
                    if (swapDealSettings.Outbounds != null)
                    {
                        int totalVolume = 0;
                        foreach (var outbound in swapDealSettings.Outbounds)
                        {
                            totalVolume += outbound.Volume;
                        }
                        Decimal dailyTotalVolume = (decimal)totalVolume / nbOfDealDays;
                        foreach (var outbound in swapDealSettings.Outbounds)
                        {
                            Decimal dailyGroupVolume = (decimal)outbound.Volume / nbOfDealDays;
                            outbound.SupplierZones.ThrowIfNull("outbound.SupplierZones", deal.DealId);
                            foreach (var supplierZone in outbound.SupplierZones)
                            {
                                string supplierZoneName = supplierZoneManager.GetSupplierZoneName(supplierZone.ZoneId);
                                supplierZoneName.ThrowIfNull("supplierZoneName", supplierZone.ZoneId);

                                foreach (var day in dealDays)
                                {
                                    RawMemoryRecord record = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
                                    record.FieldValues.Add("Day", day);
                                    record.FieldValues.Add("Deal", deal.DealId);
                                    record.FieldValues.Add("IsSale", false);
                                    record.FieldValues.Add("ZoneGroup", outbound.Name);
                                    record.FieldValues.Add("Zone", supplierZoneName);
                                    record.FieldValues.Add("DailyEstimatedVolume", estimationPerZone ? dailyGroupVolume : dailyTotalVolume);
                                    records.Add(record);
                                }
                            }
                        }
                    }
                }
            }

            return records;
        }

        private class SPDealManager : SwapDealManager
        {
            public IEnumerable<DealDefinition> GetAllSwapDeals()
            {
                return GetCachedSwapDeals();
            }
        }
    }
}
