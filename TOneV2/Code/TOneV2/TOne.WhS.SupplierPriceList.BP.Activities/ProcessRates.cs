using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    #region Arguments Classes
    public class ProcessRatesInput
    {
        public List<Zone> Zones { get; set; }
        public DateTime? MinimumDate { get; set; }
    }

    #endregion
    public class ProcessRates : BaseAsyncActivity<ProcessRatesInput>
    {
       public InOutArgument<List<Zone>> Zones { get; set; }
       public InArgument<DateTime?> MinimumDate { get; set; }
       protected override void DoWork(ProcessRatesInput inputArgument, AsyncActivityHandle handle)
       {
           DateTime startPreparing = DateTime.Now;
           SupplierRateManager manager = new SupplierRateManager();
           List<SupplierRate> existingRates = manager.GetSupplierRates((DateTime)inputArgument.MinimumDate);
           RatesByZone ratesByZone = new RatesByZone();
           //convert existingrates to dictionary
           foreach (SupplierRate rate in existingRates)
           {
               List<Rate> rates=null;
               if (!ratesByZone.TryGetValue(rate.ZoneId,out rates)){
                   rates=new List<Rate>();
                   rates.Add(new Rate{
                    BeginEffectiveDate=  rate.BeginEffectiveDate,
                    EndEffectiveDate=rate.EndEffectiveDate,
                    NormalRate=rate.NormalRate,
                    SupplierRateId=rate.SupplierRateId,
                    ZoneId=rate.ZoneId,
                    PriceListId=rate.PriceListId
                   });
                   ratesByZone.Add(rate.ZoneId, rates);
               }else
               {
                   rates.Add(new Rate
                   {
                       BeginEffectiveDate = rate.BeginEffectiveDate,
                       EndEffectiveDate = rate.EndEffectiveDate,
                       NormalRate = rate.NormalRate,
                       SupplierRateId = rate.SupplierRateId,
                       ZoneId = rate.ZoneId,
                       PriceListId = rate.PriceListId
                   });
                   ratesByZone.Add(rate.ZoneId, rates);
               }
           }


           foreach (Zone zone in  inputArgument.Zones)
           {
               if(zone.Status==TOne.WhS.SupplierPriceList.Entities.Status.New)
               {
                   zone.Rates.Add(new Rate
                   {
                       BeginEffectiveDate = zone.BeginEffectiveDate,
                       EndEffectiveDate = zone.EndEffectiveDate,
                       NormalRate = zone.NewRate.Rate,
                       ZoneId = zone.SupplierZoneId
                   });
               }
               else if (zone.Status == TOne.WhS.SupplierPriceList.Entities.Status.NotChanged)
               {
                     List<Rate> matchedRates=null;
                     ratesByZone.TryGetValue(zone.SupplierZoneId,out matchedRates);

                     foreach (Rate matchedRate in matchedRates)
                        {
                            if (zone.NewRate.Rate == matchedRate.NormalRate && zone.BeginEffectiveDate == matchedRate.BeginEffectiveDate && zone.EndEffectiveDate == matchedRate.EndEffectiveDate)
                            {
                                zone.Rates.Add(new Rate{
                                    ZoneId=matchedRate.ZoneId,
                                    SupplierRateId=matchedRate.SupplierRateId,
                                    Status=TOne.WhS.SupplierPriceList.Entities.Status.NotChanged,
                                    NormalRate=matchedRate.NormalRate,
                                    BeginEffectiveDate=matchedRate.BeginEffectiveDate,
                                    EndEffectiveDate=matchedRate.EndEffectiveDate,
                                    Parent=zone,
                                    PriceListId=matchedRate.PriceListId
                                });
                            }
                            else if (zone.NewRate.Rate == matchedRate.NormalRate)
                             {
                                 if (matchedRate.BeginEffectiveDate == zone.BeginEffectiveDate && (matchedRate.EndEffectiveDate < zone.EndEffectiveDate || matchedRate.EndEffectiveDate > zone.EndEffectiveDate || matchedRate.EndEffectiveDate==null))
                                    {
                                         zone.Rates.Add(new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             SupplierRateId = matchedRate.SupplierRateId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                             NormalRate = matchedRate.NormalRate,
                                             BeginEffectiveDate = matchedRate.BeginEffectiveDate,
                                             EndEffectiveDate = zone.EndEffectiveDate,
                                             Parent = zone,
                                             PriceListId = matchedRate.PriceListId
                                         });
                                    }
                                 else if (matchedRate.BeginEffectiveDate < zone.BeginEffectiveDate)
                                 {
                                     if (matchedRate.EndEffectiveDate < zone.BeginEffectiveDate)
                                     {
                                         zone.Rates.Add(new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                             NormalRate = zone.NewRate.Rate,
                                             BeginEffectiveDate = zone.BeginEffectiveDate,
                                             EndEffectiveDate = zone.EndEffectiveDate,
                                             Parent = zone,
                                             PriceListId = matchedRate.PriceListId
                                         });
                                     }
                                     else if (matchedRate.EndEffectiveDate > zone.BeginEffectiveDate && matchedRate.EndEffectiveDate < zone.EndEffectiveDate)
                                     {
                                         zone.Rates.Add(new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             SupplierRateId = matchedRate.SupplierRateId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                             NormalRate = matchedRate.NormalRate,
                                             BeginEffectiveDate = matchedRate.BeginEffectiveDate,
                                             EndEffectiveDate = zone.BeginEffectiveDate,
                                             Parent = zone,
                                             PriceListId = matchedRate.PriceListId
                                         });
                                         zone.Rates.Add(new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                             NormalRate = matchedRate.NormalRate,
                                             BeginEffectiveDate = zone.BeginEffectiveDate,
                                             EndEffectiveDate = zone.EndEffectiveDate,
                                             Parent = zone,
                                             PriceListId = matchedRate.PriceListId
                                         });
                                     }
                                     else if (matchedRate.EndEffectiveDate > zone.EndEffectiveDate || matchedRate.EndEffectiveDate==null)
                                     {
                                         zone.Rates.Add(new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             SupplierRateId = matchedRate.SupplierRateId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                             NormalRate = matchedRate.NormalRate,
                                             BeginEffectiveDate = matchedRate.BeginEffectiveDate,
                                             EndEffectiveDate = zone.EndEffectiveDate,
                                             Parent = zone,
                                             PriceListId = matchedRate.PriceListId
                                         });
                                     }
                                 }
                              }


                   }
               }

               if (zone.Rates.Count == 0)
               {
                   zone.Rates.Add(new Rate
                   {
                       ZoneId = zone.SupplierZoneId,
                       Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                       NormalRate = zone.NewRate.Rate,
                       BeginEffectiveDate = zone.BeginEffectiveDate,
                       EndEffectiveDate = zone.EndEffectiveDate,
                       Parent = zone,
                   });
               }
           }

           TimeSpan spent = DateTime.Now.Subtract(startPreparing);
           handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "ProcessRates done and takes:{0}", spent);
       }

       protected override ProcessRatesInput GetInputArgument(AsyncCodeActivityContext context)
       {
           return new ProcessRatesInput
           {
               Zones = this.Zones.Get(context),
               MinimumDate = this.MinimumDate.Get(context)
           };
       }
       protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
       {
           if (this.Zones.Get(context) == null)
               this.Zones.Set(context, new List<Zone>());

           base.OnBeforeExecute(context, handle);
       }
    }
}
