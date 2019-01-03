﻿using System;
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
         [Route("GetGenericBusinessEntityRuntimeInfo")]
        public GenericBESelectorRuntimeInfo GetGenericBusinessEntityRuntimeInfo(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBESelectorRuntimeInfo(businessEntityDefinitionId);
        }
    
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

        [HttpGet]
        [Route("GetGenericBEExtendedSettingsConfigs")]
        public IEnumerable<GenericBEExtendedSettingsConfig> GetGenericBEExtendedSettingsConfigs()
        {
            return _manager.GetGenericBEExtendedSettingsConfigs();
        }
        [HttpGet]
        [Route("GenericBEActionFilterConditionConfigs")]
        public IEnumerable<GenericBEActionFilterConditionConfig> GetGenericBEActionFilterConditionConfigs()
        {
            return _manager.GetGenericBEActionFilterConditionConfigs();
        }


        [HttpGet]
        [Route("GetGenericBEOnAfterSaveHandlerSettingsConfigs")]
        public IEnumerable<GenericBEOnAfterSaveHandlerSettingsConfig> GetGenericBEOnAfterSaveHandlerSettingsConfigs()
        {
            return _manager.GetGenericBEOnAfterSaveHandlerSettingsConfigs();
        }


        [HttpGet]
        [Route("GetGenericBEOnBeforeInsertHandlerSettingsConfigs")]
        public IEnumerable<GenericBEOnBeforeInsertHandlerSettingsConfig> GetGenericBEOnBeforeInsertHandlerSettingsConfigs()
        {
            return _manager.GetGenericBEOnBeforeInsertHandlerSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBESaveConditionSettingsConfigs")]
        public IEnumerable<GenericBESaveConditionSettingsConfig> GetGenericBESaveConditionSettingsConfigs()
        {
            return _manager.GetGenericBESaveConditionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBEConditionSettingsConfigs")]
        public IEnumerable<GenericBEConditionSettingsConfig> GetGenericBEConditionSettingsConfigs()
        {
            return _manager.GetGenericBEConditionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBESerialNumberPartSettingsConfigs")]
        public IEnumerable<GenericBESerialNumberPartSettingsConfig> GetGenericBESerialNumberPartSettingsConfigs()
        {
            return _manager.GetGenericBESerialNumberPartSettingsConfigs();
        }

        [HttpGet]
        [Route("GetGenericBEOnBeforeGetFilteredHandlerSettingsConfigs")]
        public IEnumerable<GenericBEOnBeforeGetFilteredHandlerSettingsConfig> GetGenericBEOnBeforeGetFilteredHandlerSettingsConfigs()
        {
            return _manager.GetGenericBEOnBeforeGetFilteredHandlerSettingsConfigs();
        }

        [HttpGet]
        [Route("GetDataRecordTypeFields")]
        public Dictionary<string, DataRecordField> GetDataRecordTypeFields(Guid dataRecordTypeId)
        {
            return _manager.GetDataRecordTypeFields(dataRecordTypeId);
        }

        [HttpPost]
        [Route("GetRemoteGenericBEDefinitionInfo")]
        public IEnumerable<BusinessEntityDefinitionInfo> GetRemoteGenericBEDefinitionInfo(RemoteGenericBEDefinitionInfoInput input)
        {
            return _manager.GetRemoteGenericBEDefinitionInfo(input);
        }

        [HttpPost]
        [Route("GetUpdateBulkActionGenericBEFieldsRuntime")]
        public IEnumerable<UpdateGenericBEFieldRuntime> GetUpdateBulkActionGenericBEFieldsRuntime(GetUpdateBulkActionsRuntimeInput input)
        {
            return _manager.GetUpdateBulkActionGenericBEFieldsRuntime(input);
        }
        [HttpGet]
        [Route("GetGenericBEBulkActionSettingsConfigs")]
        public IEnumerable<GenericBEBulkActionSettingsConfig> GetGenericBEBulkActionSettingsConfigs()
        {
            return _manager.GetGenericBEBulkActionSettingsConfigs();
        }

    }
}