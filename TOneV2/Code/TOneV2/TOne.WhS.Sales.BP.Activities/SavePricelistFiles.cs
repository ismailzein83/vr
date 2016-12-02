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
            int sellingNumberPlanId;
            List<int> customerIds = new List<int>();
            
            if(ownerType == SalePriceListOwnerType.SellingProduct)
            {
                sellingNumberPlanId = GetSellingProductSellingNumberPlanId(ownerId);
                IEnumerable<CarrierAccountInfo> customersOfSellingProduct = customerSellingProductManager.GetCustomersBySellingProductId(ownerId);
                //TODO: check on nulls here
                customerIds = customersOfSellingProduct.Select(itm => itm.CarrierAccountId).ToList();
            }
            else
            {
                sellingNumberPlanId = GetCustomerSellingNumberPlanId(ownerId);
                customerIds.Add(ownerId);
            }

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

            IEnumerable<CarrierAccountInfo> customersToSavePricelistsFor = this.GetCustomersToSavePriceListsFor(sellingNumberPlanId, salePLZoneChanges);
            CustomersWithPriceListFile.Set(context, customersToSavePricelistsFor);
        }

        #region Private Methods

        private IEnumerable<CarrierAccountInfo> GetCustomersToSavePriceListsFor(int sellingNumberPlanId, IEnumerable<SalePLZoneChange> zonesChanges)
        {
            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = StructureZonesChangesByCountry(zonesChanges);

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccountInfo> customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId);
            DateTime today = DateTime.Today;

            List<CarrierAccountInfo> customersToSavePricelistsFor = new List<CarrierAccountInfo>();
            if (customers != null)
            {
                foreach (CarrierAccountInfo customer in customers)
                {
                    IEnumerable<int> customerCountryIds = customerCountryManager.GetCustomerCountryIds(customer.CarrierAccountId, today, false);

                    if (customerCountryIds != null && customerCountryIds.Intersect(zonesChangesByCountry.Keys).Count() > 0)
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
