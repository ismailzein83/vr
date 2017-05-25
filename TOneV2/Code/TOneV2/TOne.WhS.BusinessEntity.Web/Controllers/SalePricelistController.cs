using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Web;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SalePricelist")]
    public class WhS_BE_SalePricelistController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSalePriceLists")]
        public object GetFilteredSalePriceLists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
        {
            SalePriceListManager manager = new SalePriceListManager();
            return GetWebResponse(input, manager.GetFilteredPricelists(input));
        }

        [HttpPost]
        [Route("SendPriceList")]
        public object SendPriceList(object priceListId)
        {
            SalePriceListManager manager = new SalePriceListManager();
            return manager.SendPriceList((long)priceListId);
        }

        [HttpGet]
        [Route("GetSalePriceListIdsByProcessInstanceId")]
        public object GetSalePriceListIdsByProcessInstanceId(long processInstanceId)
        {
            return new SalePriceListManager().GetSalePriceListIdsByProcessInstanceId(processInstanceId);
        }

        [HttpPost]
        [Route("SendCustomerPriceLists")]
        public void SendCustomerPriceLists(IEnumerable<int> customerPriceListIds)
        {
            new SalePriceListManager().SendCustomerPriceLists(customerPriceListIds);
        }
    }
}
