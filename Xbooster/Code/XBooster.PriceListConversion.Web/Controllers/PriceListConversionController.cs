using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Web.Base;
using XBooster.PriceListConversion.Business;

namespace XBooster.PriceListConversion.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceListConversion")]
    [JSONWithTypeAttribute]
    public class PriceListConversionController : BaseAPIController
    {
        [HttpPost]
        [Route("PriceListConvertAndDownload")]
        public object PriceListConvertAndDownload(Entities.PriceListConversion priceListConversion)
        {
            PriceListConversionManager manager = new PriceListConversionManager();
            byte[] bytes = manager.PriceListConvertAndDownload(priceListConversion);
            return GetExcelResponse(bytes, "ConvertedExcel.xls");
        }
    }
}