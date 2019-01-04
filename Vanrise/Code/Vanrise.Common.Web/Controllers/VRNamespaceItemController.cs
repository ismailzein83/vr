using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNamespaceItem")]
    [JSONWithTypeAttribute]
    public class VRNamespaceItemController : BaseAPIController
    {
        VRNamespaceItemManager _manager = new VRNamespaceItemManager();

        [HttpPost]
        [Route("GetFilteredVRNamespaceItems")]
        public object GetFilteredVRNamespaceItems(Vanrise.Entities.DataRetrievalInput<VRNamespaceItemQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRNamespaceItems(input), "NamespaceItems");
        }

        [HttpGet]
        [Route("GetVRNamespaceItem")]
        public VRNamespaceItem GetVRNamespaceItem(Guid vrNamespaceItemId)
        {
            return _manager.GetVRNamespaceItem(vrNamespaceItemId);
        }

        [HttpPost]
        [Route("AddVRNamespaceItem")]
        public Vanrise.Entities.InsertOperationOutput<VRNamespaceItemDetail> AddVRNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {
            return _manager.AddVRNamespaceItem(vrNamespaceItem);
        }

        [HttpPost]
        [Route("UpdateVRNamespaceItem")]
        public Vanrise.Entities.UpdateOperationOutput<VRNamespaceItemDetail> UpdateVRNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {
            return _manager.UpdateVRNamespaceItem(vrNamespaceItem);
        }

        [HttpGet]

        [Route("GetVRDynamicCodeSettingsConfigs")]
        public IEnumerable<VRDynamicCodeConfig> GetVRDynamicCodeSettingsConfigs()
        {
            return _manager.GetVRDynamicCodeSettingsConfigs();
        }

        [HttpPost]
        [Route("TryCompileNamespaceItem")]
        public Vanrise.Common.Business.VRNamespaceItemManager.VRNamespaceItemCompilationOutput TryCompileNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {
            return _manager.TryCompileNamespaceItem(vrNamespaceItem);
        }

        [HttpPost]
        [Route("ExportCompilationResult")]
        public object ExportCompilationResult(VRNamespaceItem vrNamespaceItem)
        {
            Vanrise.Common.Business.VRNamespaceItemManager.VRNamespaceItemCompilationOutput result = TryCompileNamespaceItem(vrNamespaceItem);
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