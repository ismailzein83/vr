using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchSWSync : SwitchRouteSynchronizer
    {
        #region properties
        public string OwnerName { get; set; }
        public string MasterConnectionString { get; set; }
        public string RouteConnectionString { get; set; }
        public string TariffConnectionString { get; set; }
        public int NumberOfOptions { get; set; }
        public string Separator { get; set; }

        #endregion
        public override Guid ConfigId { get { return new Guid("64152327-5DB5-47AE-9569-23D38BCB18CC"); } }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            //prepare data 
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return "";
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {

        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {

        }
    }
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }
        public string InnerPrefix { get; set; }

    }
}
