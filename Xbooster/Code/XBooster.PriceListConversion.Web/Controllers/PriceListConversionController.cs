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
                string priceListName = null;
                if(priceListConversion.InputPriceListName !=null)
                {
                    priceListName =string.Format("{0}_{1}.xls", priceListConversion.InputPriceListName.Trim(), template.Name.Trim());
                }else
                {
                    priceListName=string.Format("{0}.xls",template.Name.Trim());
                }
                return GetExcelResponse(bytes, priceListName);
            }
            catch(Exception ex)
            {
                return GetExceptionResponse(ex);
            }
           
        }
    }
}