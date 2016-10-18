using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataTransformationDefinition")]
    public class DataTransformationDefinitionController : BaseAPIController
    {

        [HttpGet]
        [Route("GetDataTransformationStepConfig")]
        public IEnumerable<DataTransformationStepConfig> GetDataTransformationStepConfig()
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationStepConfig();
        }

        [HttpGet]
        [Route("GetDataTransformationDefinition")]
        public DataTransformationDefinition GetDataTransformationDefinition(Guid dataTransformationDefinitionId)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationDefinition(dataTransformationDefinitionId);
        }

        [HttpGet]
        [Route("GetDataTransformationDefinitionRecords")]
        public IEnumerable<DataTransformationRecordType> GetDataTransformationDefinitionRecords(Guid dataTransformationDefinitionId)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId);
        }


        [HttpGet]
        [Route("GetDataTransformationRecordsInfo")]
        public IEnumerable<DataTransformationRecordType> GetDataTransformationRecordsInfo(Guid dataTransformationDefinitionId, string filter = null)
        {
            DataTransformationRecordTypeInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DataTransformationRecordTypeInfoFilter>(filter) : null;
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId, serializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredDataTransformationDefinitions")]
        public object GetFilteredDataTransformationDefinitions(Vanrise.Entities.DataRetrievalInput<DataTransformationDefinitionQuery> input)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return GetWebResponse(input, dataRecordTypeManager.GetFilteredDataTransformationDefinitions(input));
        }

        [Route("UpdateDataTransformationDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<DataTransformationDefinitionDetail> UpdateDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.UpdateDataTransformationDefinition(dataTransformationDefinition);
        }

        [HttpPost]
        [Route("AddDataTransformationDefinition")]
        public Vanrise.Entities.InsertOperationOutput<DataTransformationDefinitionDetail> AddDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.AddDataTransformationDefinition(dataTransformationDefinition);
        }


        [HttpGet]
        [Route("GetDataTransformationDefinitions")]
        public IEnumerable<DataTransformationDefinitionInfo> GetDataTransformationDefinitions(string filter = null)
        {
            DataTransformationDefinitionFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DataTransformationDefinitionFilter>(filter) : null;
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationDefinitions(deserializedFilter);

        }

        [HttpPost]
        [Route("TryCompileSteps")]
        public DataTransformationCompilationOutput TryCompileSteps(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            DataTransformationRuntimeType dataTransformationRuntimeType;
            List<string> errorMessages;
            bool compilationResult = dataRecordTypeManager.TryCompileDataTransformation(dataTransformationDefinition, out dataTransformationRuntimeType, out errorMessages);

            if (compilationResult)
            {
                return new DataTransformationCompilationOutput
                {
                    ErrorMessages = null,
                    Result = true
                };
            }
            else
            {
                return new DataTransformationCompilationOutput
                {
                    ErrorMessages = errorMessages,
                    Result = false
                };
            }
        }

        [HttpPost]
        [Route("ExportCompilationResult")]
        public object ExportCompilationResult(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationCompilationOutput result = TryCompileSteps(dataTransformationDefinition);
            return base.GetExcelResponse(result.ErrorMessages.SelectMany(s => System.Text.Encoding.ASCII.GetBytes(s)).ToArray(), "CompilationResult.xls");
        }
    }
}