using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public class ServiceHostManager
    {
        #region Singleton

        static ServiceHostManager _current = new ServiceHostManager();

        public static ServiceHostManager Current
        {
            get
            {
                return _current;
            }
        }

        private ServiceHostManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeStart"], out _wcfPortRangeStart))
                _wcfPortRangeStart = 40000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeEnd"], out _wcfPortRangeEnd))
                _wcfPortRangeEnd = 50000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFServiceHostingRetries"], out _wcfServiceHostingRetries))
                _wcfServiceHostingRetries = 100;
        }

        #endregion

        #region Fields

        int _wcfPortRangeStart;
        int _wcfPortRangeEnd;
        int _wcfServiceHostingRetries;

        #endregion

        #region Methods

        string _servicePortNumber;

        public ServiceHost CreateAndOpenTCPServiceHost(Type serviceType, Type contractType, Action<ServiceHost> onServiceHostCreated, Action<ServiceHost> onServiceHostRemoved, out string serviceUrl)
        {
            serviceUrl = null;
            ServiceHost serviceHost = null;
            lock (this)
            {
                var random = new Random();
                for (int i = 0; i < _wcfServiceHostingRetries; i++)
                {
                    if (String.IsNullOrEmpty(_servicePortNumber) || i > 0)
                        _servicePortNumber = random.Next(_wcfPortRangeStart, _wcfPortRangeEnd).ToString();
                    serviceUrl = String.Format("net.tcp://{0}:{1}/{2}", Environment.MachineName, _servicePortNumber, serviceType.Name);
                    serviceHost = new ServiceHost(serviceType);
                    serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
                    serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

                    try
                    {
                        var endPoint = AddTCPEndPoint(serviceHost, contractType, serviceUrl);
                        if (onServiceHostCreated != null)
                            onServiceHostCreated(serviceHost);
                        serviceHost.Open();
                        LoggerFactory.GetLogger().WriteInformation("Service URL registered successfully '{0}'", serviceUrl);
                        break;
                    }
                    catch(Exception ex)
                    {
                        if (onServiceHostRemoved != null)
                            onServiceHostRemoved(serviceHost);
                        LoggerFactory.GetLogger().WriteWarning("Could not register Service '{0}'. Error: {1}", serviceUrl, ex);
                        if (i == (_wcfServiceHostingRetries - 1))//last iteration
                            throw;
                    }
                }
            }
            return serviceHost;
        }

        private ServiceEndpoint AddTCPEndPoint(ServiceHost serviceHost, Type contractType, string serviceUrl)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            var endPoint = serviceHost.AddServiceEndpoint(contractType, binding, serviceUrl);
            return endPoint;
        }

        #endregion
    }
}
