using ExcelConversion.Business;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace ExcelConversion.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PriceListConversion")]
    [JSONWithTypeAttribute]
    public class PriceListConversionController : BaseAPIController
    {
        [HttpPost]
        [Route("PriceListConvertAndDownload")]
        public object PriceListConvertAndDownload(PriceListConversion priceListConversion)
        {
            PriceListConversionManager manager = new PriceListConversionManager();
            byte[] bytes = manager.PriceListConvertAndDownload(priceListConversion);
            return GetExcelResponse(bytes, "ConvertedExcel.xls");
        }
        [HttpPost]
        [Route("ConvertAndDownload")]
        public object ConvertAndDownload(ExcelToConvert excelToConvert)
        {
            PriceListConversionManager manager = new PriceListConversionManager();
            byte[] bytes = manager.ConvertAndDownload(excelToConvert);
            return GetExcelResponse(bytes, "ConvertedExcel.xls");
        }
    }
}