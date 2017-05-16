using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerCountry")]
    public class CustomerCountryController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("AreEffectiveOrFutureCountriesSoldToCustomer")]
        public bool AreEffectiveOrFutureCountriesSoldToCustomer(int customerId, DateTime date)
        {
            return new CustomerCountryManager().AreEffectiveOrFutureCountriesSoldToCustomer(customerId, date);
        }
    }
}