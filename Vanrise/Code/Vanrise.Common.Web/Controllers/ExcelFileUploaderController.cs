using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common.Business.ExcelFileUploader;

namespace Vanrise.Common.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "ExcelFileUploader")]
    [JSONWithTypeAttribute]
    public class ExcelFileUploaderController : BaseAPIController
    {
        ExcelFileUploaderManager excelFileUploaderManager = new ExcelFileUploaderManager();

        [HttpPost]
        [Route("UploadExcelFile")]
        public ExcelUploaderOutput UploadExcelFile(ExcelUploaderInput input)
        {
            return excelFileUploaderManager.UploadExcelFile(input);
        }

    }
}