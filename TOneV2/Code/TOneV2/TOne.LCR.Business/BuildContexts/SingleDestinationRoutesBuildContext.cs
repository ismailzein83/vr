using System;
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
        RouteRulesByActionDataType _routeRules;
        RouteOptionRulesBySupplier _routeOptionsRules;
        Dictionary<Type, RouteRuleMatchFinder> _routeRuleFindersByActionDataType;

        int _saleZoneId;
        CustomerRates _saleZoneCustomerRates;

       

        public SingleDestinationRoutesBuildContext(SingleDestinationCodeMatches singleDestinationCodeMatches, ZoneCustomerRates customerRates,
            RouteRulesByActionDataType routeRules, RouteOptionRulesBySupplier routeOptionsRules)
        {
            _singleDestinationCodeMatches = singleDestinationCodeMatches;
            _customerRates = customerRates;
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

        #endregion

        #region Private Methods

        void InitializeRoutes()
        {
            CodeMatch sysCodeMatch = _singleDestinationCodeMatches.SysCodeMatch;
            _saleZoneId = sysCodeMatch.SupplierZoneId;
            _customerRates.ZonesCustomersRates.TryGetValue(sysCodeMatch.SupplierZoneId, out _saleZoneCustomerRates);
        }

        #endregion

        #region Internal Methods

        List<RouteSupplierOption> _retrievedOptions = new List<RouteSupplierOption>();
        int _retrievedOptionNextReturnedIndex;
        int _nextSupplierRateIndex;
        int _ratesCount;
        //CodeMatch[] _codeMatches;

        #region Private Classes

        public class RouteSupplierOptionComparer : IComparer<RouteSupplierOption>
        {
            public int Compare(RouteSupplierOption x, RouteSupplierOption y)
            {
                return Decimal.Compare(x.Rate, y.Rate);
            }
        }


        #endregion

        static RouteSupplierOptionComparer s_comparer = new RouteSupplierOptionComparer();
        
        RouteSupplierOption[] _orderedLCR;
        //RouteSupplierOption[] _lcr;
        int _optionsCount;
        IEnumerator<CodeMatch> _orderedCodeMatchEnumerator;
        //Dictionary<int, CodeMatch> _codeMatches;

        internal RouteSupplierOption GetNextOptionInLCR()
        {
           if(_orderedLCR == null)
           {
               _orderedCodeMatchEnumerator = _singleDestinationCodeMatches.OrderedCodeMatches.GetEnumerator();
               _optionsCount = _singleDestinationCodeMatches.OrderedCodeMatches.Count;
               _orderedLCR = new RouteSupplierOption[_optionsCount];
           }

            if (_optionsCount > _retrievedOptionNextReturnedIndex)
            {
                RouteSupplierOption nextOption = _orderedLCR[_retrievedOptionNextReturnedIndex]; 
                if(nextOption == null)
                {
                    _orderedCodeMatchEnumerator.MoveNext();
                    CodeMatch minRateCodeMatch = _orderedCodeMatchEnumerator.Current;                    
                    var optionInfo = new OptionInfo
                    {
                        SupplierId = minRateCodeMatch.SupplierId,
                        SupplierZoneId = minRateCodeMatch.SupplierZoneId,
                        Rate = minRateCodeMatch.SupplierRate.Rate,
                        ServicesFlag = minRateCodeMatch.SupplierRate.ServicesFlag
                    };
                    nextOption = new RouteSupplierOption(optionInfo);
                    _orderedLCR[_retrievedOptionNextReturnedIndex] = nextOption;
                }
                
                _retrievedOptionNextReturnedIndex++;
                if ((nextOption.ServicesFlag & _currentRoute.ServicesFlag) == _currentRoute.ServicesFlag)
                    return new RouteSupplierOption(nextOption.Info);
                else
                    return GetNextOptionInLCR();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        RouteDetail _currentRoute;
        public List<RouteDetail> BuildRoutes()
        {
            InitializeRoutes();
            if (_saleZoneId == default(int) || _saleZoneCustomerRates == null || _saleZoneCustomerRates.CustomersRates == null || _saleZoneCustomerRates.CustomersRates.Count == 0)
                return null;

            List<RouteDetail> routes = new List<RouteDetail>();

            _routeRuleFindersByActionDataType = new Dictionary<Type, RouteRuleMatchFinder>();
            if (_routeRules != null)
            {
                foreach (var customerRuleEntry in _routeRules)
                {
                    _routeRuleFindersByActionDataType.Add(customerRuleEntry.Key, new RouteRuleMatchFinder(_singleDestinationCodeMatches.RouteCode, _saleZoneId, customerRuleEntry.Value));
                }
            }
            
            foreach (var customerRateEntry in _saleZoneCustomerRates.CustomersRates)
            {
                _currentRoute = new RouteDetail
                {
                    Code = _singleDestinationCodeMatches.RouteCode,
                    CustomerID = customerRateEntry.Key,
                    Rate = customerRateEntry.Value.Rate,
                    ServicesFlag = customerRateEntry.Value.ServicesFlag,
                    SaleZoneId = _saleZoneId
                };
                _retrievedOptionNextReturnedIndex = 0;
                using (RouteBuildContext routeBuildContext = new RouteBuildContext(_currentRoute, this))
                {
                    routeBuildContext.BuildRoute();
                    routes.Add(_currentRoute);
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