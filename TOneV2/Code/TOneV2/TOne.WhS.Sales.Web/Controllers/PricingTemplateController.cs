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
            return GetWebResponse(input, manager.GetFilteredPricingTemplates(input));
        }

        [HttpGet]
        [Route("GetPricingTemplate")]
        public PricingTemplate GetPricingTemplate(int pricingTemplateId)
        {
            return manager.GetPricingTemplate(pricingTemplateId);
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
    }
}