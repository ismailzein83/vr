using ExcelConversion.Business;
using ExcelConversion.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace ExcelConversion.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "Excel")]
    [JSONWithTypeAttribute]
    public class ExcelConversion_ExcelController : BaseAPIController
    {
        [HttpGet]
        [Route("ReadExcelFile")]
        public ExcelWorkbook ReadExcelFile(int fileId) 
        {
            ExcelManager manager = new ExcelManager();
            return manager.ReadExcelFile(fileId);
        }

    }
}