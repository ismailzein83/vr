using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Contract")]
    [JSONWithTypeAttribute]
    public class ContractController : BaseAPIController
    {
        ContractManager contractManager = new ContractManager();

        [HttpGet]
        [Route("GetContract")]
        public Contract GetContract()
        {
            return contractManager.GetContract();
        }
    }
}
