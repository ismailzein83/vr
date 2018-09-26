using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNamespace")]
    [JSONWithTypeAttribute]
    public class VRNamespaceController : BaseAPIController
    {
        VRNamespaceManager _manager = new VRNamespaceManager();

        [HttpPost]
        [Route("GetFilteredVRNamespaces")]
        public object GetFilteredVRNamespaces(Vanrise.Entities.DataRetrievalInput<VRNamespaceQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRNamespaces(input), "Namespaces");
        }

        [HttpGet]
        [Route("GetVRNamespace")]
        public VRNamespace GetVRNamespace(Guid vrNamespaceId)
        {
            return _manager.GetVRNamespace(vrNamespaceId);
        }

        [HttpPost]
        [Route("AddVRNamespace")]
        public Vanrise.Entities.InsertOperationOutput<VRNamespaceDetail> AddVRNamespace(VRNamespace vrNamespace)
        {
            return _manager.AddVRNamespace(vrNamespace);
        }

        [HttpPost]
        [Route("UpdateVRNamespace")]
        public Vanrise.Entities.UpdateOperationOutput<VRNamespaceDetail> UpdateVRNamespace(VRNamespace vrNamespace)
        {
            return _manager.UpdateVRNamespace(vrNamespace);
        }

        [HttpPost]
        [Route("TryCompileNamespace")]
        public Vanrise.Common.Business.VRNamespaceManager.VRNamespaceCompilationOutput TryCompileNamespace(VRNamespace vrNamespace)
        {
            return _manager.TryCompileNamespace(vrNamespace);
        }

        [HttpPost]
        [Route("ExportCompilationResult")]
        public object ExportCompilationResult(VRNamespace vrNamespace)
        {
            Vanrise.Common.Business.VRNamespaceManager.VRNamespaceCompilationOutput result = TryCompileNamespace(vrNamespace);
            var errorsByte = new List<byte>();

            foreach (var error in result.ErrorMessages)
            {
                byte[] encodedError = System.Text.Encoding.ASCII.GetBytes(error);
                for (var i = 0; i < encodedError.Length; i++)
                {
                    errorsByte.Add(encodedError[i]);
                }
            }

            return base.GetExcelResponse(errorsByte.ToArray(), "CompilationResult.xls");
        }
    }
}