using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using System.Data;
using Vanrise.Queueing;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public class CustomerZoneRateOutput
    {
        public ZoneCustomerRates CustomerZoneRates { get; set; }
    }

    public class CustomerZoneRateInput
    {

        public HashSet<int> ZoneIds { get; set; }

        public int RoutingDatabaseId { get; set; }
    }
    public sealed class GetCustomerZoneRates : BaseAsyncActivity<CustomerZoneRateInput, CustomerZoneRateOutput>
    {

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<int>> ZoneIds { get; set; }

        [RequiredArgument]
        public OutArgument<ZoneCustomerRates> CustomerZoneRates { get; set; }

        protected override CustomerZoneRateOutput DoWorkWithResult(CustomerZoneRateInput inputArgument, AsyncActivityHandle handle)
        {
            ZoneCustomerRates customerRates = new ZoneCustomerRates();
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            customerRates = dataManager.GetZoneCustomerRates(inputArgument.ZoneIds);

            return new CustomerZoneRateOutput()
            {
                CustomerZoneRates = customerRates
            };

        }

        protected override CustomerZoneRateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CustomerZoneRateInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                ZoneIds = this.ZoneIds.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CustomerZoneRateOutput result)
        {
            this.CustomerZoneRates.Set(context, result.CustomerZoneRates);
        }
    }
}
