using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExcelFileParser")]
    public class VRCommon_ExcelFileParserController : BaseAPIController
    {
        [HttpGet]
        [Route("GetUploadedDataValues")]
        public IEnumerable<string> GetUploadedDataValues(long fileId, ExcelFileValueType type)
        {
            return new ExcelFileParserManger().GetUploadedDataValues(fileId, type);
        }

        [HttpGet]
        [Route("DowloadFileExcelParserTemplate")]
        public object DowloadFileExcelParserTemplate(string fieldName)
        {
            ExcelFileParserManger manager = new ExcelFileParserManger();
            byte[] bytes = manager.DowloadFileExcelParserTemplate(fieldName);
            return GetExcelResponse(bytes, string.Format("{0}-Template.xlsx", fieldName));
        }

    }
}