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
        [Route("GetDataTransformationDefinition")]
        public DataTransformationDefinition GetDataTransformationDefinition(int dataTransformationDefinitionId)
        {
            DataTransformationDefinitionManager dataRecordTypeManager = new DataTransformationDefinitionManager();
            return dataRecordTypeManager.GetDataTransformationDefinition(dataTransformationDefinitionId);
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
    }
}