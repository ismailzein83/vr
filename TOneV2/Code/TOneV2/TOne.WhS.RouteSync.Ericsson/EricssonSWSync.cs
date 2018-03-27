using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

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
        public List<int> OutgoingTrafficCustomers { get; set; }
        public List<int> IncomingTrafficSuppliers { get; set; }
        public List<LocalSupplierMapping> LocalSupplierMappings { get; set; }
        public string InterconnectGeneralPrefix { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }


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

        #endregion
    }
}