﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBusinessEntity")]
    public class GenericBusinessEntityController : BaseAPIController
    {
        GenericBusinessEntityManager _manager = new GenericBusinessEntityManager();

        [HttpPost]
        [Route("GetFilteredGenericBusinessEntities")]
        public object GetFilteredGenericBusinessEntities(Vanrise.Entities.DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            if (!_manager.DoesUserHaveViewAccess(input.Query.BusinessEntityDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredGenericBusinessEntities(input), _manager.GenericBusinessEntityDefinitionTitle(input.Query.BusinessEntityDefinitionId));
        }

        [HttpGet]
        [Route("GetGenericBusinessEntityEditorRuntime")]
        public GenericBusinessEntityRuntimeEditor GetGenericBusinessEntityEditorRuntime(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId, int? historyId = null)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId, historyId);
        }
        [HttpGet]
        [Route("GetGenericBusinessEntityDetail")]
        public GenericBusinessEntityDetail GetGenericBusinessEntityDetail(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetGenericBusinessEntityDetail(genericBusinessEntityId, businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetGenericBusinessEntity")]
        public GenericBusinessEntity GetGenericBusinessEntity(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid businessEntityDefinitionId)
        {
            return _manager.DoesUserHaveAddAccess(businessEntityDefinitionId);
        }

        [HttpPost]
        [Route("AddGenericBusinessEntity")]
        public object AddGenericBusinessEntity(GenericBusinessEntityToAdd genericBusinessEntity)
        {
            if (!DoesUserHaveAddAccess(genericBusinessEntity.BusinessEntityDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.AddGenericBusinessEntity(genericBusinessEntity);
        }

        [HttpPost]
        [Route("UpdateGenericBusinessEntity")]
        public object UpdateGenericBusinessEntity(GenericBusinessEntityToUpdate genericBusinessEntity)
        {
            if (!DoesUserHaveEditAccess(genericBusinessEntity.BusinessEntityDefinitionId))
                return GetUnauthorizedResponse();

            return _manager.UpdateGenericBusinessEntity(genericBusinessEntity);
        }

        [HttpPost]
        [Route("DeleteGenericBusinessEntity")]
        public object DeleteGenericBusinessEntity(DeleteGenericBusinessEntityInput input)
        {
            if (!_manager.DoesUserHaveActionAccess("Delete", input.BusinessEntityDefinitionId, input.BusinessEntityActionTypeId))
                return GetUnauthorizedResponse();

            return _manager.DeleteGenericBusinessEntity(input);
        }

        [HttpGet]
        [Route("DoesUserHaveEditAccess")]
        public bool DoesUserHaveEditAccess(Guid businessEntityDefinitionId)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.DoesUserHaveEditAccess(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("DoesUserHaveViewAccess")]
        public bool DoesUserHaveViewAccess(Guid businessEntityDefinitionId)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.DoesUserHaveViewAccess(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("DoesUserHaveDeleteAccess")]
        public bool DoesUserHaveDeleteAccess(Guid businessEntityDefinitionId)
        {
            return _manager.DoesUserHaveDeleteAccess(businessEntityDefinitionId);
        }


        [HttpGet]
        [Route("GetGenericBusinessEntityInfo")]
        public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, string serializedFilter = null, string searchValue = null)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            GenericBusinessEntityInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<GenericBusinessEntityInfoFilter>(serializedFilter) : null;
            return manager.GetGenericBusinessEntityInfo(businessEntityDefinitionId, filter, searchValue);
        }

        [HttpGet]
        [Route("DownloadGenericBusinessEntityTemplate")]
        public object DownloadGenericBusinessEntityTemplate(Guid businessEntityDefinitionId)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            byte[] templateWithDateFormatBytes = manager.DownloadGenericBusinessEntityTemplate(businessEntityDefinitionId);
            MemoryStream memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(templateWithDateFormatBytes, 0, templateWithDateFormatBytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, string.Format("{0} Template.xlsx", manager.GenericBusinessEntityDefinitionTitle(businessEntityDefinitionId)));

        }

        [HttpGet]
        [Route("UploadGenericBusinessEntities")]
        public UploadGenericBusinessEntityLog UploadGenericBusinessEntities(Guid businessEntityDefinitionId, int fileID)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.AddUploadedGenericBusinessEntities(businessEntityDefinitionId, fileID);
        }
        [HttpGet]
        [Route("DownloadBusinessEntityLog")]
        public object DownloadBusinessEntityLog(int fileID)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            byte[] bytes = manager.DownloadBusinessEntityLog(fileID);
            return GetExcelResponse(bytes, "ImportedResults.xls");
        }

        [HttpPost]
        [Route("ExecuteGenericBEBulkActions")]
        public object ExecuteGenericBEBulkActions(ExecuteGenericBEBulkActionProcessInput input)
        {
            return _manager.ExecuteGenericBEBulkActions(input);
        }
        [HttpGet]
        [Route("GetGenericBETitleFieldValue")]
        public string GetGenericBETitleFieldValue(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetGenericBETitleFieldValue(genericBusinessEntityId, businessEntityDefinitionId);
        }
        [HttpPost]
        [Route("GetGenericEditorColumnsInfo")]
        public List<GridColumnAttribute> GetGenericEditorColumnsInfo(GetGenericEditorColumnsInfoInput input)
        {
            GetGenericEditorColumnsInfoContext getGenericEditorColumnsInfoContext = new GetGenericEditorColumnsInfoContext { DataRecordTypeId = input.DataRecordTypeId };
            return input.GenericEditorDefinitionSetting.GetGridColumnsAttributes(getGenericEditorColumnsInfoContext);
        }


        [HttpPost]
        [Route("GetDependentFieldValues")]
        public Dictionary<string, object> GetDependentFieldValues(GetDependentFieldValuesInput input)
        {
            input.ThrowIfNull("input");
            return _manager.GetDependentFieldValues(input.DataRecordTypeId, input.FieldValues);
        }
        [HttpPost]
        [Route("ExecuteRangeGenericEditorProcess")]
        public RangeGenericEditorProcessMessage ExecuteRangeGenericEditorProcess(RangeGenericEditorProcessInput input)
        {
            return _manager.ExecuteRangeGenericEditorProcess(input);
        }

        [HttpGet]
        [Route("GetGenericBESelectorConditionConfigs")]
        public IEnumerable<GenericBESelectorConditionConfig> GetGenericBESelectorConditionConfigs()
        {
            return _manager.GetGenericBESelectorConditionConfigs();
        }

        public class GetDependentFieldValuesInput
        {
            public Guid DataRecordTypeId { get; set; }

            public Dictionary<string, object> FieldValues { get; set; }
        }

    }
}