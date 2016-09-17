using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Distributor")]
    public class DistributorController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredDistributors")]
        public object GetFilteredDistributors(Vanrise.Entities.DataRetrievalInput<DistributorQuery> input)
        {
            DistributorManager distributorManager = new DistributorManager();
            return GetWebResponse(input, distributorManager.GetFilteredDistributors(input));
        }

        [HttpGet]
        [Route("GetDistributorsInfo")]
        public IEnumerable<DistributorInfo> GetDistributorsInfo()
        {
            DistributorManager distributorManager = new DistributorManager();
            return distributorManager.GetDistributorsInfo(null);
        }

        [HttpPost]
        [Route("AddDistributor")]
        public Vanrise.Entities.InsertOperationOutput<DistributorDetail> AddDistributor(Distributor distributor)
        {
            DistributorManager distributorManager = new DistributorManager();
            return distributorManager.AddDistributor(distributor);
        }

        [HttpPost]
        [Route("UpdateDistributor")]
        public Vanrise.Entities.UpdateOperationOutput<DistributorDetail> UpdateDistributor(Distributor distributor)
        {
            DistributorManager distributorManager = new DistributorManager();
            return distributorManager.UpdateDistributor(distributor);
        }
    }
}