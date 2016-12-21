using System.Collections.Generic;
using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using Vanrise.Entities;
using System;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class CustomerRouteManager
    {
        public IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(DataRetrievalInput<CustomerRouteQuery> input)
        {
            Dictionary<string, int> customerRouteidDictionary = new Dictionary<string, int>();

            int matchRouteId;
            if (!customerRouteidDictionary.TryGetValue(input.Query.CustomerName, out matchRouteId)) return null;

            string routeTableName = string.Format("rt{0}", matchRouteId);
            var allCustomers = GetCustomerRoutes(routeTableName, input.Query.Top, "Desc", input.Query.CodePrefix);

            return DataRetrievalManager.Instance.ProcessResult(input,
                allCustomers.ToBigResult(input, null, CustomerRouteDetailMapper));
        }

        #region Private Functions

        private Dictionary<int, CustomerRoute> GetCustomerRoutes(string routeTableName, int top, string orderBy, string codePrefix)
        {
            ICustomerRouteDataManager manager = IVSwitchDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            Helper.SetSwitchConfig(manager);
            return manager.GetCustomerRoutes(routeTableName, top, orderBy, codePrefix).ToDictionary(c => c.RouteId, c => c);
        }

        #endregion

        #region Mapper
        private CustomerRouteDetail CustomerRouteDetailMapper(CustomerRoute customerRoute)
        {
            return new CustomerRouteDetail { Entity = customerRoute };
        }
        #endregion
    }
}
