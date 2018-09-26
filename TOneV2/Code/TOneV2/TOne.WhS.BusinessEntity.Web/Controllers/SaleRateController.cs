using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleRate")]
    public class WhSBE_SaleRateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleRates")]
        public object GetFilteredSaleRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
        {
            SaleRateManager manager = new SaleRateManager();
            return GetWebResponse(input, manager.GetFilteredSaleRates(input), "Sale Rates");
        }
        [HttpGet]
        [Route("GetPrimarySaleEntity")]
        public PrimarySaleEntity GetPrimarySaleEntity()
        {
            var configManager = new ConfigManager();
            return configManager.GetPrimarySaleEntity();
        }
    }

}