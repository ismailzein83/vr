using System.Collections.Generic;
using System.Linq;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetCustomerZoneDetailsInput
    {
        public IEnumerable<CodePrefixSaleCodes> SaleCodes { get; set; }

        public int RoutingDatabase { get; set; }
    }

    public class GetCustomerZoneDetailsOutput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }
    }

    public sealed class GetCustomerZoneDetails : BaseAsyncActivity<GetCustomerZoneDetailsInput, GetCustomerZoneDetailsOutput>
    {
        public InArgument<IEnumerable<CodePrefixSaleCodes>> SaleCodes { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabase { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        protected override GetCustomerZoneDetailsOutput DoWorkWithResult(GetCustomerZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            CustomerZoneDetailByZone customerZoneDetailsByZone = null;
            ICustomerZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabase);

            IEnumerable<CustomerZoneDetail> customerZoneDetails;
            if (inputArgument.SaleCodes != null)
                customerZoneDetails = dataManager.GetFilteredCustomerZoneDetailsByZone(inputArgument.SaleCodes.SelectMany(itm => itm.SaleCodes).Select(itm => itm.ZoneId).Distinct());
            else
                customerZoneDetails = dataManager.GetCustomerZoneDetails();

            if (customerZoneDetails != null)
            {
                customerZoneDetailsByZone = new CustomerZoneDetailByZone();
                foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
                {
                    List<CustomerZoneDetail> zoneDetails;
                    if (!customerZoneDetailsByZone.TryGetValue(customerZoneDetail.SaleZoneId, out zoneDetails))
                    {
                        zoneDetails = new List<CustomerZoneDetail>();
                        customerZoneDetailsByZone.Add(customerZoneDetail.SaleZoneId, zoneDetails);
                    }
                    zoneDetails.Add(customerZoneDetail);
                }
            }

            return new GetCustomerZoneDetailsOutput
            {
                CustomerZoneDetails = customerZoneDetailsByZone
            };
        }

        protected override GetCustomerZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCustomerZoneDetailsInput
            {
                SaleCodes = this.SaleCodes.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCustomerZoneDetailsOutput result)
        {
            this.CustomerZoneDetails.Set(context, result.CustomerZoneDetails);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Customer Zone Details is done", null);
        }
    }
}
