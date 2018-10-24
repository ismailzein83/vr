using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Entities;
using TOne.WhS.RouteSync.Huawei.Business;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei
{
    public partial class HuaweiSWSync : SwitchRouteSynchronizer
    {
        int _supplierRNLength = 3;

        public override Guid ConfigId { get { return new Guid("376687E2-268D-4DFA-AA39-3205C3CD18E5"); } }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public List<HuaweiSSHCommunication> SwitchCommunicationList { get; set; }

        public List<SwitchLogger> SwitchLoggerList { get; set; }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            WhSRouteSyncHuaweiManager whSRouteSyncHuaweiManager = new WhSRouteSyncHuaweiManager();
            whSRouteSyncHuaweiManager.Initialize(context.SwitchId);

            RouteCaseManager routeCaseManager = new RouteCaseManager();
            routeCaseManager.Initialize(context.SwitchId);

            RouteManager routeManager = new RouteManager();
            routeManager.Initialize(context.SwitchId);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || context.Routes.Count == 0 || CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            List<HuaweiConvertedRoute> convertedRoutes = new List<HuaweiConvertedRoute>();
            Dictionary<string, RouteCase> routeCases = new RouteCaseManager().GetCachedRouteCases(context.SwitchId);
            HashSet<string> routeCasesToAdd = new HashSet<string>();

            foreach (var route in context.Routes)
            {
                var customerCarrierMapping = CarrierMappings.GetRecord(route.CustomerId);
                if (customerCarrierMapping == null)
                    continue;

                var customerMapping = customerCarrierMapping.CustomerMapping;
                if (customerMapping == null)
                    continue;

                StringBuilder sb_RouteCase = new StringBuilder(customerMapping.RSSN.ToString());
                int numberOfOptions = 0;

                foreach (var option in route.Options)
                {
                    var supplierCarrierMapping = CarrierMappings.GetRecord(option.SupplierId);
                    if (supplierCarrierMapping == null)
                        continue;

                    var supplierMapping = supplierCarrierMapping.SupplierMapping;
                    if (supplierMapping == null)
                        continue;

                    numberOfOptions++;
                    sb_RouteCase.Append(string.Concat("_", supplierMapping.RouteName));

                    if (numberOfOptions > this.NumberOfOptions)
                        break;
                }

                HuaweiConvertedRoute huaweiConvertedRoute = new HuaweiConvertedRoute()
                {
                    CustomerId = route.CustomerId,
                    Code = route.Code
                };

                if (numberOfOptions > 0)
                {
                    string routeCaseAsString = sb_RouteCase.ToString();

                    huaweiConvertedRoute.RouteCaseAsString = routeCaseAsString;
                }
                else
                {

                }

            }
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            throw new NotImplementedException();
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}