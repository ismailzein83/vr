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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntity")]
    public class ExtensibleBEController:BaseAPIController
    {
        [HttpGet]
        [Route("GetExtensibleBEItemRuntime")]
        public ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(int dataRecordTypeId, int businessEntityDefinitionId)
        {
            var manager = GetManager(businessEntityDefinitionId);
            return manager.GetExtensibleBEItemRuntime(dataRecordTypeId, businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetDataRecordTypesInfo")]
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(int businessEntityDefinitionId)
        {
            var manager = GetManager(businessEntityDefinitionId);

            return manager.GetDataRecordTypesInfo(businessEntityDefinitionId);
        }
        IExtensibleBEManager GetManager(int businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);

            Type managerType = Type.GetType(businessEntityDefinition.Settings.ManagerFQTN);
            return Activator.CreateInstance(managerType) as IExtensibleBEManager;
        }
    }
}