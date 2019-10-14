using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    class SaleRateDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId => new Guid("E1D7E364-F78A-45DC-9EE4-19463E05ABA4");

        public override bool DoesSupportFilterOnAllFields => false;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            DateTime effectiveDate = DateTime.Today.Date;
            List<RoutingCustomerInfoDetails> customerInfoDetails = GetCustomerInfoDetails();
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveDate, false));
            SaleEntityZoneRateLocator futureCustomerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveDate, true));

            var saleZoneManager = new SaleZoneManager();
            var saleRatesInfo = new List<SaleRateInfo>();
            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                int customerId = customerInfo.CustomerId;
                int sellingProductId = customerInfo.SellingProductId;
                IEnumerable<SaleZone> customerSaleZones = saleZoneManager.GetCustomerSaleZones(customerId, effectiveDate, true);

                if (customerSaleZones == null)
                    continue;

                foreach (var customerZone in customerSaleZones)
                {
                    long saleZoneId = customerZone.SaleZoneId;
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerId, sellingProductId, saleZoneId);
                    if (customerZoneRate == null)
                    {
                        //zone might be open in the future
                        SaleEntityZoneRate futureCustomerZoneRate = futureCustomerZoneRateLocator.GetCustomerZoneRate(customerId, sellingProductId, saleZoneId);

                        if (futureCustomerZoneRate == null || futureCustomerZoneRate.Rate == null)
                            throw new VRBusinessException($"Zone {customerZone.Name} does not have any effective rates on customer {customerId}");

                        var futureSaleRate = new SaleRateInfo
                        {
                            CustomerId = customerId,
                            ZoneId = saleZoneId,
                            PendingRate = futureCustomerZoneRate.Rate.Rate,
                            PendingRateBED = futureCustomerZoneRate.Rate.BED
                        };
                        saleRatesInfo.Add(futureSaleRate);
                    }

                    var currentRate = customerZoneRate.Rate;
                    var saleRate = new SaleRateInfo
                    {
                        CustomerId = customerId,
                        Rate = currentRate.Rate,
                        ZoneId = saleZoneId,
                        CurrentRateBED = currentRate.BED
                    };

                    SaleRate pendingRate = GetPendingRate(customerId, sellingProductId, saleZoneId, customerZoneRate, futureCustomerZoneRateLocator);

                    if (pendingRate != null)
                    {
                        if (!currentRate.EED.HasValue)
                            throw new VRBusinessException($"Zone {customerZone.Name} has multiple effective rates on customer {customerId}");

                        saleRate.PendingRate = pendingRate.Rate;
                        saleRate.DaysToEnd = currentRate.EED.Value.Subtract(DateTime.Now).Days;
                        saleRate.CurrentRateBED = pendingRate.BED;
                    }
                    saleRatesInfo.Add(saleRate);
                }
            }
            foreach (var saleRateInfo in saleRatesInfo)
            {
                context.OnRecordLoaded(DataRecordObjectMapper(saleRateInfo), DateTime.Now);
            }
        }

        #region Private Methods
        private SaleRate GetPendingRate(int customerId, int sellingProductId, long zoneId, SaleEntityZoneRate CurrentCustomerZoneRate, SaleEntityZoneRateLocator futureCustomerZoneRateLocator)
        {
            SaleEntityZoneRate futureCustomerZoneRate = futureCustomerZoneRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);

            if (futureCustomerZoneRate == null)
                return null;

            if (futureCustomerZoneRate.Rate.Rate == CurrentCustomerZoneRate.Rate.Rate)
                return null;

            return futureCustomerZoneRate.Rate;
        }

        private DataRecordObject DataRecordObjectMapper(SaleRateInfo saleRateInfo)
        {
            var saleRateObject = new Dictionary<string, object>
            {
                {"Zone", saleRateInfo.ZoneId},
                {"Customer", saleRateInfo.CustomerId},
                {"CurrentRate", saleRateInfo.Rate},
                {"PendingRate", saleRateInfo.PendingRate},
                {"DaysToEnd", saleRateInfo.DaysToEnd},
                {"CurrentRateBED",saleRateInfo.CurrentRateBED },
                {"PendingRateBED",saleRateInfo.PendingRateBED }
            };
            return new DataRecordObject(new Guid("d03b09aa-4af2-44cd-b9c7-3cf51a567218"), saleRateObject);
        }
        private List<RoutingCustomerInfoDetails> GetCustomerInfoDetails()
        {
            var carrierAccountManager = new CarrierAccountManager();
            var customers = carrierAccountManager.GetAllCustomers();

            var customerInfos = new List<RoutingCustomerInfoDetails>();
            foreach (var customer in customers)
            {
                if (carrierAccountManager.IsCarrierAccountActive(customer))
                    customerInfos.Add(new RoutingCustomerInfoDetails
                    {
                        CustomerId = customer.CarrierAccountId,
                        SellingProductId = customer.SellingProductId.Value
                    });
            }
            return customerInfos;
        }
        #endregion
        private class SaleRateInfo
        {
            public long ZoneId { get; set; }
            public int CustomerId { get; set; }
            public int? DaysToEnd { get; set; }
            public decimal Rate { get; set; }
            public decimal? PendingRate { get; set; }
            public DateTime CurrentRateBED { get; set; }
            public DateTime? PendingRateBED { get; set; }
        }
    }
}
