using System;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierTargetMatch")]
    public class SupplierTargetMatchController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSupplierTargetMatches")]
        public object GetFilteredSupplierTargetMatches(Vanrise.Entities.DataRetrievalInput<SupplierTargetMatchQuery> input)
        {
            SupplierTargetMatchManager manager = new SupplierTargetMatchManager();
            return GetWebResponse(input, manager.GetFilteredSupplierTargetMatches(input), "Supplier Target Matches");
        }

        [HttpGet]
        [Route("GetTargetMatchMethods")]
        public IEnumerable<SupplierTargetMatchMethodConfig> GetTargetMatchMethods()
        {
            var manager = new SupplierTargetMatchManager();
            return manager.GetTargetMatchMethodConfigs();
        }


        //[HttpGet]
        //[Route("DownloadSupplierTargetMatches")]
        //public object DownloadSupplierTargetMatches()
        //{
        //    byte[] fileContent = new SupplierTargetMatchManager().DownloadSupplierTargetMatches();
        //    return GetExcelResponse(fileContent, "Supplier Target Match.xls");
        //}
    }
}