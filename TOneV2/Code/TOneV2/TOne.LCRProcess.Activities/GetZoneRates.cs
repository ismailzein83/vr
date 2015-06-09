using System.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using TOne.LCR.Data;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetZoneRates : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<int>> SupplierZoneIds { get; set; }
        [RequiredArgument]
        public InArgument<HashSet<int>> CustomerZoneIds { get; set; }
        [RequiredArgument]
        public InArgument<bool> RebuildZoneRates { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierZoneRates> SupplierZoneRates { get; set; }

        [RequiredArgument]
        public OutArgument<ZoneCustomerRates> CustomerZoneRates { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ZoneCustomerRates customerZoneRates = new ZoneCustomerRates();
            SupplierZoneRates supplierZoneRates = new SupplierZoneRates();
            int routingDatabaseId = this.RoutingDatabaseId.Get(context);
            HashSet<int> supplierZoneIds = this.SupplierZoneIds.Get(context);
            HashSet<int> customerZoneIds = this.CustomerZoneIds.Get(context);
            bool rebuildZoneRates = this.RebuildZoneRates.Get(context);
            if (!rebuildZoneRates)
            {
                IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
                dataManager.DatabaseId = this.RoutingDatabaseId.Get(context);
                customerZoneRates = dataManager.GetZoneCustomerRates(customerZoneIds);
                supplierZoneRates = dataManager.GetSupplierZoneRates(supplierZoneIds);
            }
            else
            {
                List<int> allZoneIds = new List<int>();
                allZoneIds.AddRange(supplierZoneIds);
                allZoneIds.AddRange(customerZoneIds);

                List<ZoneRate> customerRates = new List<ZoneRate>();
                List<ZoneRate> supplierRates = new List<ZoneRate>();
                RateManager rateManager = new RateManager();
                rateManager.GetCalculatedZoneRates(DateTime.Now, false, allZoneIds, out customerRates, out supplierRates);
                customerZoneRates = GetZoneCustomerRates(customerRates);
                supplierZoneRates = GetSupplierZoneRates(supplierRates);
            }

            this.SupplierZoneRates.Set(context, supplierZoneRates);
            this.CustomerZoneRates.Set(context, customerZoneRates);
        }

        private SupplierZoneRates GetSupplierZoneRates(List<ZoneRate> zoneRates)
        {
            SupplierZoneRates supplierZoneRates = new SupplierZoneRates();
            supplierZoneRates.RatesByZoneId = new Dictionary<int, RateInfo>();
            List<RateInfo> rateInfos = new List<RateInfo>();
            foreach (ZoneRate zoneRate in zoneRates)
            {
                var rate = new RateInfo
                       {
                           ZoneId = zoneRate.ZoneId,
                           Rate = zoneRate.Rate,
                           ServicesFlag = zoneRate.ServicesFlag,
                           PriceListId = zoneRate.PriceListId
                       };
                if (!supplierZoneRates.RatesByZoneId.ContainsKey(rate.ZoneId))
                {
                    supplierZoneRates.RatesByZoneId.Add(rate.ZoneId, rate);
                    rateInfos.Add(rate);
                }
            }
            return supplierZoneRates;
        }


        private ZoneCustomerRates GetZoneCustomerRates(List<ZoneRate> zoneRates)
        {
            ZoneCustomerRates zoneCustomerRates = new ZoneCustomerRates();
            zoneCustomerRates.ZonesCustomersRates = new Dictionary<int, CustomerRates>();
            foreach (ZoneRate zoneRate in zoneRates)
            {
                var rate = new RateInfo
                       {
                           ZoneId = zoneRate.ZoneId,
                           Rate = zoneRate.Rate,
                           ServicesFlag = zoneRate.ServicesFlag,
                           PriceListId = zoneRate.PriceListId
                       };

                CustomerRates customerRates;
                if (!zoneCustomerRates.ZonesCustomersRates.TryGetValue(rate.ZoneId, out customerRates))
                {
                    customerRates = new CustomerRates();
                    customerRates.CustomersRates = new Dictionary<string, RateInfo>();
                    zoneCustomerRates.ZonesCustomersRates.Add(rate.ZoneId, customerRates);
                }

                if (!customerRates.CustomersRates.ContainsKey(zoneRate.CarrierAccountId))
                    customerRates.CustomersRates.Add(zoneRate.CarrierAccountId, rate);
            }
            return zoneCustomerRates;
        }
    }
}
