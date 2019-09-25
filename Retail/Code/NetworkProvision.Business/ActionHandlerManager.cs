using System;
using System.Collections.Generic;
using System.Text;
using NetworkProvision.Data;
using NetworkProvision.Entities;
using Vanrise.Common;

namespace NetworkProvision.Business
{
    public class ActionHandlerManager
    {
        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        CacheManager GetCacheManager()
        {
            return s_cacheManager;
        }

        public string ExecuteActionGenerateCommand(Dictionary<string, object> input)
        {
            var networkElementTypeObject = input.GetRecord("NetworkElementType");
            string networkElementTypeIdAsString = string.Empty;
            if (networkElementTypeObject != null)
                networkElementTypeObject.ToString();
            Guid
            return "";
        }
        public bool ExecuteActionProvision(Dictionary<string, object> input)
        {
            return false;
        }
        public List<Type> BuildAllActionHandlerControllers()
        {
            List<Type> types = new List<Type>();

            StringBuilder controllersStringBuilder = new StringBuilder(@"
                                using System;
                                using System.Collections.Generic;
                                using System.Web.Http;
                                using Vanrise.BusinessProcess.Business;
                                using Vanrise.BusinessProcess.Entities;
                                using Vanrise.Web.Base;

                                namespace #Namespace#
                                {
                                    [RoutePrefix (""#RoutePrefix#""+""#GenerateCodeControllerName#"")]
                                    public class #GenerateCodeControllerFullName#: BaseAPIController
                                    {
                                        #GenerateCodeMethods#

                                        private System.Net.Http.HttpResponseMessage GetUnauthorizedResponse()
                                        {
                                            System.Net.Http.HttpResponseMessage msg = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                                            msg.Content = new System.Net.Http.StringContent(""you are not authorized to perform this request"");
                                            return msg;
                                        }
                                    }

                                    [RoutePrefix (""#RoutePrefix#""+""#ProvisionControllerName#"")]
                                    public class #ProvisionControllerFullName#: BaseAPIController
                                    {
                                        #ProvisionMethods#

                                        private System.Net.Http.HttpResponseMessage GetUnauthorizedResponse()
                                        {
                                            System.Net.Http.HttpResponseMessage msg = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                                            msg.Content = new System.Net.Http.StringContent(""you are not authorized to perform this request"");
                                            return msg;
                                        }
                                    }
                                }
                                }
                                #GenerateCodeClassMembers#

                                #ProvisionClassMembers#");

            StringBuilder generateCodeMethods = new StringBuilder();
            StringBuilder provisionMethods = new StringBuilder();
            StringBuilder generateCodeClassMembers = new StringBuilder();
            StringBuilder provisionClassMembers = new StringBuilder();

            string inParameterTypeAsString = "Dictionary <string,object>";
            string inParameterName = "input";

            //Temp: should get all actions from data base
            var actions = new List<ProvisionAction>();

            foreach (var action in actions)
            {
                //Add action id to input
                StringBuilder generateCodeMethodBuilder = new StringBuilder(@" 
                [HttpPost]
                [Route(""#MethodName#"")]
                public string #MethodName#(#InParameterType# #InParameterName#)
                {
                    var actionHandlerManager = new NetworkProvision.Business.ActionHandlerManager();
                    return actionHandlerManager.ExecuteActionGenerateCommand(#InParameterName#);
                }
                ");

                StringBuilder provisionMethodBuilder = new StringBuilder(@" 
                [HttpPost]
                [Route(""#MethodName#"")]
                public bool #MethodName#(#InParameterType# #InParameterName#)
                {
                    var actionHandlerManager = new NetworkProvision.Business.ActionHandlerManager();
                    return actionHandlerManager.ExecuteActionProvision(#InParameterName#);
                }
                ");

                generateCodeMethodBuilder.Replace("#MethodName#", action.Name);
                generateCodeMethodBuilder.Replace("#InParameterType#", inParameterTypeAsString);
                generateCodeMethodBuilder.Replace("#InParameterName#", inParameterName);
                generateCodeMethods.Append(generateCodeMethodBuilder);

                provisionMethodBuilder.Replace("#MethodName#", action.Name);
                provisionMethodBuilder.Replace("#InParameterType#", inParameterTypeAsString);
                provisionMethodBuilder.Replace("#InParameterName#", inParameterName);
                provisionMethods.Append(provisionMethodBuilder);

            }

            controllersStringBuilder.Replace("#GenerateCodeMethods#", generateCodeMethods.ToString());
            controllersStringBuilder.Replace("#ProvisionMethods#", provisionMethods.ToString());

            //Read from data base
            List<NetworkElementTypeProvisionHandler> networkElementTypeProvisionHandlers = new List<NetworkElementTypeProvisionHandler>();
            foreach (var handler in networkElementTypeProvisionHandlers)
            {
                StringBuilder generateHandlerNamespaceBuilder = new StringBuilder(@" 
                namespace #namespaceAsString#
                {
                        #handlerCode#
                }
                ");

                StringBuilder provisionHandlerNamespaceBuilder = new StringBuilder(@" 
                namespace #namespaceAsString#
                {
                        #handlerCode#
                }
                ");

                string generateCodeHandlerCode = handler.GenerateCodeHandler.GetCode(new ProvisionHandlerGetCodeContect());
                if (!string.IsNullOrEmpty(generateCodeHandlerCode))
                {
                    var namespaceAsSring = "GenerateCode" + handler.NetworkElementTypeId + handler.ActionId;
                    generateHandlerNamespaceBuilder.Replace("#namespaceAsString#", namespaceAsSring);
                    generateHandlerNamespaceBuilder.Replace("#handlerCode#", generateCodeHandlerCode);
                    generateCodeClassMembers.Append(generateHandlerNamespaceBuilder);
                }

                string provisionHandlerCode = handler.ProvisionHandler.GetCode(new ProvisionHandlerGetCodeContect());
                if (!string.IsNullOrEmpty(provisionHandlerCode))
                {
                    var namespaceAsSring = "Provision" + handler.NetworkElementTypeId + handler.ActionId;
                    provisionHandlerNamespaceBuilder.Replace("#namespaceAsString#", namespaceAsSring);
                    provisionHandlerNamespaceBuilder.Replace("#handlerCode#", provisionHandlerCode);
                    provisionClassMembers.Append(provisionHandlerNamespaceBuilder);
                }
            }

            controllersStringBuilder.Replace("#GenerateCodeClassMembers#", generateCodeClassMembers.ToString());
            controllersStringBuilder.Replace("#ProvisionClassMembers#", provisionClassMembers.ToString());

            Vanrise.Common.CSharpCompilationOutput output;
            if (Vanrise.Common.CSharpCompiler.TryCompileClass(controllersStringBuilder.ToString(), out output))
            {
                types.AddRange(output.OutputAssembly.GetTypes());
            }

            return types;
        }

        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IActionHandlerDataManager actionHandlerDataManager = NetworkProvisionDataManagerFactory.GetDataManager<IActionHandlerDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return actionHandlerDataManager.AreActionHandlersUpdated(ref _updateHandle);
            }
        }
        #endregion
    }

    public interface IExecuteActionProvisionContext
    {
    }

    public interface IExecuteActionGenerateCommandContext
    {
    }
}
