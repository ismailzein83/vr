using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using XBooster.PriceListConversion.Business;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceListTemplate")]
    [JSONWithTypeAttribute]
    public class PriceListTemplateController:BaseAPIController
    {
        private PriceListTemplateManager _manager;
        public PriceListTemplateController()
        {
            this._manager = new PriceListTemplateManager();
        }
       
        [HttpPost]
        [Route("GetFilteredPriceListTemplates")]
        public object GetFilteredPriceListTemplates(Vanrise.Entities.DataRetrievalInput<PriceListTemplateQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredPriceListTemplates(input));
        }
    }
}