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
        int _supplierRNLength = 3;
        public override Guid ConfigId { get { return new Guid("376687E2-268D-4DFA-AA39-3205C3CD18E5"); } }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

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

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public CustomerMapping CustomerMapping { get; set; }

        public SupplierMapping SupplierMapping { get; set; }
    }

    public class CustomerMapping
    {
        public int RSSN { get; set; }

        public string CSCName { get; set; }

        public List<DNSet> DNSets { get; set; }
    }

    public class SupplierMapping
    {
        public string RouteName { get; set; }

        public string ISUP { get; set; }
    }

    public class DNSet
    {
        public int Number { get; set; }
    }
}