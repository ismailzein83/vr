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
        public IDataRetrievalResult<CustomerRouteOption> GetFilteredCustomerRouteOptions(DataRetrievalInput<CustomerRouteOptionQuery> input)
        {
            EndPointManager endPointManager = new EndPointManager();
            List<EndPointInfo> acls = endPointManager.GetAclList(input.Query.CustomerId);
            if (acls == null) return null;
            Dictionary<int, CustomerRouteOption> allCustomersOptions =
                GetCustomerRouteOption(acls, input.Query.Code).ToDictionary(it => it.RouteId, it => it);

            return DataRetrievalManager.Instance.ProcessResult(input, allCustomersOptions.ToBigResult(input, null));
        }

        #region Private Functions
        private List<CustomerRouteOption> GetCustomerRouteOption(List<EndPointInfo> acls, string code)
        {
            ICustomerRouteDataManager manager = IVSwitchDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            Helper.SetSwitchConfig(manager);
            string destinationQuery = string.Format("where destination = '{0}'", code);
            string orderByQuery = "ORDER BY preference";
            var customerRoutes = manager.GetCustomerRouteOptions(acls, string.Empty, orderByQuery, destinationQuery);
            List<CustomerRouteOption> options =
                customerRoutes.Select(GetConvertedOptions).Where(convertedOption => convertedOption != null).ToList();
            return options;
        }
        private Dictionary<string, ConvertedCustomerRoute> GetCustomerRoutes(List<EndPointInfo> acls, int top, string orderBy, string codePrefix)
        {
            ICustomerRouteDataManager manager = IVSwitchDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            Helper.SetSwitchConfig(manager);
            Dictionary<string, ConvertedCustomerRoute> convertedRoutes = new Dictionary<string, ConvertedCustomerRoute>();

            string destinationQuery = (!string.IsNullOrEmpty(codePrefix))
                ? string.Format("where destination like '{0}%'", codePrefix)
                : string.Empty;
            string topQuery = string.Format("limit {0}", top);
            string orderByQuery = string.Format(" order by destination {0} ", orderBy);
            var customerRoutes = manager.GetCustomerRoutes(acls, topQuery, orderByQuery, destinationQuery);

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
                    CustomerRouteOption customerRouteOption = GetConvertedOptions(options);
                    if (customerRouteOption != null) convertedRoute.Options.Add(customerRouteOption);
                }
                if (!convertedRoutes.ContainsKey(convertedRoute.DestinationCode))
                    convertedRoutes[convertedRoute.DestinationCode] = convertedRoute;
            }
            return convertedRoutes;
        }
        private CustomerRouteOption GetConvertedOptions(CustomerRoute route)
        {
            RouteManager routeManager = new RouteManager();
            CarrierAccountManager accountManager = new CarrierAccountManager();
            Dictionary<int, int> mappedSupplierRouteIds = routeManager.GetRouteAndSupplierIds();
            int supplierId;
            if (!mappedSupplierRouteIds.TryGetValue(route.RouteId, out supplierId)) return null;
            return new CustomerRouteOption
            {
                RouteId = route.RouteId,
                SupplierId = supplierId,
                SupplierName = accountManager.GetCarrierAccountName(supplierId),
                Percentage = route.Percentage,
                Priority = route.Preference
            };
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
