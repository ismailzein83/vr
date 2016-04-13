using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
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
        [Route("GetFilteredInputPriceListTemplates")]
        public object GetFilteredPriceListTemplates(Vanrise.Entities.DataRetrievalInput<PriceListTemplateQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredInputPriceListTemplates(input));
        }

        [HttpPost]
        [Route("UpdateInputPriceListTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail> UpdateInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.UpdateInputPriceListTemplate(priceListTemplate);
        }

        [HttpPost]
        [Route("AddInputPriceListTemplate")]
        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail> AddInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.AddInputPriceListTemplate(priceListTemplate);
        }
        [HttpGet]
        [Route("GetOutputPriceListConfigurationTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetOutputPriceListConfigurationTemplateConfigs()
        {
            return _manager.GetOutputPriceListConfigurationTemplateConfigs();
        }
        [HttpGet]
        [Route("GetPriceListTemplate")]
        public PriceListTemplate GetPriceListTemplate(int priceListTemplateId)
        {
            return _manager.GetPriceListTemplate(priceListTemplateId);
        }
    }
}