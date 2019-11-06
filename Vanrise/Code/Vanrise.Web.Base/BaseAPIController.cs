﻿using System;
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
            return GetWebResponse<T>(dataRetrievalInput, result, "ExcelReport");
        }

        protected object GetWebResponse<T>(DataRetrievalInput dataRetrievalInput, IDataRetrievalResult<T> result, string resultFileName)
        {
            ExcelResult<T> excelResult = result as ExcelResult<T>;
            if (excelResult != null)
                return GetExcelResponse(excelResult, resultFileName);

            RemoteExcelResult<T> remoteExcelResult = result as RemoteExcelResult<T>;
            if (remoteExcelResult != null && !dataRetrievalInput.IsAPICall)
                return GetExcelResponseFromRemote(remoteExcelResult, resultFileName);

            return result;
        }
        protected object GetExcelResponseFromRemote(RemoteExcelResult remoteExcelResult)
        {
            return GetExcelResponseFromRemote(remoteExcelResult, "ExcelReport");
        }
        protected object GetExcelResponseFromRemote(RemoteExcelResult remoteExcelResult, string resultFileName)
        {
            MemoryStream ms = new MemoryStream(remoteExcelResult.Data);
            return GetExcelResponse(new ExcelResult() { ExcelFileStream = ms }, resultFileName);
        }
        protected object GetExcelResponse(ExcelResult excelResult)
        {
            return GetExcelResponse(excelResult, "ExcelReport");
        }
        protected object GetExcelResponse(ExcelResult excelResult, string resultFileName)
        {
            var fileName = string.Format("{0}.xlsx", resultFileName);
            if (excelResult.ConversionResultType != null)
                return excelResult;
            if (excelResult.ExcelFileContent != null)
                return GetExcelResponse(excelResult.ExcelFileContent, fileName);
            else
                return GetExcelResponse(excelResult.ExcelFileStream, fileName);
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

        protected object GetExcelResponse(byte[] bytes, string fileName)
        {

            MemoryStream stream = new System.IO.MemoryStream();
            if (bytes != null)
                stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(stream, fileName);
        }

        protected HttpResponseMessage GetExceptionResponse(Exception ex)
        {
            var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            response.Content = new StringContent(ex.Message);
            response.Content.Headers.Add("ExceptionMessage", ex.Message);
            return response;
        }

        protected HttpResponseMessage GetUnauthorizedResponse()
        {
            HttpResponseMessage msg = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            msg.Content = new System.Net.Http.StringContent("you are not authorized to perform this request");
            return msg;
        }
    }
}