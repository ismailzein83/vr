using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Vanrise.Entities;

namespace Vanrise.Web.Base
{
    public class BaseAPIController : ApiController
    {
        protected object GetWebResponse<T>(DataRetrievalInput dataRetrievalInput, IDataRetrievalResult<T> result)
        {
            ExcelResult<T> excelResult = result as ExcelResult<T>;
            if (excelResult != null)
                return GetExcelResponse(excelResult);

            return result;
        }

        private object GetExcelResponse<T>(ExcelResult<T> excelResult)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            excelResult.ExcelFileStream.Position = 0;
            response.Content = new StreamContent(excelResult.ExcelFileStream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format("ExcelReport.xls")
            };
            return response;
        }
    }
}