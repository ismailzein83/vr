using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CountryWithDefaultRate")]
    public class CountryWithDefaultRateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCountries")]
        public object GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryWithDefaultRateQuery> input)
        {
            CountryWithDefaultRateManager manager = new CountryWithDefaultRateManager();
            return GetWebResponse(input, manager.GetFilteredCountries(input), "Country With Default Rate");
        }
    }
}