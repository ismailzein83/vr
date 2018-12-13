using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRDynamicCode
{
    public class HttpProxyDynamicCodeSettings : VRDynamicCodeSettings
    {
        public override Guid ConfigId { get { return new Guid("5F14D26D-7B43-41BE-9A3A-6BA0A7EB8316"); } }
        public Guid ConnectionId { get; set; }
        public string ClassName { get; set; }
        public List<HttpProxyMethod> Methods { get; set; }
        public string FreeCode { get; set; }
        public override string Generate(IVRDynamicCodeSettingsContext context)
        {
            StringBuilder methodsBuilder = new StringBuilder();
            if (Methods!=null && Methods.Count > 0)
            {
                foreach(var method in Methods)
                {
                    if (methodsBuilder.Length > 0)
                        methodsBuilder.AppendLine();
                    methodsBuilder.Append(method.Generate(new HttpProxyMethodContext { ConnectionId = ConnectionId}));
                }
            }

            StringBuilder classBuilder = new StringBuilder(@"
                public class #CLASSNAME#
                {
                    #METHODS#
                }

                #FREECODE#
            ");
            classBuilder.Replace("#CLASSNAME#", ClassName);
            classBuilder.Replace("#METHODS#", methodsBuilder.ToString());
            if (FreeCode != null)
            {
                classBuilder.Replace("#FREECODE#", FreeCode);
            }
            else
            {
                classBuilder.Replace("#FREECODE#", "");
            }
            return classBuilder.ToString();
        }
    }
    public class HttpProxyMethod
    {
        public string MethodName { get; set; }
        public VRHttpMethod MethodType { get; set; }
        public VRHttpMessageFormat MessageFormat { get; set; }
        public string ActionPath { get; set; }
        public string ReturnType { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<HttpProxyParameter> Parameters { get; set; }
        public Dictionary<string, string> URLParameters { get; set; }
        public string ResponseLogic { get; set; }
        public string Generate(IHttpProxyMethodContext context)
        {
            StringBuilder methodParametersBuilder = new StringBuilder();
            StringBuilder bodyParametersBuilder = new StringBuilder();
            StringBuilder bodyParameterNamesBuilder = new StringBuilder();
            List<string> headerParameters = new List<string> ();
            List<string> urlParameters = new List<string>();

            bool doesBodyParameterExist = false;
            string bodyParameter = null;

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    if (methodParametersBuilder.Length > 0)
                    {
                        methodParametersBuilder.Append(", ");
                    }
                    methodParametersBuilder.Append(parameter.ParameterType);
                    methodParametersBuilder.Append(" ");
                    methodParametersBuilder.Append(parameter.ParameterName);

                    if (bodyParametersBuilder.Length > 0)
                    {
                        bodyParametersBuilder.Append(", ");
                    }
                    bodyParametersBuilder.Append(parameter.ParameterType);
                    bodyParametersBuilder.Append(" ");
                    bodyParametersBuilder.Append(parameter.ParameterName);

                    if (bodyParameterNamesBuilder.Length > 0)
                    {
                        bodyParameterNamesBuilder.Append(", ");
                    }
                    bodyParameterNamesBuilder.Append(parameter.ParameterName);

                    if (parameter.IncludeInBody)
                    {
                        doesBodyParameterExist = true;
                        bodyParameter = parameter.ParameterName;
                    }
                    if (parameter.IncludeInHeader)
                    {
                        headerParameters.Add(parameter.ParameterName);
                    }
                    if (parameter.IncludeInURL)
                    {
                        urlParameters.Add(parameter.ParameterName);
                    }
                }
            }

            StringBuilder functionBuilder = new StringBuilder(@" 

                public #RETURNTYPE# #METHODNAME#(#PARAMETERS#)
                {
                   Vanrise.Common.Business.VRConnectionManager connectionManager = new Vanrise.Common.Business.VRConnectionManager();
                   var vrConnection = connectionManager.GetVRConnection<Vanrise.Common.Business.VRHttpConnection>(#CONNECTIONID#);
                   Vanrise.Common.Business.VRHttpConnection connectionSettings = vrConnection.Settings as Vanrise.Common.Business.VRHttpConnection;

                   Dictionary<string, string> headers = new Dictionary<string, string>();
                   #BUILDHEADERS#

                   Dictionary<string, string> urlParameters = new Dictionary<string, string>();
                   #BUILDURLPARAMETERS#
                
                   object result=null;
                   connectionSettings.TrySendRequest(#ACTIONPATH#, Vanrise.Entities.VRHttpMethod.#HTTPMETHOD#,Vanrise.Entities.VRHttpMessageFormat.#MESSAGEFORMAT#,urlParameters,headers,#BODY#, (response)=>{
                    
                   #RESULTVALUE#

                   },true, (error)=>{});
                   #RETURN#
                } 

                #BUILDBODYMETHOD#

                #BUILDRESPONSEHANDLER#

            ");
            functionBuilder.Replace("#METHODNAME#", MethodName);
            functionBuilder.Replace("#PARAMETERS#", methodParametersBuilder.ToString());
            var connectionId = string.Format("new Guid(\"{0}\")", context.ConnectionId.ToString());
            functionBuilder.Replace("#CONNECTIONID#", connectionId);
            var actionPath = string.Format("\"{0}\"", ActionPath);
            functionBuilder.Replace("#ACTIONPATH#", actionPath);
            functionBuilder.Replace("#HTTPMETHOD#", MethodType.ToString());
            functionBuilder.Replace("#MESSAGEFORMAT#", MessageFormat.ToString());
            if (ReturnType!=null && !ReturnType.Equals("void"))
            {
                StringBuilder resultBuilder = new StringBuilder(@"
                    return (#RETURNTYPE#)result;
                ");
                resultBuilder.Replace("#RETURNTYPE#", ReturnType);
                functionBuilder.Replace("#RETURNTYPE#", ReturnType);
                functionBuilder.Replace("#RETURN#", resultBuilder.ToString());
                if (ResponseLogic != null)
                {
                    StringBuilder reponseHandlerBuilder = new StringBuilder(@"#RETURNTYPE# GetResponseHandler_#METHODNAME#(Vanrise.Common.Business.VRHttpResponse response){
                    #RESPONSEHANDLERLOGIC#
                    }");
                    reponseHandlerBuilder.Replace("#RETURNTYPE#", ReturnType);
                    reponseHandlerBuilder.Replace("#METHODNAME#", MethodName);
                    reponseHandlerBuilder.Replace("#RESPONSEHANDLERLOGIC#", this.ResponseLogic);
                    functionBuilder.Replace("#BUILDRESPONSEHANDLER#", reponseHandlerBuilder.ToString());
                    functionBuilder.Replace("#RESULTVALUE#", string.Format("result = GetResponseHandler_{0}(response);", MethodName));

                }
                else
                {
                    functionBuilder.Replace("#RESULTVALUE#", string.Format("result = response.DeserializeResponse<{0}>();", ReturnType));
                    functionBuilder.Replace("#BUILDRESPONSEHANDLER#", "");
                }
            }
            else
            {
                functionBuilder.Replace("#RETURN#", "");
                functionBuilder.Replace("#RETURNTYPE#", "void");
                functionBuilder.Replace("#RESULTVALUE#", "");
                functionBuilder.Replace("#BUILDRESPONSEHANDLER#", "");
            }

            StringBuilder urlParametersBuilder = new StringBuilder();
            if(this.URLParameters != null && this.URLParameters.Count > 0)
            {
                foreach (var prm in this.URLParameters)
                {
                    urlParametersBuilder.AppendLine(String.Concat("urlParameters.Add(\"", prm.Key, "\", \"", prm.Value, "\");"));
                }
            }
            if(urlParameters!=null && urlParameters.Count > 0)
            {
                foreach(var prm in urlParameters)
                {
                    urlParametersBuilder.AppendLine(String.Concat("urlParameters.Add(\"", prm, "\", ", prm, ");"));
                }
            }

            functionBuilder.Replace("#BUILDURLPARAMETERS#", urlParametersBuilder.ToString());

            StringBuilder headersBuilder = new StringBuilder();
            if(this.Headers != null && this.Headers.Count > 0)
            {
                foreach (var header in this.Headers)
                {
                    headersBuilder.AppendLine(String.Concat("headers.Add(\"", header.Key, "\", \"", header.Value, "\");"));
                }
            }
              
            if(headerParameters != null && headerParameters.Count > 0)
            {
                foreach(var header in headerParameters)
                {
                    headersBuilder.AppendLine(String.Concat("headers.Add(\"", header, "\",", header, ");"));
                }
            }
            functionBuilder.Replace("#BUILDHEADERS#", headersBuilder.ToString());
            if(MethodType == VRHttpMethod.Post)
            {
                if (doesBodyParameterExist && bodyParameter != null)
                {
                    functionBuilder.Replace("#BODY#", string.Format("Vanrise.Common.Serializer.Serialize({0})", bodyParameter));
                    functionBuilder.Replace("#BUILDBODYMETHOD#", " ");
                }
                else
                {
                    functionBuilder.Replace("#BODY#", string.Format("GetBody_{0}({1})", MethodName, bodyParameterNamesBuilder.ToString()));

                    StringBuilder bodyFunction = new StringBuilder(@"string GetBody_#METHODNAME#(#BODYPARAMETERS#){
                    #BODYLOGIC#
                    }");
                    bodyFunction.Replace("#METHODNAME#", MethodName);
                    bodyFunction.Replace("#BODYPARAMETERS#", bodyParametersBuilder.ToString());
                    bodyFunction.Replace("#BODYLOGIC#", this.Body);
                    functionBuilder.Replace("#BUILDBODYMETHOD#", bodyFunction.ToString());
                }
            }
            else
            {
                functionBuilder.Replace("#BODY#", "null");
                functionBuilder.Replace("#BUILDBODYMETHOD#", "");
            }
            return functionBuilder.ToString();
        }
    }

    public interface IHttpProxyMethodContext
    {
        Guid ConnectionId { get; }
    }
    public class HttpProxyMethodContext : IHttpProxyMethodContext
    {
        public Guid ConnectionId { get; set; }
    }
    public class HttpProxyParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType{ get; set; }
        public bool IncludeInHeader { get; set; }
        public bool IncludeInBody { get; set; }
        public bool IncludeInURL { get; set; }
    }
}
