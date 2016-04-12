﻿using ExcelConversion.Business;
using ExcelConversion.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
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
        [HttpPost]
        [Route("ReadExcelFilePage")]
        public ExcelWorksheet ReadExcelFilePage(ExcelPageQuery Query)
        {
            ExcelManager manager = new ExcelManager();
            return manager.ReadExcelFilePage(Query);
        }
        [HttpGet]
        [Route("GetFieldMappingTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetFieldMappingTemplateConfigs()
        {
            ExcelManager manager = new ExcelManager();
            return manager.GetFieldMappingTemplateConfigs();
        }
        [HttpGet]
        [Route("GetConcatenatedPartTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetConcatenatedPartTemplateConfigs()
        {
            ExcelManager manager = new ExcelManager();
            return manager.GetConcatenatedPartTemplateConfigs();
        }

    }
}