using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.SupplierPricelist.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceList")]
    public class CP_SupPriceList_PriceListController : BaseAPIController
    {
        [HttpPost]
        [Route("ImportPriceList")]
        public void ImportPriceList(PriceList priceList)
        {
            ImportPriceListManager manager = new ImportPriceListManager();
            bool succeded = manager.Insert(priceList);

        }
        [HttpPost]
        [Route("GetUpdated")]
        public PriceListlUpdateOutput GetUpdated(GetUpdatedInput input)
        {
            ImportPriceListManager manager = new ImportPriceListManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetUpdated(ref maxTimeStamp, input.NbOfRows);
        }
        //[HttpGet]
        //[Route("GetUploadPriceListTemplates")]
        //public List<TemplateConfig> GetUploadPriceListTemplates()
        //{
        //    ImportPriceListManager manager = new ImportPriceListManager();
        //    return manager.GetUploadPriceListTemplates();
        //}
      
        


    }
}