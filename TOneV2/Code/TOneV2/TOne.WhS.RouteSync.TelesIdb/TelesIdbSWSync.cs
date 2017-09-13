using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;

namespace TOne.WhS.RouteSync.TelesIdb
{
    public class TelesIdbSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("29135479-8150-4E23-9A0D-A42AF69A13AE"); } }

        public IIdbDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        public bool UseTwoMappingFormat { get; set; }


        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }


        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {

        }

        public override Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            throw new NotImplementedException();
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

        public string SupplierMapping { get; set; }
    }
}
