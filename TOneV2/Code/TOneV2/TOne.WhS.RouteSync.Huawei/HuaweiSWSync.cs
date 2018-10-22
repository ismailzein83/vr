using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public class HuaweiSWSync : SwitchRouteSynchronizer
    {
        int _supplierMappingLength = 4;
        public override Guid ConfigId { get { return new Guid("376687E2-268D-4DFA-AA39-3205C3CD18E5"); } }

        public string MappingSeparator { get; set; }
        public int? NumberOfMappings { get; set; }
        public int SupplierMappingLength
        {
            get { return _supplierMappingLength; }
            set { _supplierMappingLength = value; }
        }
        public string SupplierOptionsSeparator { get; set; }
        public int NumberOfOptions { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public class CarrierMapping
        {
            public int CarrierId { get; set; }

            public List<string> CustomerMapping { get; set; }

            public List<string> SupplierMapping { get; set; }
        }
        public List<HuaweiSSHCommunication> SwitchCommunicationList { get; set; }
        public List<SwitchLogger> SwitchLoggerList { get; set; }

        #region Public Methods
        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
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

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}