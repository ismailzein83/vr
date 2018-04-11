using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Common;
using System.Linq;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class EricssonSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("94739CBC-00A7-4CEB-9285-B4CB35D7D003"); } }
        public int NumberOfOptions { get; set; }
        public int MinCodeLength { get; set; }
        public int MaxCodeLength { get; set; }
        public string LocalCountryCode { get; set; }
        public int LocalNumberLength { get; set; }
        public string InterconnectGeneralPrefix { get; set; }
        public List<OutgoingTrafficCustomer> OutgoingTrafficCustomers { get; set; }
        public List<IncomingTrafficSupplier> IncomingTrafficSuppliers { get; set; }
        public List<LocalSupplierMapping> LocalSupplierMappings { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }


        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            IWhSRouteSyncEricssonDataManager whSRouteSyncEricssonDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IWhSRouteSyncEricssonDataManager>();
            whSRouteSyncEricssonDataManager.Initialize(new WhSRouteSyncEricssonInitializeContext());

            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.Initialize(new RouteInitializeContext());

            IRouteCaseDataManager routeCaseDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            routeCaseDataManager.Initialize(new RouteCaseInitializeContext());
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
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

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            Dictionary<string, List<int>> carrierAccountIdsByTrunkName = new Dictionary<string, List<int>>();

            foreach (var mapping in this.CarrierMappings.Values)
            {
                if (mapping.CustomerMapping != null && mapping.CustomerMapping.InTrunks != null)
                {
                    foreach (var inTrunk in mapping.CustomerMapping.InTrunks)
                    {
                        List<int> carrierAccountIds = carrierAccountIdsByTrunkName.GetOrCreateItem(inTrunk.TrunkName);
                        carrierAccountIds.Add(mapping.CarrierId);
                    }
                }
            }

            Dictionary<string, List<int>> duplicatedInTrunks = carrierAccountIdsByTrunkName.Where(itm => itm.Value.Count > 1).ToDictionary(itm => itm.Key, itm => itm.Value);
            if (duplicatedInTrunks.Count == 0)
                return true;

            List<string> validationMessages = new List<string>();

            if (duplicatedInTrunks.Count > 0)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                foreach (var kvp in duplicatedInTrunks)
                {
                    string trunkName = kvp.Key;
                    List<int> customerIds = kvp.Value;

                    List<string> carrierAccountNames = new List<string>();

                    foreach (var customerId in customerIds)
                    {
                        string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
                        carrierAccountNames.Add(string.Format("'{0}'", customerName));
                    }

                    validationMessages.Add(string.Format("Trunk Name: '{0}' is duplicated at Carrier Accounts: {1}", trunkName, string.Join(", ", carrierAccountNames)));
                }
            }

            context.ValidationMessages = validationMessages;
            return false;
        }

        #endregion
    }
}