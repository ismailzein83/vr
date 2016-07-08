using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceListTemplate")]
    [JSONWithTypeAttribute]
    public class PriceListTemplateController : BaseAPIController
    {
        private PriceListTemplateManager _manager;
        public PriceListTemplateController()
        {
            this._manager = new PriceListTemplateManager();
        }

        [HttpPost]
        [Route("UpdateInputPriceListTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateInfo> UpdateInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.UpdateInputPriceListTemplate(priceListTemplate);
        }

        [HttpPost]
        [Route("AddInputPriceListTemplate")]
        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateInfo> AddInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.AddInputPriceListTemplate(priceListTemplate);
        }
        [HttpGet]
        [Route("GetPriceListTemplate")]
        public PriceListTemplate GetPriceListTemplate(int priceListTemplateId)
        {
            return _manager.GetPriceListTemplate(priceListTemplateId);
        }
        [HttpGet]
        [Route("GetInputPriceListTemplates")]
        public IEnumerable<PriceListTemplateInfo> GetInputPriceListTemplates(string filter = null)
        {
            PriceListTemplateFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<PriceListTemplateFilter>(filter) : null;
            return _manager.GetInputPriceListTemplates(deserializedFilter);
        }
        [HttpGet]
        [Route("GetInputPriceListConfigurationTemplateConfigs")]
        public IEnumerable<PriceListInputConfig> GetInputPriceListConfigurationTemplateConfigs()
        {
            return _manager.GetInputPriceListConfigurationTemplateConfigs();
        }
    }
}