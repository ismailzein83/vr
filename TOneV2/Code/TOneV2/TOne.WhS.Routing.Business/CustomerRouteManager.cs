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
        CarrierAccountManager _carrierAccountManager;
        SaleZoneManager _saleZoneManager;
        SupplierZoneManager _supplierZoneManager;

        public CustomerRouteManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _supplierZoneManager = new SupplierZoneManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            ICustomerRouteDataManager manager =  RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            var routingDatabase = routingDatabaseManager.GetRoutingDatabase(input.Query.RoutingDatabaseId);

            if (routingDatabase == null)//in case of deleted database
                routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(input.Query.RoutingDatabaseId);

            if(routingDatabase == null)
                throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId:{0}", input.Query.RoutingDatabaseId));

            var customerRoutingDatabases = routingDatabaseManager.GetRoutingDatabasesReady(routingDatabase.ProcessType, routingDatabase.Type).OrderByDescending(itm => itm.CreatedTime);
            manager.RoutingDatabase = customerRoutingDatabases.First();

            BigResult<CustomerRoute> customerRouteResult = manager.GetFilteredCustomerRoutes(input);

            BigResult<CustomerRouteDetail> customerRouteDetailResult = new BigResult<CustomerRouteDetail>()
            {
                ResultKey = customerRouteResult.ResultKey,
                TotalCount = customerRouteResult.TotalCount,
                Data = customerRouteResult.Data.MapRecords(CustomerRouteDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        private CustomerRouteDetail CustomerRouteDetailMapper(CustomerRoute customerRoute)
        {
            List<CustomerRouteOptionDetail> optionDetails = this.GetRouteOptionDetails(customerRoute);

            return new CustomerRouteDetail()
            {
                Entity = customerRoute,
                CustomerName = _carrierAccountManager.GetCarrierAccountName(customerRoute.CustomerId),
                ZoneName = _saleZoneManager.GetSaleZoneName(customerRoute.SaleZoneId),
                RouteOptionDetails = optionDetails
            };
        }

        private List<CustomerRouteOptionDetail> GetRouteOptionDetails(CustomerRoute customerRoute)
        {
            if (customerRoute.Options == null)
                return null;

            List<CustomerRouteOptionDetail> optionDetails = new List<CustomerRouteOptionDetail>();

            foreach (RouteOption item in customerRoute.Options)
            {
                optionDetails.Add(new CustomerRouteOptionDetail()
                {
                    IsBlocked = item.IsBlocked,
                    Percentage = item.Percentage,
                    SupplierCode = item.SupplierCode,
                    SupplierName = _carrierAccountManager.GetCarrierAccountName(item.SupplierId),
                    SupplierRate = item.SupplierRate,
                    SupplierZoneName = _supplierZoneManager.GetSupplierZoneName(item.SupplierZoneId),
                    ExactSupplierServiceIds = item.ExactSupplierServiceIds.ToList()
                });
            }

            return optionDetails;
        }

        internal void LoadRoutesFromCurrentDB(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            var routingDatabases = routingDatabaseManager.GetRoutingDatabasesReady(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            if(routingDatabases != null && routingDatabases.Count() > 0)
            {
                ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
                dataManager.RoutingDatabase = routingDatabases.OrderByDescending(itm => itm.CreatedTime).First();
                dataManager.LoadRoutes(customerId, codePrefix, onRouteLoaded);
            }
        }
    }
}
