using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Web.Base;

namespace Vanrise.ExcelConversion.Web
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Excel")]
    [JSONWithTypeAttribute]
    public class ExcelController:BaseAPIController
    {
        [HttpGet]
        [Route("ReadExcelFile")]
        public ExcelWorkbook ReadExcelFile(int fileId)
        {
            ExcelManager manager = new ExcelManager();
            return manager.ReadExcelFile(fileId);
        }
        [HttpPost]
        [Route("ReadExcelFilePage")]
        public ExcelWorksheet ReadExcelFilePage(ExcelPageQuery Query)
        {
            ExcelManager manager = new ExcelManager();
            return manager.ReadExcelFilePage(Query);
        }
        [HttpGet]
        [Route("GetFieldMappingTemplateConfigs")]
        public IEnumerable<FieldMappingConfig> GetFieldMappingTemplateConfigs()
        {
            ExcelManager manager = new ExcelManager();
            return manager.GetFieldMappingTemplateConfigs();
        }
        [HttpGet]
        [Route("GetConcatenatedPartTemplateConfigs")]
        public IEnumerable<ConcatenatedPartConfig> GetConcatenatedPartTemplateConfigs()
        {
            ExcelManager manager = new ExcelManager();
            return manager.GetConcatenatedPartTemplateConfigs();
        }
    }
}