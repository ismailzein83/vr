using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusSWSync : SwitchRouteSynchronizer
    {
        public const int SupplierMappingLength = 4;

        public override Guid ConfigId { get { return new Guid("99B59E02-1305-49E5-9342-1B4E08C91439"); } }

        public IFreeRadiusPostgresDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public int? NumberOfMappings { get; set; }

        public string SupplierOptionsSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

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
    }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<CustomerMapping> CustomerMapping { get; set; }

        public List<SupplierMapping> SupplierMapping { get; set; }
    }

    public class CustomerMapping
    {
        public string  Mapping { get; set; }  
    }

    public class SupplierMapping
    {
        public string Mapping { get; set; }
    }
}
