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
        public int SupplierId { get; set; }
        public int CurrencyId { get; set; }
    }

    #endregion
    public class ProcessRates : BaseAsyncActivity<ProcessRatesInput>
    {
       public InOutArgument<List<Zone>> Zones { get; set; }
       public InArgument<DateTime?> MinimumDate { get; set; }
       public InArgument<int> SupplierId { get; set; }
       public InArgument<int> CurrencyId { get; set; }
       protected override void DoWork(ProcessRatesInput inputArgument, AsyncActivityHandle handle)
       {
           DateTime startPreparing = DateTime.Now;
           SupplierRateManager manager = new SupplierRateManager();
           List<SupplierRate> existingRates = manager.GetSupplierRatesEffectiveAfter(inputArgument.SupplierId,(DateTime)inputArgument.MinimumDate);
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
                    PriceListId=rate.PriceListId,
                    CurrencyID = inputArgument.CurrencyId
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
                       PriceListId = rate.PriceListId,
                       CurrencyID = inputArgument.CurrencyId
                   });
               }
           }


           foreach (Zone zone in  inputArgument.Zones)
           {
               if (zone.Rates == null || zone.Rates.Count==0){
                    zone.Rates=new List<Rate>();
               }
               if(zone.Status==TOne.WhS.SupplierPriceList.Entities.Status.New)
               {
                   zone.Rates.Add(new Rate
                   {
                       BeginEffectiveDate = zone.BeginEffectiveDate,
                       EndEffectiveDate = zone.EndEffectiveDate,
                       NormalRate = zone.NewRate.Rate,
                       ZoneId = zone.SupplierZoneId,
                       Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                       CurrencyID = inputArgument.CurrencyId
                   });
               }
               else if (zone.Status == TOne.WhS.SupplierPriceList.Entities.Status.NotChanged)
               {
                     List<Rate> matchedRates;
                     ratesByZone.TryGetValue(zone.SupplierZoneId,out matchedRates);
                     foreach (Rate matchedRate in matchedRates)
                        {
                            Rate rate = new Rate
                            {
                                ZoneId = matchedRate.ZoneId,
                                SupplierRateId = matchedRate.SupplierRateId,
                                PriceListId = matchedRate.PriceListId,
                                CurrencyID = matchedRate.CurrencyID
                            };

                            if (zone.NewRate.Rate == matchedRate.NormalRate && zone.BeginEffectiveDate == matchedRate.BeginEffectiveDate && zone.EndEffectiveDate == matchedRate.EndEffectiveDate)
                            {
                                rate.Status = TOne.WhS.SupplierPriceList.Entities.Status.NotChanged;
                                rate.NormalRate = matchedRate.NormalRate;
                                rate.BeginEffectiveDate = matchedRate.BeginEffectiveDate;
                                rate.EndEffectiveDate = matchedRate.EndEffectiveDate;
                                rate.CurrencyID = matchedRate.CurrencyID;
                                zone.Rates.Add(rate);
                            }
                            else if (zone.NewRate.Rate == matchedRate.NormalRate)
                             {
                                 

                                 if (matchedRate.BeginEffectiveDate == zone.BeginEffectiveDate && (matchedRate.EndEffectiveDate < zone.EndEffectiveDate || matchedRate.EndEffectiveDate > zone.EndEffectiveDate || matchedRate.EndEffectiveDate==null))
                                    {
                                        rate.Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated;
                                        rate.NormalRate = matchedRate.NormalRate;
                                        rate.BeginEffectiveDate = matchedRate.BeginEffectiveDate;
                                        rate.EndEffectiveDate = zone.EndEffectiveDate;
                                        rate.CurrencyID = matchedRate.CurrencyID;
                                        zone.Rates.Add(rate);
                                    }
                                 else if (matchedRate.BeginEffectiveDate < zone.BeginEffectiveDate)
                                 {
                                     if (matchedRate.EndEffectiveDate < zone.BeginEffectiveDate)
                                     {
                                         rate.Status = TOne.WhS.SupplierPriceList.Entities.Status.New;
                                         rate.NormalRate = zone.NewRate.Rate;
                                         rate.BeginEffectiveDate = zone.BeginEffectiveDate;
                                         rate.EndEffectiveDate = zone.EndEffectiveDate;
                                         rate.CurrencyID = inputArgument.CurrencyId;
                                         zone.Rates.Add(rate);
                                     }
                                     else if (matchedRate.EndEffectiveDate > zone.BeginEffectiveDate && matchedRate.EndEffectiveDate < zone.EndEffectiveDate)
                                     {
                                         rate.Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated;
                                         rate.NormalRate = matchedRate.NormalRate;
                                         rate.BeginEffectiveDate = matchedRate.BeginEffectiveDate;
                                         rate.EndEffectiveDate = zone.EndEffectiveDate;
                                         rate.CurrencyID = matchedRate.CurrencyID;
                                         zone.Rates.Add(rate);

                                         Rate newRate = new Rate
                                         {
                                             ZoneId = matchedRate.ZoneId,
                                             SupplierRateId = matchedRate.SupplierRateId,
                                             PriceListId = matchedRate.PriceListId,
                                             Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                             NormalRate = matchedRate.NormalRate,
                                             BeginEffectiveDate = zone.BeginEffectiveDate,
                                             EndEffectiveDate = zone.EndEffectiveDate,
                                             CurrencyID = matchedRate.CurrencyID
                                         };
                                         zone.Rates.Add(rate);
                                     }
                                     else if (matchedRate.EndEffectiveDate > zone.EndEffectiveDate || matchedRate.EndEffectiveDate==null)
                                     {
                                         rate.Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated;
                                         rate.NormalRate = matchedRate.NormalRate;
                                         rate.BeginEffectiveDate = matchedRate.BeginEffectiveDate;
                                         rate.EndEffectiveDate = zone.EndEffectiveDate;
                                         rate.CurrencyID = matchedRate.CurrencyID;
                                         zone.Rates.Add(rate);
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
                       CurrencyID = inputArgument.CurrencyId
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
               SupplierId = this.SupplierId.Get(context),
               MinimumDate = this.MinimumDate.Get(context),
               CurrencyId=this.CurrencyId.Get(context)
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
