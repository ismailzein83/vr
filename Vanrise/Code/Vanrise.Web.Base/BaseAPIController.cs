using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
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

        protected object GetExcelResponse(ExcelResult excelResult)
        {
            return GetExcelResponse(excelResult.ExcelFileStream, "ExcelReport.xls");
        }

        protected object GetExcelResponse(Stream stream, string fileName)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            stream.Position = 0;
            response.Content = new StreamContent(stream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlPathEncode(fileName)
            };
            return response;
        }
        protected object GetExcelResponse(byte[] bytes,string fileName)
        {
            MemoryStream stream = new System.IO.MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(stream,fileName);
        }
    }
}