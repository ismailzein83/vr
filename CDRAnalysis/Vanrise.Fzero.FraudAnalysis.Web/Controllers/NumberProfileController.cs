using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "NumberProfile")]
    [JSONWithTypeAttribute]
    public class NumberProfileController : BaseAPIController
    {

        [HttpPost]
        [Route("GetNumberProfiles")]
        public object GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileQuery> input)
        {
            NumberProfileManager manager = new NumberProfileManager();

            return GetWebResponse(input, manager.GetNumberProfiles(input));
        }
    }
}