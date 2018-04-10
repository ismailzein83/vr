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
                _wcfServiceHostingRetries = 1000;
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
            string portNumber;
            return CreateAndOpenTCPServiceHost(serviceType, contractType, out portNumber, onServiceHostCreated, onServiceHostRemoved, out serviceUrl);
        }

        public ServiceHost CreateAndOpenTCPServiceHost(Type serviceType, Type contractType, out string portNumber, Action<ServiceHost> onServiceHostCreated, Action<ServiceHost> onServiceHostRemoved, out string serviceUrl)
        {
            serviceUrl = null;
            ServiceHost serviceHost = null;
            portNumber = null;
            lock (this)
            {
                var random = new Random();
                for (int i = 0; i < _wcfServiceHostingRetries; i++)
                {
                    if (String.IsNullOrEmpty(_servicePortNumber) || i > 0)
                        _servicePortNumber = random.Next(_wcfPortRangeStart, _wcfPortRangeEnd).ToString();
                    bool rethrowIfError = (i == (_wcfServiceHostingRetries - 1));//last iteration
                    serviceHost = CreateAndOpenTCPServiceHost(serviceType, contractType, _servicePortNumber, rethrowIfError, onServiceHostCreated, onServiceHostRemoved, out serviceUrl);
                    portNumber = _servicePortNumber;
                    if (serviceHost != null)
                        break;
                }
            }
            return serviceHost;
        }

        public ServiceHost CreateAndOpenTCPServiceHost(Type serviceType, Type contractType, string portNumber, Action<ServiceHost> onServiceHostCreated, Action<ServiceHost> onServiceHostRemoved, out string serviceUrl)
        {
            return CreateAndOpenTCPServiceHost(serviceType, contractType, portNumber, true, onServiceHostCreated, onServiceHostRemoved, out serviceUrl);
        }

        private ServiceHost CreateAndOpenTCPServiceHost(Type serviceType, Type contractType, string portNumber, bool rethrowIfError, Action<ServiceHost> onServiceHostCreated, Action<ServiceHost> onServiceHostRemoved, out string serviceUrl)
        {
            serviceUrl = BuildTCPServiceURL(serviceType, portNumber);
            ServiceHost serviceHost = new ServiceHost(serviceType);
            serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            try
            {
                var endPoint = AddTCPEndPoint(serviceHost, contractType, serviceUrl);
                if (onServiceHostCreated != null)
                    onServiceHostCreated(serviceHost);
                serviceHost.Open();
                LoggerFactory.GetLogger().WriteInformation("Service URL registered successfully '{0}'", serviceUrl);

            }
            catch (Exception ex)
            {
                if (onServiceHostRemoved != null)
                    onServiceHostRemoved(serviceHost);
                serviceHost = null;
                LoggerFactory.GetLogger().WriteWarning("Could not register Service '{0}'. Error: {1}", serviceUrl, ex);
                if (rethrowIfError)
                    throw;
            }

            return serviceHost;
        }

        public static string BuildTCPServiceURL(Type serviceType, string portNumber)
        {
            return String.Format("net.tcp://{0}:{1}/{2}", Environment.MachineName, portNumber, serviceType.Name);
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
