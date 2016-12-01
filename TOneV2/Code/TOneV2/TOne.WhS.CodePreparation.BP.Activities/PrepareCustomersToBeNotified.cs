﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class PrepareCustomersToBeNotified : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> ZonesChanges { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersToBeNotified { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            IEnumerable<SalePLZoneChange> zonesChanges = this.ZonesChanges.Get(context);

            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = StructureZonesChangesByCountry(zonesChanges);

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccountInfo> customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId);
            DateTime today = DateTime.Today;

            List<CarrierAccountInfo> customersToBeNotified = new List<CarrierAccountInfo>();
            if (customers != null)
            {
                foreach (CarrierAccountInfo customer in customers)
                {
                    IEnumerable<int> customerCountryIds = customerCountryManager.GetCustomerCountryIds(customer.CarrierAccountId, today, false);

                    if (customerCountryIds != null && customerCountryIds.Intersect(zonesChangesByCountry.Keys).Count() > 0)
                        customersToBeNotified.Add(customer);
                }
            }

            CustomersToBeNotified.Set(context, customersToBeNotified);
        }

        #region Private Methods
        private Dictionary<int, List<SalePLZoneChange>> StructureZonesChangesByCountry(IEnumerable<SalePLZoneChange> zonesChanges)
        {
            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = new Dictionary<int, List<SalePLZoneChange>>();

            List<SalePLZoneChange> zonesChangesList;

            foreach (SalePLZoneChange zoneChange in zonesChanges)
            {
                if (!zonesChangesByCountry.TryGetValue(zoneChange.CountryId, out zonesChangesList))
                {
                    zonesChangesList = new List<SalePLZoneChange>();
                    zonesChangesList.Add(zoneChange);
                    zonesChangesByCountry.Add(zoneChange.CountryId, zonesChangesList);
                }
                else
                    zonesChangesList.Add(zoneChange);
            }

            return zonesChangesByCountry;
        }
        
        #endregion
    }
}
