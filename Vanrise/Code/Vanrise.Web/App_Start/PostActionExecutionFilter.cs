using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Web.App_Start
{
    public class PostActionExecutionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            Vanrise.Security.Business.ConfigManager configManager = new Vanrise.Security.Business.ConfigManager();
            ReceivedRequestLogSettings receivedRequestLogSettings = configManager.GetReceivedRequestLogSettings();

            if (receivedRequestLogSettings != null && receivedRequestLogSettings.EnableLogging)
            {
                if (actionExecutedContext.Request == null)
                    throw new NullReferenceException("actionExecutedContext.Request");

                var pathWithoutApi = string.Empty;
                var moduleName = ParseRequestModuleName(actionExecutedContext.Request, out pathWithoutApi);
                bool insertLog = false;

                if (receivedRequestLogSettings.Filter == null)
                    insertLog = true;

                else if (receivedRequestLogSettings.Filter != null)
                {
                    var context = new ModuleFilterApplicableContext { ModuleName = moduleName };
                    if (receivedRequestLogSettings.Filter.IsModuleApplicable(context))
                        insertLog = true;
                }

                if (insertLog)
                {
                    var startDateTime = (actionExecutedContext.Request.Properties.GetRecord("MS_HttpContext") as System.Web.HttpContextWrapper).Timestamp;
                    string requestHeader = null;
                    string arguments = null;
                    string requestBody = null;
                    string responseHeader = null;
                    string responseBody = null;

                    if (receivedRequestLogSettings.EnableParametersLogging)
                        arguments = Vanrise.Common.Serializer.Serialize(actionExecutedContext.ActionContext.ActionArguments);

                    if (receivedRequestLogSettings.EnableRequestHeaderLogging)
                        requestHeader = Vanrise.Common.Serializer.Serialize(actionExecutedContext.Request.Headers);

                    if (receivedRequestLogSettings.EnableRequestBodyLogging)
                        requestBody = GetBodyFromRequest(actionExecutedContext.Request);

                    var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                    var method = actionExecutedContext.Request.Method.Method;
                    var controllerName = ParseControllerName(pathWithoutApi, moduleName);
                    var absoluteUri = actionExecutedContext.Request.RequestUri.AbsoluteUri;
                    var absolutePath = actionExecutedContext.Request.RequestUri.AbsolutePath;
                    Vanrise.Security.Entities.ContextFactory.GetContext().TryGetLoggedInUserId(out int? userId);

                    string responseStatusCode = null;
                    bool responseIsSuccessStatusCode = false;
                    if (actionExecutedContext.Response != null)
                    {
                        if (receivedRequestLogSettings.EnableResponseHeaderLogging)
                            responseHeader = Vanrise.Common.Serializer.Serialize(actionExecutedContext.Response.Headers);

                        if (receivedRequestLogSettings.EnableResponseBodyLogging)
                            responseBody = (actionExecutedContext.Response.Content != null) ? actionExecutedContext.Response.Content.ReadAsStringAsync().Result : null;

                        responseStatusCode = actionExecutedContext.Response.StatusCode.ToString();
                        responseIsSuccessStatusCode = actionExecutedContext.Response.IsSuccessStatusCode;
                    }

                    var receivedRequestLogDataManager = CommonDataManagerFactory.GetDataManager<IReceivedRequestLogDataManager>();
                    receivedRequestLogDataManager.Insert(actionName, method, moduleName, controllerName, absoluteUri, absolutePath, requestHeader, arguments, requestBody, responseHeader, responseStatusCode, responseIsSuccessStatusCode, responseBody, startDateTime, userId);
                }
            }

            if (actionExecutedContext.Exception != null)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(actionExecutedContext.Exception);
                if (!new Security.Business.SecurityManager().GetExactExceptionMessage())
                    throw new Exception("Unexpected error occurred. Please consult technical support.");
            }

            try
            {
                if (actionExecutedContext.Response != null && actionExecutedContext.Response.Headers != null)
                    actionExecutedContext.Response.Headers.Add("ServerDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }

        string GetBodyFromRequest(HttpRequestMessage request)
        {
            string data = null;
            if (request.Content != null)
            {
                using (var stream = request.Content.ReadAsStreamAsync().Result)
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }
                    data = request.Content.ReadAsStringAsync().Result;
                }
            }
            return data;
        }

        string ParseRequestModuleName(System.Net.Http.HttpRequestMessage request, out string pathWithoutAPI)
        {
            string absolutePath = request.RequestUri.AbsolutePath;
            pathWithoutAPI = absolutePath.Substring(absolutePath.IndexOf("api/") + 4);
            return pathWithoutAPI.Substring(0, pathWithoutAPI.IndexOf('/'));
        }

        string ParseControllerName(string pathWithoutAPI, string moduleName)
        {
            string pathWithoutModuleName = pathWithoutAPI.Substring(moduleName.Length + 1);
            return pathWithoutModuleName.Substring(0, pathWithoutModuleName.IndexOf('/'));
        }
    }
}