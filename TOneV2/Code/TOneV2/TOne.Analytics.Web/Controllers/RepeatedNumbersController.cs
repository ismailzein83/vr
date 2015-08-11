using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class RepeatedNumbersController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetRepeatedNumbersData(Vanrise.Entities.DataRetrievalInput<RepeatedNumbersInput> input)
        {
            RepeatedNumbersManager manager = new RepeatedNumbersManager();
            return GetWebResponse(input, manager.GetRepeatedNumbersData(input));

        }
    }
}