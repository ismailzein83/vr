using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class SingleDestinationRoutesBuildContext
    {
        #region ctor/Local Variables

        Dictionary<string, CodeMatch> _suppliersCodeMatches;
        ZoneCustomerRates _customerRates;
        SupplierZoneRates _supplierRates;
        RouteRulesByActionDataType _routeRules;
        SupplierRouteOptionRulesMatches _supplierRules;

        string _routeCode;
        int _saleZoneId;
        CustomerRates _saleZoneCustomerRates;

        public SingleDestinationRoutesBuildContext(Dictionary<string, CodeMatch> suppliersCodeMatches, ZoneCustomerRates customerRates, SupplierZoneRates supplierRates,
            RouteRulesByActionDataType routeRules, SupplierRouteOptionRulesMatches supplierRules)
        {
            _suppliersCodeMatches = suppliersCodeMatches;
            _customerRates = customerRates;
            _supplierRates = supplierRates;
            _routeRules = routeRules;
            _supplierRules = supplierRules;
        }

        #endregion

        #region Private Methods

        void InitializeRoutes()
        {
            CodeMatch sysCodeMatch;
            if (_suppliersCodeMatches.TryGetValue("SYS", out sysCodeMatch))
            {
                _routeCode = sysCodeMatch.Code;
                _saleZoneId = sysCodeMatch.SupplierZoneId;
                _customerRates.ZonesCustomersRates.TryGetValue(sysCodeMatch.SupplierZoneId, out _saleZoneCustomerRates);
            }
        }

        #endregion

        #region Internal Methods

        IEnumerable<RouteSupplierOption> _lcrOptions;
        internal IEnumerable<RouteSupplierOption> GetLCROptions(short servicesFlag)
        {
            if (_lcrOptions == null)
            {
                var lcrOptions = new List<RouteSupplierOption>();
                foreach (CodeMatch cm in _suppliersCodeMatches.Values)
                {
                    if (cm.SupplierId != "SYS")
                    {
                        ZoneRates supplierZoneRates;
                        if (_supplierRates.SuppliersZonesRates.TryGetValue(cm.SupplierId, out supplierZoneRates))
                        {
                            RateInfo rate;
                            if (supplierZoneRates.ZonesRates.TryGetValue(cm.SupplierZoneId, out rate))
                            {
                                RouteSupplierOption option = new RouteSupplierOption
                                {
                                    SupplierId = cm.SupplierId,
                                    SupplierZoneId = cm.SupplierZoneId,
                                    Rate = rate.Rate,
                                    ServicesFlag = rate.ServicesFlag
                                };
                                lcrOptions.Add(option);
                            }
                        }
                    }
                }
                _lcrOptions = lcrOptions.OrderBy(itm => itm.Rate);
            }
            return CloneHelper.Clone<IEnumerable<RouteSupplierOption>>(_lcrOptions.Where(itm => itm.ServicesFlag >= servicesFlag));
        }

        #endregion

        #region Public Methods

        public List<RouteDetail> BuildRoutes()
        {
            InitializeRoutes();
            if (_routeCode == null || _saleZoneId == default(int) || _saleZoneCustomerRates == null || _saleZoneCustomerRates.CustomersRates == null || _saleZoneCustomerRates.CustomersRates.Count == 0)
                return null;

            List<RouteDetail> routes = new List<RouteDetail>();

            Dictionary<Type, RouteRuleMatchFinder> routeRuleFindersByActionDataType = new Dictionary<Type, RouteRuleMatchFinder>();
            if (_routeRules != null)
            {
                foreach (var customerRuleEntry in _routeRules.Rules)
                {
                    routeRuleFindersByActionDataType.Add(customerRuleEntry.Key, new RouteRuleMatchFinder(_routeCode, _saleZoneId, customerRuleEntry.Value));
                }
            }

            foreach (var customerRateEntry in _saleZoneCustomerRates.CustomersRates)
            {
                RouteDetail route = new RouteDetail
                {
                    Code = _routeCode,
                    CustomerID = customerRateEntry.Key,
                    Rate = customerRateEntry.Value.Rate,
                    ServicesFlag = customerRateEntry.Value.ServicesFlag,
                    SaleZoneId = _saleZoneId
                };
                RouteBuildContext routeBuildContext = new RouteBuildContext(route, this, _suppliersCodeMatches, _supplierRates, routeRuleFindersByActionDataType);
                routeBuildContext.BuildRoute();
                routes.Add(route);
            }

            return routes;
        }

        #endregion
    }
}