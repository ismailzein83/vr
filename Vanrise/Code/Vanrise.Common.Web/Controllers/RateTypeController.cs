using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business; 
using Vanrise.Common.Web;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Web.Controllers
{
    [RoutePrefix(Vanrise.Common.Web.Constants.ROUTE_PREFIX + "RateType")]
    public class RateTypeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRateTypes")]
        public object GetFilteredRateTypes(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.RateTypeQuery> input)
        {
            RateTypeManager manager = new RateTypeManager();
            return GetWebResponse(input, manager.GetFilteredRateTypes(input));
        }

        [HttpGet]
        [Route("GetAllRateTypes")]
        public IEnumerable<Vanrise.Entities.RateTypeInfo> GetAllRateTypes()
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.GetAllRateTypes();
        }
        
        [HttpGet]
        [Route("GetRateType")]
        public Vanrise.Entities.RateType GetRateType(int rateTypeId)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.GetRateType(rateTypeId);
        }

        [HttpPost]
        [Route("AddRateType")]
        public Vanrise.Entities.InsertOperationOutput<Vanrise.Entities.RateTypeDetail> AddRateType(Vanrise.Entities.RateType rateType)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.AddRateType(rateType);
        }
        
        [HttpPost]
        [Route("UpdateRateType")]
        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Entities.RateTypeDetail> UpdateRateType(Vanrise.Entities.RateType rateType)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.UpdateRateType(rateType);
        }
    }
}
