using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Web.Base;
using XBooster.PriceListConversion.Business;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceListConversion")]
    [JSONWithTypeAttribute]
    public class PriceListConversionController : BaseAPIController
    {
        [HttpPost]
        [Route("ConvertAndDownloadPriceList")]
        public object ConvertAndDownloadPriceList(PriceListConversionInput priceListConversion)
        {
            PriceListConversionManager manager = new PriceListConversionManager();
            try
            {
                byte[] bytes  = manager.ConvertAndDownloadPriceList(priceListConversion);
                PriceListTemplateManager templateManager = new PriceListTemplateManager();
                PriceListTemplate template =  templateManager.GetPriceListTemplate(priceListConversion.OutputPriceListTemplateId);
                return GetExcelResponse(bytes,template.Name.Trim() +".xls");
            }
            catch(Exception ex)
            {
                return GetExceptionResponse(ex);
            }
           
        }
    }
}