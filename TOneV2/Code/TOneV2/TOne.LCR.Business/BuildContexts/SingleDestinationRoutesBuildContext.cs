﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class SingleDestinationRoutesBuildContext : IDisposable
    {
        #region ctor/Local Variables

        SingleDestinationCodeMatches _singleDestinationCodeMatches;
        ZoneCustomerRates _customerRates;
        SupplierZoneRates _supplierRates;
        RouteRulesByActionDataType _routeRules;
        RouteOptionRulesBySupplier _routeOptionsRules;
        Dictionary<Type, RouteRuleMatchFinder> _routeRuleFindersByActionDataType;

        int _saleZoneId;
        CustomerRates _saleZoneCustomerRates;

       

        public SingleDestinationRoutesBuildContext(SingleDestinationCodeMatches singleDestinationCodeMatches, ZoneCustomerRates customerRates, SupplierZoneRates supplierRates,
            RouteRulesByActionDataType routeRules, RouteOptionRulesBySupplier routeOptionsRules)
        {
            _singleDestinationCodeMatches = singleDestinationCodeMatches;
            _customerRates = customerRates;
            _supplierRates = supplierRates;
            _routeRules = routeRules;
            _routeOptionsRules = routeOptionsRules;
        }

        #endregion

        #region Public Properties

        public Dictionary<Type, RouteRuleMatchFinder> RouteRuleFindersByActionDataType
        {
            get
            {
                return _routeRuleFindersByActionDataType;
            }
        }

        public RouteOptionRulesBySupplier RouteOptionsRules
        {
            get
            {
                return _routeOptionsRules;
            }
        }

        public CodeMatchesBySupplierId CodeMatchesBySupplierId
        {
            get
            {
                return _singleDestinationCodeMatches.CodeMatchesBySupplierId;
            }
        }

        public SupplierZoneRates SupplierRates
        {
            get
            {
                return _supplierRates;
            }
        }


        #endregion

        #region Private Methods

        void InitializeRoutes()
        {
            CodeMatch sysCodeMatch;
            if (_singleDestinationCodeMatches.CodeMatchesBySupplierId.TryGetValue("SYS", out sysCodeMatch))
            {
                _saleZoneId = sysCodeMatch.SupplierZoneId;
                _customerRates.ZonesCustomersRates.TryGetValue(sysCodeMatch.SupplierZoneId, out _saleZoneCustomerRates);
            }
        }

        #endregion

        #region Internal Methods

        //IEnumerable<RouteSupplierOption> _lcrOptions;
        //internal IEnumerable<RouteSupplierOption> GetLCROptions(short servicesFlag)
        //{
        //    if (_lcrOptions == null)
        //    {
        //        var lcrOptions = new List<RouteSupplierOption>();
        //        foreach (CodeMatch cm in _codeMatchesBySupplierId.Values)
        //        {
        //            if (cm.SupplierId != "SYS")
        //            {
        //                //ZoneRates supplierZoneRates;
        //                //if (_supplierRates.SuppliersZonesRates.TryGetValue(cm.SupplierId, out supplierZoneRates))
        //                //{
        //                    RateInfo rate;
        //                    if (_supplierRates.RatesByZoneId.TryGetValue(cm.SupplierZoneId, out rate))
        //                    {
        //                        RouteSupplierOption option = new RouteSupplierOption
        //                        {
        //                            SupplierId = cm.SupplierId,
        //                            SupplierZoneId = cm.SupplierZoneId,
        //                            Rate = rate.Rate,
        //                            ServicesFlag = rate.ServicesFlag
        //                        };
        //                        lcrOptions.Add(option);
        //                    }
        //                //}
        //            }
        //        }
        //        _lcrOptions = lcrOptions;//.OrderBy(itm => itm.Rate);
        //    }

        //    List<RouteSupplierOption> copy = new List<RouteSupplierOption>();

        //    foreach (var o in _lcrOptions)
        //    {
        //        if(o.ServicesFlag >= servicesFlag)
        //            copy.Add(o.Clone());
        //    }
                    
        //    return copy;
        //}

        //IEnumerator<RateInfo> _supplierRatesEnumerator;
        List<RouteSupplierOption> _retrievedOptions = new List<RouteSupplierOption>();
        int _retrievedOptionNextReturnedIndex;
        int _nextSupplierRateIndex;
        int _ratesCount;
        //CodeMatch[] _codeMatches;

        RouteSupplierOption[] _lcr;

        internal RouteSupplierOption GetNextOptionInLCR()
        {
            if (_lcr == null)
            {
                List<RouteSupplierOption> lstLCR = new List<RouteSupplierOption>();
                if (_supplierRates != null)
                {
                    foreach (var codeMatch in _singleDestinationCodeMatches.CodeMatchesByZoneId.Values)
                    {
                        RateInfo rate;
                        if (_supplierRates.RatesByZoneId.TryGetValue(codeMatch.SupplierZoneId, out rate))
                        {
                            RouteSupplierOption option = new RouteSupplierOption
                            {
                                SupplierId = codeMatch.SupplierId,
                                SupplierZoneId = codeMatch.SupplierZoneId,
                                Rate = rate.Rate,
                                ServicesFlag = rate.ServicesFlag
                            };
                            lstLCR.Add(option);
                        }
                    }
                    _lcr = lstLCR.OrderBy(itm => itm.Rate).ToArray();
                }
            }

            if (_lcr.Length > _retrievedOptionNextReturnedIndex)
            {
                var option = _lcr[_retrievedOptionNextReturnedIndex].Clone();
                _retrievedOptionNextReturnedIndex++;
                return option;
            }
            else
                return null;


            //if (_supplierRates == null || _supplierRates.OrderedRates == null)
            //    return null;
            //if (_retrievedOptions.Count > _retrievedOptionNextReturnedIndex)
            //{
            //    var option = _retrievedOptions[_retrievedOptionNextReturnedIndex].Clone();
            //    _retrievedOptionNextReturnedIndex++;
            //    return option;
            //}

            ////if (_codeMatches == null)
            ////    _codeMatches = _codeMatchesByZoneId.Values.ToArray();

            ////if (_codeMatches.Length > _nextSupplierRateIndex)
            ////{
            ////    CodeMatch codeMatch = _codeMatches[_nextSupplierRateIndex];
            ////    RouteSupplierOption option = new RouteSupplierOption
            ////    {
            ////        SupplierId = codeMatch.SupplierId,
            ////        SupplierZoneId = codeMatch.SupplierZoneId
            ////    };
            ////    _retrievedOptions.Add(option);
            ////    _retrievedOptionNextReturnedIndex++;
            ////    return option.Clone(); ;
            ////}

            //if (_ratesCount == 0)
            //    _ratesCount = _supplierRates.OrderedRates.Length;

            //while (_ratesCount > _nextSupplierRateIndex)
            //{
            //    var supplierZoneRate = _supplierRates.OrderedRates[_nextSupplierRateIndex];
            //    _nextSupplierRateIndex++;
            //    if (_singleDestinationCodeMatches.CodeMatchesByZoneId.ContainsKey(supplierZoneRate.ZoneId))
            //    {
            //        CodeMatch codeMatch = _singleDestinationCodeMatches.CodeMatchesByZoneId[supplierZoneRate.ZoneId];
            //        RouteSupplierOption option = new RouteSupplierOption
            //        {
            //            SupplierId = codeMatch.SupplierId,
            //            SupplierZoneId = codeMatch.SupplierZoneId,
            //            Rate = supplierZoneRate.Rate,
            //            ServicesFlag = supplierZoneRate.ServicesFlag
            //        };
            //        _retrievedOptions.Add(option);
            //        _retrievedOptionNextReturnedIndex++;
            //        return option.Clone();
            //    }
            //}
            //return null;
        }

        #endregion

        #region Public Methods

        public List<RouteDetail> BuildRoutes()
        {
            InitializeRoutes();
            if (_saleZoneId == default(int) || _saleZoneCustomerRates == null || _saleZoneCustomerRates.CustomersRates == null || _saleZoneCustomerRates.CustomersRates.Count == 0)
                return null;

            List<RouteDetail> routes = new List<RouteDetail>();

            _routeRuleFindersByActionDataType = new Dictionary<Type, RouteRuleMatchFinder>();
            if (_routeRules != null)
            {
                foreach (var customerRuleEntry in _routeRules.Rules)
                {
                    _routeRuleFindersByActionDataType.Add(customerRuleEntry.Key, new RouteRuleMatchFinder(_singleDestinationCodeMatches.RouteCode, _saleZoneId, customerRuleEntry.Value));
                }
            }
            //if (_supplierRates != null && _supplierRates.RatesByZoneId != null)
            //    _supplierRatesEnumerator = _supplierRates.OrderedRates.GetEnumerator();
            
            foreach (var customerRateEntry in _saleZoneCustomerRates.CustomersRates)
            {
                RouteDetail route = new RouteDetail
                {
                    Code = _singleDestinationCodeMatches.RouteCode,
                    CustomerID = customerRateEntry.Key,
                    Rate = customerRateEntry.Value.Rate,
                    ServicesFlag = customerRateEntry.Value.ServicesFlag,
                    SaleZoneId = _saleZoneId
                };
                _retrievedOptionNextReturnedIndex = 0;
                using (RouteBuildContext routeBuildContext = new RouteBuildContext(route, this))
                {
                    routeBuildContext.BuildRoute();
                    routes.Add(route);
                }
            }

            return routes;
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
}