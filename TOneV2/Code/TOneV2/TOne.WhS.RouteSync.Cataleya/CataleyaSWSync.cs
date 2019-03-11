using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Cataleya
{
    public class CataleyaSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("D770F53B-057F-4BB8-BF20-883A2DBC510B"); } }

        public string MappingSeparator { get; set; }

        public int? NumberOfMappings { get; set; }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public Guid VRConnectionId { get; set; }


        #region Public Methods

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
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