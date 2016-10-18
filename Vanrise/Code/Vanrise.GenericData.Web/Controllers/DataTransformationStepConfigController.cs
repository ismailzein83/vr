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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataTransformationStepConfig")]
    public class DataTransformationStepConfigController : BaseAPIController
    {
        //[HttpGet]
        //[Route("GetDataTransformationSteps")]
        //public IEnumerable<DataTransformationStepConfig> GetDataTransformationSteps()
        //{
        //    DataTransformationStepConfigManager manager = new DataTransformationStepConfigManager();
        //    return manager.GetDataTransformationSteps();
        //}

    }
}