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
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBusinessEntityDefinition")]
    [JSONWithTypeAttribute]
    public class GenericBusinessEntityDefinitionController : BaseAPIController
    {
        GenericBusinessEntityDefinitionManager _manager = new GenericBusinessEntityDefinitionManager();
        [HttpGet]
        [Route("GetGenericBEGridDefinition")]
        public GenericBEGridDefinition GetGenericBEGridDefinition(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionGridDefinition(businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetGenericBEGridColumnAttributes")]
        public List<GenericBEDefinitionGridColumnAttribute> GetGenericBEGridColumnAttributes(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionGridColumnAttributes(businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetGenericBEDefinitionSettings")]
        public GenericBEDefinitionSettings GetGenericBEDefinitionSettings(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetIdFieldTypeForGenericBE")]
        public DataRecordField GetIdFieldTypeForGenericBE(Guid businessEntityDefinitionId)
        {
            return _manager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetGenericBEViewDefinitionSettingsConfigs")]
        public IEnumerable<GenericBEViewDefinitionSettingsConfig> GetGenericBEViewDefinitionSettingsConfigs()
        {
            return _manager.GetGenericBEViewDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBEEditorDefinitionSettingsConfigs")]
        public IEnumerable<GenericBEEditorDefinitionSettingsConfig> GetGenericBEEditorDefinitionSettingsConfigs()
        {
            return _manager.GetGenericBEEditorDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBEFilterDefinitionSettingsConfigs")]
        public IEnumerable<GenericBEFilterDefinitionSettingsConfig> GetGenericBEFilterDefinitionSettingsConfigs()
        {
            return _manager.GetGenericBEFilterDefinitionSettingsConfigs();
        }


        [HttpGet]
        [Route("GetGenericBEActionDefinitionSettingsConfigs")]
        public IEnumerable<GenericBEActionDefinitionSettingsConfig> GetGenericBEActionDefinitionSettingsConfigs()
        {
            return _manager.GetGenericBEActionDefinitionSettingsConfigs();
        }
        
    }
}