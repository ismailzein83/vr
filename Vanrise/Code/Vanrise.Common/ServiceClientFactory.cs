using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class ServiceClientFactory
    {
        static TimeSpan _pingServiceTimeOutInterval;

        static ServiceClientFactory()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["ServiceClientFactory_PingServiceTimeOutInterval"], out _pingServiceTimeOutInterval))
                _pingServiceTimeOutInterval = TimeSpan.FromMilliseconds(500);
        }

        public static bool TryCreateTCPServiceClient<T>(string serviceURL, Action<T> onClientReady) where T : class
        {
            IChannel channel = CreateTCPChannel<T>(serviceURL);
            try
            {
                channel.Open(_pingServiceTimeOutInterval);
            }
            catch
            {
                LoggerFactory.GetLogger().WriteWarning("cannot connect to Service '{0}'. Service URL '{1}'", typeof(T).FullName, serviceURL);
                return false;
            }
            T client = channel as T;
            try
            {
                onClientReady(client);
            }
            finally
            {
                try
                {
                    (client as IDisposable).Dispose();
                }
                catch
                {

                }
            }
            return true;
        }

        public static void CreateTCPServiceClient<T>(string serviceURL, Action<T> onClientReady) where T : class
        {
            IChannel channel = CreateTCPChannel<T>(serviceURL);

            T client = channel as T;
            try
            {
                onClientReady(client);
            }
            finally
            {
                try
                {
                    (client as IDisposable).Dispose();
                }
                catch
                {

                }
            }
        }

        private static IChannel CreateTCPChannel<T>(string serviceURL) where T : class
        {
            Binding binding;

            binding = new NetTcpBinding(SecurityMode.None)
            {
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };

            binding.OpenTimeout = TimeSpan.FromMinutes(5);
            binding.CloseTimeout = TimeSpan.FromMinutes(5);
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            
            ChannelFactory<T> channelFactory = new ChannelFactory<T>(binding, new EndpointAddress(serviceURL));
            IChannel channel = channelFactory.CreateChannel() as IChannel;
            return channel;
        }


    }
}
