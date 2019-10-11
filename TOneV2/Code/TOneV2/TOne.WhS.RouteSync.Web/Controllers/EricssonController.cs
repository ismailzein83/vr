using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Ericsson;
using TOne.WhS.RouteSync.Ericsson.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Ericsson")]
    public class EricssonController : BaseAPIController
    {
        EricssonManager _manager = new EricssonManager();

        [HttpPost]
        [Route("IsSupplierTrunkUsed")]
        public IsSupplierTrunkUsedOutput IsSupplierTrunkUsed(IsSupplierTrunkUsedInput input)
        {
            List<UpdateAdditionalMessage> errorMessages;
            bool isSupplierTrunkUsed = _manager.IsSupplierTrunkUsed(input.SupplierId, input.TrunkId, input.TrunkName, input.CarrierMappings, out errorMessages);

            IsSupplierTrunkUsedOutput output = new IsSupplierTrunkUsedOutput();
            output.Result = isSupplierTrunkUsed;
            output.ErrorMessages = errorMessages;
            return output;
        }
    }

    public class IsSupplierTrunkUsedInput
    {
        public int SupplierId { get; set; }

        public Guid TrunkId { get; set; }

        public string TrunkName { get; set; }

        public Dictionary<int, CarrierMapping> CarrierMappings { get; set; }
    }

    public class IsSupplierTrunkUsedOutput
    {
        public bool Result { get; set; }

        public List<UpdateAdditionalMessage> ErrorMessages { get; set; }
    }
}