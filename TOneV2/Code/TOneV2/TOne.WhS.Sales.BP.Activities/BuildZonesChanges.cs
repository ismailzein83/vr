using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public sealed class BuildZonesChanges : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<int>> CustomerIds { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);
            IEnumerable<int> customerIds = this.CustomerIds.Get(context);
            int ownerId = this.OwnerId.Get(context);
            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
            IEnumerable<SalePLZoneChange> zoneChanges = null;
            int sellingNumberPlanId;
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                CarrierAccount account = carrierAccountManager.GetCarrierAccount(ownerId);
                sellingNumberPlanId = account.SellingNumberPlanId.Value;
                zoneChanges = this.GetZoneChangesForCustomer(existingZones, ownerId);

            }
            else
            {   
                SellingProductManager sellingNumberPlanManager = new SellingProductManager();
                sellingNumberPlanId = sellingNumberPlanManager.GetSellingNumberPlanId(ownerId).Value;
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                List<RoutingCustomerInfoDetails> routingCustomersInfoDetails = new List<RoutingCustomerInfoDetails>();
                IEnumerable<CarrierAccountInfo> carrierAccounts = customerSellingProductManager.GetCustomerNamesBySellingProductId(ownerId);
                foreach (CarrierAccountInfo carrierAccountInfo in carrierAccounts)
                {
                    routingCustomersInfoDetails.Add(new RoutingCustomerInfoDetails()
                    {
                        CustomerId = carrierAccountInfo.CarrierAccountId,
                        SellingProductId = ownerId
                    });
                }
                SaleRateReadAllNoCache saleRateReadWithNoCache = new SaleRateReadAllNoCache(routingCustomersInfoDetails, DateTime.Today, false);
                SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(saleRateReadWithNoCache);
                zoneChanges = this.GetZoneChangesForSellingProduct(existingZones, rateLocator, routingCustomersInfoDetails);
            }



            NotificationManager notificationManager = new NotificationManager();
            notificationManager.BuildNotifications(sellingNumberPlanId, customerIds, zoneChanges, DateTime.Today, SalePLChangeType.Rate);
           
        }

        #region Private Methods

        private IEnumerable<SalePLZoneChange> GetZoneChangesForCustomer(IEnumerable<ExistingZone> existingZones, int customerId)
        {
            List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();
            List<int> ownerIds = new List<int>();
            ownerIds.Add(customerId);

            if (existingZones != null)
            {
                foreach (ExistingZone existingZone in existingZones)
                {
                    if (existingZone.NewRates.Count > 0 || existingZone.ExistingRates.Any(item => item.ChangedRate != null))
                    {
                        SalePLZoneChange zoneChange = new SalePLZoneChange()
                        {
                            ZoneName = existingZone.Name,
                            CountryId = existingZone.CountryId,
                            HasCodeChange = false,
                            CustomersHavingRateChange = ownerIds
                        };
                        zonesChanges.Add(zoneChange);
                    }
                }
            }

            return zonesChanges;
        }


        private IEnumerable<SalePLZoneChange> GetZoneChangesForSellingProduct(IEnumerable<ExistingZone> existingZones, SaleEntityZoneRateLocator rateLocator,
            IEnumerable<RoutingCustomerInfoDetails> routingCustomersInfoDetails)
        {
            List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();
         
            if (existingZones != null)
            {
                foreach (ExistingZone existingZone in existingZones)
                {
                    if (existingZone.NewRates.Count > 0 || existingZone.ExistingRates.Any(item => item.ChangedRate != null))
                    {
                        List<int> ownerIds = new List<int>();

                        foreach (RoutingCustomerInfoDetails routingCustomerInfoDetails in routingCustomersInfoDetails)
                        {
                           SaleEntityZoneRate zoneRate = rateLocator.GetCustomerZoneRate(routingCustomerInfoDetails.CustomerId, routingCustomerInfoDetails.SellingProductId, existingZone.ZoneId);
                           if (zoneRate != null && zoneRate.Source == SalePriceListOwnerType.SellingProduct)
                               ownerIds.Add(routingCustomerInfoDetails.CustomerId);
                        }

                        SalePLZoneChange zoneChange = new SalePLZoneChange()
                        {
                            ZoneName = existingZone.Name,
                            CountryId = existingZone.CountryId,
                            HasCodeChange = false,
                            CustomersHavingRateChange = ownerIds
                        };
                        zonesChanges.Add(zoneChange);
                    }
                }
            }

            return zonesChanges;
        }


        #endregion
    }
}
