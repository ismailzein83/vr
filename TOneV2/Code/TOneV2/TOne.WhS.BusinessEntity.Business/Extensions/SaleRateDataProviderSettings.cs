using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    class SaleRateDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId => new Guid("E1D7E364-F78A-45DC-9EE4-19463E05ABA4");

        public override bool DoesSupportFilterOnAllFields => false;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            DateTime effectiveDate = context.FromTime.Value;
            List<RoutingCustomerInfoDetails> customerInfoDetails = GetCustomerInfoDetails();
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, effectiveDate, false));
            SaleEntityZoneRateLocator futureCustomerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(customerInfoDetails, context.FromTime.Value, true));

            var carrierAccountManager = new CarrierAccountManager();
            var saleZoneManager = new SaleZoneManager();
            var saleRatesInfo = new List<SaleRateInfo>();
            foreach (RoutingCustomerInfoDetails customerInfo in customerInfoDetails)
            {
                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerInfo.CustomerId, CarrierAccountType.Customer);
                IEnumerable<SaleZone> customerSaleZones = saleZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, effectiveDate, false);

                foreach (var customerZone in customerSaleZones)
                {
                    SaleEntityZoneRate customerZoneRate = customerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);
                    SaleEntityZoneRate futureCustomerZoneRate = futureCustomerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, customerZone.SaleZoneId);

                    var currentRate = customerZoneRate.Rate;
                    var saleRate = new SaleRateInfo
                    {
                        CustomerId = customerInfo.CustomerId,
                        Rate = currentRate.Rate,
                        ZoneId = customerZone.SaleZoneId
                    };
                    if (futureCustomerZoneRate != null)
                        saleRate.FutureRate = futureCustomerZoneRate.Rate.Rate;
                    if (currentRate.EED.HasValue)
                        saleRate.DaysToEnd = DateTime.Now.Subtract(currentRate.EED.Value).Days;
                    saleRatesInfo.Add(saleRate);
                }

                IEnumerable<SaleZone> futurCustomerSaleZones = saleZoneManager.GetCustomerSaleZones(customerInfo.CustomerId, null, false);

                foreach (var futureCustomerZone in futurCustomerSaleZones)
                {
                    SaleEntityZoneRate futureCustomerZoneRate = futureCustomerZoneRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, futureCustomerZone.SaleZoneId);
                    if (futureCustomerZoneRate == null || futureCustomerZoneRate.Rate == null)
                        continue;

                    var futureSaleRate = new SaleRateInfo
                    {
                        CustomerId = customerInfo.CustomerId,
                        FutureRate = futureCustomerZoneRate.Rate.Rate,
                        ZoneId = futureCustomerZone.SaleZoneId
                    };
                    saleRatesInfo.Add(futureSaleRate);
                }
            }
            foreach (var saleRateInfo in saleRatesInfo)
            {
                context.OnRecordLoaded(DataRecordObjectMapper(saleRateInfo), DateTime.Now);
            }
        }

        #region Private Methods
        private DataRecordObject DataRecordObjectMapper(SaleRateInfo saleRateInfo)
        {
            var saleRateObject = new Dictionary<string, object>
            {
                {"Zone", saleRateInfo.ZoneId},
                {"Customer", saleRateInfo.CustomerId},
                {"CurrentRate", saleRateInfo.Rate},
                {"FutureRate", saleRateInfo.FutureRate},
                {"DaysToEnd", saleRateInfo.DaysToEnd}
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
            public decimal? FutureRate { get; set; }
        }
    }
}
