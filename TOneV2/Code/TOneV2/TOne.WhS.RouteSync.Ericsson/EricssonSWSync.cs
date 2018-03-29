using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;

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
            IRouteDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            dataManager.Initialize(context);
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

        #endregion
    }
}