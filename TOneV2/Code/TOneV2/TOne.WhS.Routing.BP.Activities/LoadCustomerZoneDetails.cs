using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public class LoadCustomerZoneDetailsInput
    {
        public IEnumerable<SaleCode> SaleCodes { get; set; }
    }

    public class LoadCustomerZoneDetailsOutput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }
    }

    public sealed class LoadCustomerZoneDetails : BaseAsyncActivity<LoadCustomerZoneDetailsInput, LoadCustomerZoneDetailsOutput>
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SaleCode>> SaleCodes { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        protected override LoadCustomerZoneDetailsOutput DoWorkWithResult(LoadCustomerZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            CustomerZoneDetailByZone customerZoneDetails = new CustomerZoneDetailByZone();
            
            foreach (SaleCode code in inputArgument.SaleCodes)
            {
                if (!customerZoneDetails.ContainsKey(code.ZoneId))
                {
                    CustomerZoneDetail zoneDetail1 = new CustomerZoneDetail()
                    {
                        CustomerId = 1,
                        SaleZoneId = code.ZoneId,
                        SellingProductId = 1
                    };

                    CustomerZoneDetail zoneDetail2 = new CustomerZoneDetail()
                    {
                        CustomerId = 2,
                        SaleZoneId = code.ZoneId,
                        SellingProductId = 1
                    };

                    List<CustomerZoneDetail> zoneDetails = new List<CustomerZoneDetail>();
                    zoneDetails.Add(zoneDetail1);
                    zoneDetails.Add(zoneDetail2);

                    customerZoneDetails.Add(code.ZoneId, zoneDetails);
                }
            }

            return new LoadCustomerZoneDetailsOutput
            {
                CustomerZoneDetails = customerZoneDetails
            };
        }

        protected override LoadCustomerZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCustomerZoneDetailsInput
            {
                SaleCodes = this.SaleCodes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCustomerZoneDetailsOutput result)
        {
            this.CustomerZoneDetails.Set(context, result.CustomerZoneDetails);
        }
    }
}
