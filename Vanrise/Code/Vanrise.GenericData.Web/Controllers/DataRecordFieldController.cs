﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordFields")]
    public class DataRecordFieldsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordFieldTypeConfigs")]
        public IEnumerable<DataRecordFieldTypeConfig> GetDataRecordFieldTypeConfigs()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldTypeConfigs();
        }
        [HttpGet]
        [Route("GetListRecordRuntimeViewTypeConfigs")]
        public IEnumerable<ListRecordRuntimeViewTypeConfig> GetListRecordRuntimeViewTypeConfigs()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetListRecordRuntimeViewTypeConfigs();
        }
        
        [HttpGet]
        [Route("GetDataRecordFieldsInfo")]
        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(Guid dataRecordTypeId, string serializedFilter = null)
        {
            DataRecordFieldInfoFilter filter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<DataRecordFieldInfoFilter>(serializedFilter) : null;
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldsInfo(dataRecordTypeId, filter);
        }
        [HttpPost]
        [Route("GetListDataRecordTypeGridViewColumnAtts")]
        public List<GenericBEDefinitionGridColumnAttribute> GetListDataRecordTypeGridViewColumnAtts(DataRecordTypeGridViewColumnsInput input)
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetListDataRecordTypeGridViewColumnAtts(input);
        }
 
        [HttpGet]
        [Route("GetDataRecordAttributes")]
        public List<DataRecordGridColumnAttribute> GetDataRecordAttributes(Guid dataRecordTypeId)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordAttributes(dataRecordTypeId);
        }
        [HttpGet]
        [Route("GetDataRecordFieldFormulaExtensionConfigs")]
        public IEnumerable<DataRecordFieldFormulaConfig> GetDataRecordFieldFormulaExtensionConfigs()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldFormulaExtensionConfigs();
        }
        [HttpPost]
        [Route("TryResolveDifferences")]
        public GenericFieldDifferencesResolver TryResolveDifferences(TryResolveDifferencesInput input)
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.TryResolveDifferences(input.LoggableEntityUniqueName, input.FieldValues);
        }

        [HttpGet]
        [Route("GetFieldCustomObjectTypeSettingsConfig")]
        public IEnumerable<FieldCustomObjectTypeSettingsConfig> GetFieldCustomObjectTypeSettingsConfig()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetFieldCustomObjectTypeSettingsConfig();
        }

        //[HttpPost]
        //[Route("GetFieldTypeDescription")]
        //public DataRecordFieldTypeInfo GetFieldTypeDescription(FieldTypeDescriptionInput input)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.GetFieldTypeDescription(input);
        //}
        [HttpPost]
        [Route("GetFieldTypeDescription")]
        public string GetFieldTypeDescription(FieldTypeDescriptionInput input)
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetFieldTypeDescription(input);
        }
        [HttpPost]
        [Route("GetFieldTypeListDescription")]
        public List<Dictionary<string,string>> GetFieldTypeListDescription(ListFieldTypeDescriptionInput input)
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetFieldTypeListDescription(input);
        }
        [HttpPost]
        [Route("GetFieldsDescription")]
        public List<Dictionary<string, string>> GetFieldsDescription(FieldsDescriptionInput input)
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetFieldsDescription(input);
        }

    }
   
    public class TryResolveDifferencesInput
    {
        public string LoggableEntityUniqueName { get; set; }
        public List<GenericFieldChangeInfo> FieldValues { get; set; }
    }
}