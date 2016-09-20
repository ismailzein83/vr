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
        [Route("UpdateOutputPriceListTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail> UpdateOutputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.UpdateOutputPriceListTemplate(priceListTemplate);
        }

        [HttpPost]
        [Route("AddOutputPriceListTemplate")]
        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail> AddOutputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            return _manager.AddOutputPriceListTemplate(priceListTemplate);
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
        public IEnumerable<OutputPriceListConfigurationTemplateConfig> GetOutputPriceListConfigurationTemplateConfigs()
        {
            return _manager.GetOutputPriceListConfigurationTemplateConfigs();
        }
        [HttpGet]
        [Route("GetPriceListTemplate")]
        public PriceListTemplate GetPriceListTemplate(int priceListTemplateId)
        {
            return _manager.GetPriceListTemplate(priceListTemplateId);
        }
        [HttpGet]
        [Route("GetOutputPriceListTemplates")]
        public IEnumerable<PriceListTemplateInfo> GetOutputPriceListTemplates(string filter = null)
        {
            PriceListTemplateFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<PriceListTemplateFilter>(filter) : null;
            return _manager.GetOutputPriceListTemplates(deserializedFilter);
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
        public IEnumerable<InputPriceListConfigurationTemplateConfig> GetInputPriceListConfigurationTemplateConfigs()
        {
            return _manager.GetInputPriceListConfigurationTemplateConfigs();
        }
        [HttpGet]
        [Route("GetOutputFieldMappingTemplateConfigs")]
        public IEnumerable<OutputFieldMappingTemplateConfig> GetOutputFieldMappingTemplateConfigs()
        {
            return _manager.GetOutputFieldMappingTemplateConfigs();
        }
    }
}