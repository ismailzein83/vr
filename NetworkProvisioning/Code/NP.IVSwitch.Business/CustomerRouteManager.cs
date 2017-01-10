using System.Collections.Generic;
using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using Vanrise.Entities;
using System;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class CustomerRouteManager
    {
        public IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(DataRetrievalInput<CustomerRouteQuery> input)
        {
            EndPointManager endPointManager = new EndPointManager();
            List<EndPointInfo> acls = endPointManager.GetAclList(input.Query.CustomerId);
            if (acls == null) return null;
            var allCustomers = GetCustomerRoutes(acls, input.Query.Top, "Desc", input.Query.CodePrefix);

            return DataRetrievalManager.Instance.ProcessResult(input,
                allCustomers.ToBigResult(input, null, CustomerRouteDetailMapper));
        }

        #region Private Functions

        private Dictionary<string, ConvertedCustomerRoute> GetCustomerRoutes(List<EndPointInfo> acls, int top, string orderBy, string codePrefix)
        {
            ICustomerRouteDataManager manager = IVSwitchDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            Helper.SetSwitchConfig(manager);
            Dictionary<string, ConvertedCustomerRoute> convertedRoutes = new Dictionary<string, ConvertedCustomerRoute>();
            RouteManager routeManager = new RouteManager();
            CarrierAccountManager accountManager = new CarrierAccountManager();
            Dictionary<int, int> mappedSupplierRouteIds = routeManager.GetRouteAndSupplierIds();

            var customerRoutes = manager.GetCustomerRoutes(acls, top, orderBy, codePrefix);
            if (customerRoutes.Count == 0) return new Dictionary<string, ConvertedCustomerRoute>();

            var groupedRoutes =
                 customerRoutes.GroupBy(r => r.Destination)
                     .Select(group => new { Code = group.Key, Iems = group.ToList() });

            foreach (var route in groupedRoutes)
            {
                ConvertedCustomerRoute convertedRoute = new ConvertedCustomerRoute
                {
                    DestinationCode = route.Code,
                    Options = new List<CustomerRouteOption>()
                };
                foreach (var options in route.Iems)
                {
                    int supplierId;
                    if (!mappedSupplierRouteIds.TryGetValue(options.RouteId, out supplierId)) continue;
                    CustomerRouteOption customerRouteOption = new CustomerRouteOption
                    {
                        RouteId = options.RouteId,
                        SupplierId = supplierId,
                        Percentage = options.Percentage,
                        Priority = options.Preference,
                        SupplierName = accountManager.GetCarrierAccountName(supplierId)
                    };
                    convertedRoute.Options.Add(customerRouteOption);
                }
                if (!convertedRoutes.ContainsKey(convertedRoute.DestinationCode))
                    convertedRoutes[convertedRoute.DestinationCode] = convertedRoute;
            }
            return convertedRoutes;
        }

        #endregion
        #region Mapper
        private CustomerRouteDetail CustomerRouteDetailMapper(ConvertedCustomerRoute customerRoute)
        {
            return new CustomerRouteDetail
            {
                Entity = customerRoute
            };
        }
        #endregion
    }
}
