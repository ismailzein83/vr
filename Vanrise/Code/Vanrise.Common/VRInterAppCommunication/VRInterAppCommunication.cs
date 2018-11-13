using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class VRInterAppCommunication
    {
        #region Static

        internal const string INTERAPPCOMMUNICATION_SERVICE_MANAGER = "VRInterAppCommunicationServiceManager";

        static int s_tcpPortRangeStart;
        static int s_tcpPortRangeEnd;
        static int s_tcpServiceHostingRetries;

        static Dictionary<string, VRCommunicationRegisteredServiceInfo> s_registeredServicesByName = new Dictionary<string, VRCommunicationRegisteredServiceInfo>();

        static Dictionary<Type, Type> s_ProxyTypesByContractTypes = new Dictionary<Type, Type>();

        static Object s_lockObj = new object();
        static TcpListener s_tcpListener;
        static string s_portNumber;

        static VRInterAppCommunication()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["TCPPortRangeStart"], out s_tcpPortRangeStart))
                s_tcpPortRangeStart = 40000;
            if (!int.TryParse(ConfigurationManager.AppSettings["TCPPortRangeEnd"], out s_tcpPortRangeEnd))
                s_tcpPortRangeEnd = 50000;
            if (!int.TryParse(ConfigurationManager.AppSettings["TCPServiceHostingRetries"], out s_tcpServiceHostingRetries))
                s_tcpServiceHostingRetries = 1000;            
        }

        #endregion

        #region Public Methods

        public static void RegisterService(Type serviceType, Type contractType, out string serviceUrl)
        {
            if (!contractType.IsInterface)
                throw new Exception($"contractType '{contractType}' is not an Interface");
            if (!contractType.IsAssignableFrom(serviceType))
                throw new Exception($"serviceType '{serviceType}' doesnt implement contractType '{contractType}'");
            StartTCPServiceIfNotStarted();
            string serviceName = string.Concat(serviceType.FullName, "/", Guid.NewGuid());
            serviceUrl = string.Concat(Environment.MachineName, ":", s_portNumber, ":", serviceName);
            AddServiceToRegisteredServices(serviceType, contractType, serviceName);
        }

        
        public static bool TryCreateServiceClient<T>(string serviceURL, Action<T> onClientReady) where T : class
        {
            T proxy = null;
            try
            {
                proxy = GetProxy<T>(serviceURL);
                proxy.CastWithValidate<VRInterAppCommunicationProxy>("proxy").Connect();
            }
            catch(Exception ex)
            {
                LoggerFactory.GetLogger().WriteWarning("cannot connect to Service. Service URL '{0}'. Error '{1}'", serviceURL, ex.ToString());
                return false;
            }

            onClientReady(proxy);
            return true;
        }

        public static void CreateServiceClient<T>(string serviceURL, Action<T> onClientReady) where T : class
        {
            T proxy = GetProxy<T>(serviceURL);
            onClientReady(proxy);
        }



        #endregion

        #region Private Methods

        private static void AddServiceToRegisteredServices(Type serviceType, Type contractType, string serviceName)
        {
            lock (s_registeredServicesByName)
            {
                VRCommunicationRegisteredServiceInfo serviceInfo = new VRCommunicationRegisteredServiceInfo
                {
                    ServiceInstance = Activator.CreateInstance(serviceType),
                    Methods = new Dictionary<string, MethodInfo>()
                };
                foreach (var methodInfo in contractType.GetMethods())
                {
                    if (serviceInfo.Methods.ContainsKey(methodInfo.Name))
                        throw new Exception($"duplicate method names not supported. Method Name '{methodInfo.Name}'");
                    serviceInfo.Methods.Add(methodInfo.Name, methodInfo);
                }
                s_registeredServicesByName.Add(serviceName, serviceInfo);
            }
        }

        private static void StartTCPServiceIfNotStarted()
        {
            if (s_tcpListener == null)
            {
                lock (s_lockObj)
                {
                    if (s_tcpListener == null)
                    {
                        if (!s_registeredServicesByName.ContainsKey(INTERAPPCOMMUNICATION_SERVICE_MANAGER))
                            AddServiceToRegisteredServices(typeof(VRInterAppCommunicationServiceManager), typeof(IVRInterAppCommunicationServiceManager), INTERAPPCOMMUNICATION_SERVICE_MANAGER);
                        var random = new Random();
                        for (int i = 0; i < s_tcpServiceHostingRetries; i++)
                        {
                            int portNumber = random.Next(s_tcpPortRangeStart, s_tcpPortRangeEnd);
                            bool rethrowIfError = (i == (s_tcpServiceHostingRetries - 1));//last iteration
                            try
                            {
                                s_tcpListener = new TcpListener(IPAddress.Any, portNumber);
                                s_tcpListener.Start();
                                s_portNumber = portNumber.ToString();
                                OpenThreadToListenToTCPRequests();
                                LoggerFactory.GetLogger().WriteInformation("InterApp Communication Service registered successfully. Machine Name '{0}' Port Number '{1}'", Environment.MachineName, portNumber);
                                break;
                            }
                            catch(Exception ex)
                            {
                                LoggerFactory.GetLogger().WriteWarning("Could not register InterApp Communication Service. Machine Name '{0}' Port Number '{1}. Error: {2}", Environment.MachineName, portNumber, ex);
                                if (rethrowIfError)
                                    throw;
                            }
                        }
                    }
                }
            }
        }

        private static void OpenThreadToListenToTCPRequests()
        {
            Task t = new Task(() =>
            {
                using (TcpClient tcpClient = s_tcpListener.AcceptTcpClient())
                {
                    OpenThreadToListenToTCPRequests();
                    using (NetworkStream s = tcpClient.GetStream())
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            using (StreamWriter sw = new StreamWriter(s))
                            {
                                sw.AutoFlush = true;
                                string receivedValue = sr.ReadLine();
                                VRTCPResponse response = new VRTCPResponse();
                                string serializedTCPResponse = null;
                                try
                                {
                                    VRTCPRequest tcpRequest = Serializer.Deserialize(receivedValue).CastWithValidate<VRTCPRequest>("tcpRequest");
                                    VRCommunicationRegisteredServiceInfo registeredService;
                                    if (!s_registeredServicesByName.TryGetValue(tcpRequest.ServiceName, out registeredService))
                                        throw new Exception($"Service '{tcpRequest.ServiceName}' not registered");
                                    MethodInfo methodInfo;
                                    if (!registeredService.Methods.TryGetValue(tcpRequest.MethodName, out methodInfo))
                                        throw new Exception($"Method '{tcpRequest.MethodName}' not found in service '{tcpRequest.ServiceName}'");
                                    var returnedValue = methodInfo.Invoke(registeredService.ServiceInstance, tcpRequest.Parameters);
                                    if (methodInfo.ReturnType != typeof(void))
                                    {
                                        response.Response = Serializer.Serialize(returnedValue);   
                                    }
                                    response.IsSucceeded = true;
                                    serializedTCPResponse = Serializer.Serialize(response);
                                }
                                catch (Exception ex)
                                {
                                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                                    response.IsSucceeded = false;
                                    response.Response = ex.ToString();
                                    serializedTCPResponse = Serializer.Serialize(response);
                                }
                                sw.WriteLine(serializedTCPResponse);
                                sw.Close();
                            }
                            sr.Close();
                        }
                        s.Close();
                    }
                }
            });
            t.Start();
        }

        private static T GetProxy<T>(string serviceUrl) where T : class
        {
            Type interfaceType = typeof(T);
            Type proxyType;
            if(!s_ProxyTypesByContractTypes.TryGetValue(interfaceType, out proxyType))
            {
                lock(s_ProxyTypesByContractTypes)
                {
                    if(!s_ProxyTypesByContractTypes.TryGetValue(interfaceType, out proxyType))
                    {
                        StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Common.VRInterAppCommunicationProxy, #INTERFACEFQTN#
                    {   
                        public #CLASSNAME#(string serviceUrl) : base(serviceUrl)
                        {
                        }

                        #METHODS#
                    }
                }
                ");

                        classDefinitionBuilder.Replace("#INTERFACEFQTN#", interfaceType.FullName);

                        StringBuilder methodsBuilder = new StringBuilder();

                        foreach (var methodInfo in interfaceType.GetMethods())
                        {
                            methodsBuilder.Append("public ");
                            if (methodInfo.ReturnType == typeof(void))
                                methodsBuilder.Append("void");
                            else
                                methodsBuilder.Append(CSharpCompiler.TypeToString(methodInfo.ReturnType));
                            methodsBuilder.Append($" {methodInfo.Name}(");
                            bool isFirstParameter = true;
                            foreach (var parameterInfo in methodInfo.GetParameters())
                            {
                                if (!isFirstParameter)
                                    methodsBuilder.Append(", ");
                                isFirstParameter = false;
                                methodsBuilder.Append($"{CSharpCompiler.TypeToString(parameterInfo.ParameterType)} {parameterInfo.Name}");
                            }
                            methodsBuilder.Append(")");
                            methodsBuilder.AppendLine();
                            methodsBuilder.Append("{");
                            methodsBuilder.AppendLine();
                            if (methodInfo.ReturnType != typeof(void))
                                methodsBuilder.Append("return ");
                            methodsBuilder.Append("base.SendRequest");
                            if (methodInfo.ReturnType != typeof(void))
                                methodsBuilder.Append($"<{CSharpCompiler.TypeToString(methodInfo.ReturnType)}>");
                            methodsBuilder.Append($"(\"{methodInfo.Name}\"");
                            
                            foreach (var parameterInfo in methodInfo.GetParameters())
                            {
                                methodsBuilder.Append(", ");
                                methodsBuilder.Append(parameterInfo.Name);
                            }
                            methodsBuilder.Append(");");
                            methodsBuilder.AppendLine();
                            methodsBuilder.Append("}");
                            methodsBuilder.AppendLine();
                        }
                        classDefinitionBuilder.Replace("#METHODS#", methodsBuilder.ToString());

                        string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Common");
                        string className = interfaceType.Name.Substring(1);
                        classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                        classDefinitionBuilder.Replace("#CLASSNAME#", className);
                        string fullTypeName = String.Format("{0}.{1}", classNamespace, className);
                        CSharpCompilationOutput compilationOutput;
                        if (!CSharpCompiler.TryCompileClass(classDefinitionBuilder.ToString(), out compilationOutput))
                        {
                            StringBuilder errorsBuilder = new StringBuilder();
                            if (compilationOutput.ErrorMessages != null)
                            {
                                foreach (var errorMessage in compilationOutput.ErrorMessages)
                                {
                                    errorsBuilder.AppendLine(errorMessage);
                                }
                            }
                            throw new Exception(String.Format("Compile Error when building executor type for PropValueReader. Errors: {0}",
                                errorsBuilder));
                        }
                        proxyType = compilationOutput.OutputAssembly.GetType(fullTypeName);
                        proxyType.ThrowIfNull("proxyType");
                        s_ProxyTypesByContractTypes.Add(interfaceType, proxyType);
                    }
                }
            }
            return Activator.CreateInstance(proxyType, serviceUrl).CastWithValidate<T>("proxyObj");
        }

        #endregion

        #region Private Classes

        private class VRCommunicationRegisteredServiceInfo
        {
            public Object ServiceInstance { get; set; }

            public Dictionary<string, MethodInfo> Methods { get; set; }

        }

        private interface IVRInterAppCommunicationServiceManager
        {
            bool IsServiceRegistered(string serviceName);
        }


        private class VRInterAppCommunicationServiceManager : IVRInterAppCommunicationServiceManager
        {
            public bool IsServiceRegistered(string serviceName)
            {
                return s_registeredServicesByName.ContainsKey(serviceName);
            }
        }

        #endregion
    }

    public class VRTCPRequest
    {
        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public object[] Parameters { get; set; }
    }

    public class VRTCPResponse
    {
        public bool IsSucceeded { get; set; }

        public string Response { get; set; }
    }
}
