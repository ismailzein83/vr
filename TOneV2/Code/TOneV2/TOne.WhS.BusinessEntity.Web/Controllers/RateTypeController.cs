using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{

    [RoutePrefix (Constants.ROUTE_PREFIX + "RateType")]
    public class WhSBE_RateTypeController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredRateTypes")]
        public object GetFilteredRateTypes(Vanrise.Entities.DataRetrievalInput<RateTypeQuery> input)
        {
            RateTypeManager manager = new RateTypeManager();
            return GetWebResponse(input, manager.GetFilteredRateTypes(input));
        }

        [HttpGet]
        [Route("GetAllRateTypes")]
        public IEnumerable<RateType> GetAllRateTypes()
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.GetAllRateTypes();
        }
        [HttpGet]
        [Route("GetRateType")]
        public RateType GetRateType(int rateTypeId)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.GetRateType(rateTypeId);
        }

        [HttpPost]
        [Route("AddRateType")]
        public TOne.Entities.InsertOperationOutput<RateType> AddRateType(RateType rateType)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.AddRateType(rateType);
        }
        [HttpPost]
        [Route("UpdateRateType")]
        public TOne.Entities.UpdateOperationOutput<RateType> UpdateRateType(RateType rateType)
        {
            RateTypeManager manager = new RateTypeManager();
            return manager.UpdateRateType(rateType);
        }

    }
}