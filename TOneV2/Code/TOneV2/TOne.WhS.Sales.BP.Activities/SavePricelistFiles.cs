using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SavePricelistFiles : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersWithPriceListFile { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int ownerId = OwnerId.Get(context);
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            IEnumerable<SalePLZoneChange> salePLZoneChanges = SalePLZoneChanges.Get(context);

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId;
            IEnumerable<CarrierAccountInfo> customersOfSellingProduct;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                sellingNumberPlanId = GetSellingProductSellingNumberPlanId(ownerId);
                customersOfSellingProduct = customerSellingProductManager.GetCustomersBySellingProductId(ownerId);
            }
            else
            {
                sellingNumberPlanId = GetCustomerSellingNumberPlanId(ownerId);
                CarrierAccount customer = carrierAccountManager.GetCarrierAccount(ownerId);

                customersOfSellingProduct = new List<CarrierAccountInfo>() 
                { 
                    new CarrierAccountInfo()
                    {
                        CarrierAccountId = ownerId,
                        AccountType = customer.AccountType,
                        Name = carrierAccountManager.GetCarrierAccountName(ownerId),
                        SellingNumberPlanId = customer.SellingNumberPlanId
                    }
                };
            }

            IEnumerable<CarrierAccountInfo> customersToSavePricelistsFor = this.GetCustomersToSavePriceListsFor(customersOfSellingProduct, salePLZoneChanges);
            IEnumerable<int> customerIds = customersToSavePricelistsFor.Select(item => item.CarrierAccountId);

            var salePricelistFileContext = new SalePricelistFileContext
            {
                SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                CustomerIds = customerIds,
                ZoneChanges = salePLZoneChanges,
                EffectiveDate = DateTime.Today,
                ChangeType = SalePLChangeType.Rate,
            };


            SalePriceListManager salePricelistManager = new SalePriceListManager();
            salePricelistManager.SavePricelistFiles(salePricelistFileContext);

            CustomersWithPriceListFile.Set(context, customersToSavePricelistsFor);
        }

        #region Private Methods

        private IEnumerable<CarrierAccountInfo> GetCustomersToSavePriceListsFor(IEnumerable<CarrierAccountInfo> customers, IEnumerable<SalePLZoneChange> zonesChanges)
        {
            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = StructureZonesChangesByCountry(zonesChanges);
            HashSet<int> customerIdsHavingRateChange = new HashSet<int>(zonesChanges.SelectMany(itm => itm.CustomersHavingRateChange));

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();

            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();

            DateTime today = DateTime.Today;

            List<CarrierAccountInfo> customersToSavePricelistsFor = new List<CarrierAccountInfo>();
            if (customers != null)
            {
                foreach (CarrierAccountInfo customer in customers)
                {
                    IEnumerable<int> customerCountryIds = customerCountryManager.GetCustomerCountryIds(customer.CarrierAccountId, today, false);

                    if (customerCountryIds != null && customerCountryIds.Intersect(zonesChangesByCountry.Keys).Count() > 0 && customerIdsHavingRateChange.Contains(customer.CarrierAccountId))
                        customersToSavePricelistsFor.Add(customer);
                }
            }

            return customersToSavePricelistsFor;
        }

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

        private int GetSellingProductSellingNumberPlanId(int sellingProductId)
        {
            var sellingProductManager = new SellingProductManager();
            int? sellingNumberPlanId = sellingProductManager.GetSellingNumberPlanId(sellingProductId);
            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException(string.Format("SellingProduct '{0}' was not found", sellingProductId));
            return sellingNumberPlanId.Value;
        }

        private int GetCustomerSellingNumberPlanId(int customerId)
        {
            var carrierAccountManager = new CarrierAccountManager();
            CarrierAccount customerAccount = carrierAccountManager.GetCarrierAccount(customerId);
            if (customerAccount == null)
                throw new NullReferenceException(string.Format("Customer '{0}' was not found", customerId));
            if (customerAccount.AccountType == CarrierAccountType.Supplier)
                throw new Exception(string.Format("CarrierAccount '{0}' is not a Customer", customerId));
            return customerAccount.SellingNumberPlanId.Value;
        }


        #endregion
    }
}
