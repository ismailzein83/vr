using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

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
                Entity = customerRoute,
                CustomerName = this.GetCustomerName(customerRoute.CustomerId),
                ZoneName = this.GetZoneName(customerRoute.SaleZoneId),
                RouteOptions = GetRouteOptions(customerRoute)
            };
        }

        private string GetCustomerName(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccount customer = manager.GetCarrierAccount(customerId);

            if (customer != null)
                return customer.Name;

            return "Customer Not Found";
        }

        private string GetZoneName(long zoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            SaleZone saleZone = manager.GetSaleZone(zoneId);

            if (saleZone != null)
                return saleZone.Name;

            return "Zone Not Found";
        }

        private string GetRouteOptions(CustomerRoute customerRoute)
        {
            StringBuilder builder = new StringBuilder();
            if(customerRoute.Options != null)
            {
                CarrierAccountManager manager = new CarrierAccountManager();
                foreach (RouteOption item in customerRoute.Options)
                {
                    CarrierAccount supplier = manager.GetCarrierAccount(item.SupplierId);
                    if (supplier != null)
                        builder.AppendFormat(" {0} {1}%,", supplier.Name, item.Percentage);
                    else
                        builder.Append("Supplier Not Found");
                }
            }

            string routeOptions = builder.ToString();
            routeOptions.TrimEnd(',');

            return routeOptions;
        }
    }
}
