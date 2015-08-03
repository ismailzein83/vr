using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class NumberProfileController : BaseAPIController
    {

        [HttpPost]
        public object GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileResultQuery> input)
        {
            NumberProfileManager manager = new NumberProfileManager();

            return GetWebResponse(input, manager.GetNumberProfiles(input));
        }
    }
}