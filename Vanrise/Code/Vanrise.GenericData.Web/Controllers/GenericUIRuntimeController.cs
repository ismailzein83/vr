using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericUIRuntime")]
    public class GenericUIRuntimeController:BaseAPIController
    {
        [HttpGet]
        [Route("GetExtensibleBEItemRuntime")]
        public ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(Guid dataRecordTypeId, Guid businessEntityDefinitionId)
        {
            var manager = GetManager(businessEntityDefinitionId);
            return manager.GetExtensibleBEItemRuntime(dataRecordTypeId, businessEntityDefinitionId);
        }
        //[HttpGet]
        //[Route("GetGenericManagementRuntime")]
        //public GenericManagementRuntime GetGenericManagementRuntime(Guid businessEntityDefinitionId)
        //{
        //    GenericUIRuntimeManager manager = new GenericUIRuntimeManager();
        //    return manager.GetManagementRuntime(businessEntityDefinitionId);
        //}
      //  [HttpGet]
        //[Route("GetGenericEditorRuntime")]
        //public GenericEditorRuntime GetGenericEditorRuntime(Guid businessEntityDefinitionId)
        //{
        //    GenericUIRuntimeManager manager = new GenericUIRuntimeManager();
        //    return manager.GetGenericEditorRuntime(businessEntityDefinitionId);
        //}
        [HttpPost]
        [Route("GetGenericEditorRuntimeRows")]
        public List<GenericEditorRuntimeRow> GetGenericEditorRuntimeRows(GenericEditorRuntimeRowsInput input)
        {
            GenericUIRuntimeManager manager = new GenericUIRuntimeManager();
            return manager.GetGenericEditorRuntimeRows(input.Rows, input.DataRecordTypeId);
        }
        [HttpPost]
        [Route("GetGenericEditorRuntimeSections")]
        public List<GenericEditorRuntimeSection> GetGenericEditorRuntimeSections(GenericEditorDefinitionSectionsInput input)
        {
            GenericUIRuntimeManager manager = new GenericUIRuntimeManager();
            return manager.GetGenericEditorRuntimeSections(input.Sections, input.DataRecordTypeId);
        }
        [HttpGet]
        [Route("GetDataRecordTypesInfo")]
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(Guid businessEntityDefinitionId)
        {
            var manager = GetManager(businessEntityDefinitionId);
            return manager.GetDataRecordTypesInfo(businessEntityDefinitionId);
        }
        IExtensibleBEManager GetManager(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);

            Type managerType = Type.GetType(businessEntityDefinition.Settings.ManagerFQTN);
            return Activator.CreateInstance(managerType) as IExtensibleBEManager;
        }

    }
    public class GenericEditorDefinitionSectionsInput
    {
        public Guid DataRecordTypeId { get; set; }
        public List<GenericEditorSection> Sections { get; set; }
    }
    public class GenericEditorRuntimeRowsInput
    {
        public Guid DataRecordTypeId { get; set; }
        public List<GenericEditorRow> Rows { get; set; }
    }
}