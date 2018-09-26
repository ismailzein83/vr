using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "PricingTemplate")]
    public class PricingTemplateController : BaseAPIController
    {
        PricingTemplateManager manager = new PricingTemplateManager();

        [HttpPost]
        [Route("GetFilteredPricingTemplates")]
        public object GetFilteredPricingTemplates(DataRetrievalInput<PricingTemplateQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredPricingTemplates(input), "Pricing Templates");
        }

        [HttpGet]
        [Route("GetPricingTemplateEditorRuntime")]
        public PricingTemplateEditorRuntime GetPricingTemplateEditorRuntime(int pricingTemplateId)
        {
            return manager.GetPricingTemplateEditorRuntime(pricingTemplateId);
        }

        [HttpPost]
        [Route("AddPricingTemplate")]
        public InsertOperationOutput<PricingTemplateDetail> AddPricingTemplate(PricingTemplate pricingTemplate)
        {
            return manager.AddPricingTemplate(pricingTemplate);
        }

        [HttpPost]
        [Route("UpdatePricingTemplate")]
        public UpdateOperationOutput<PricingTemplateDetail> UpdatePricingTemplate(PricingTemplateToEdit pricingTemplateToEdit) 
        {
            return manager.UpdatePricingTemplate(pricingTemplateToEdit);
        }

        [HttpGet]
        [Route("GetMarginRateCalculationExtensionConfigs")]
        public IEnumerable<MarginRateCalculationConfig> GetMarginRateCalculationExtensionConfigs()
        {
            return manager.GetMarginRateCalculationExtensionConfigs();
        }
    }
}