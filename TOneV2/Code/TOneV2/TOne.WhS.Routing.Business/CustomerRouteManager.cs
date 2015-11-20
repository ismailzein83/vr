using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            ICustomerRouteDataManager manager =  RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            manager.DatabaseId = input.Query.RoutingDatabaseId;

            BigResult<CustomerRoute> customerRouteResult = manager.GetFilteredCustomerRoutes(input);

            BigResult<CustomerRouteDetail> customerRouteDetailResult = new BigResult<CustomerRouteDetail>()
            {
                ResultKey = customerRouteResult.ResultKey,
                TotalCount = customerRouteResult.TotalCount,
                Data = customerRouteResult.Data.MapRecords(customerRouteDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        private CustomerRouteDetail customerRouteDetailMapper(CustomerRoute customerRoute)
        {
            return new CustomerRouteDetail()
            {
                Entity = customerRoute
            };
        }
    }
}
