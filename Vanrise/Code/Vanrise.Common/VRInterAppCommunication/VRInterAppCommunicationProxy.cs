using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public interface IVRInterAppCommunicationProxy
    {
        void SendRequest(string methodName, params object[] parameters);
        T SendRequest<T>(string methodName, params object[] parameters);
    }

    public class VRInterAppCommunicationProxy : IVRInterAppCommunicationProxy
    {
        string _serviceURL;
        string _machineName;
        int _portNumber;
        string _serviceName;

        public VRInterAppCommunicationProxy(string serviceURL)
        {
            serviceURL.ThrowIfNull("serviceURL");
            _serviceURL = serviceURL;
            string[] parts = serviceURL.Split(':');
            if (parts.Length != 3)
                throw new Exception($"Invalid Service URL '{serviceURL}'");
            _machineName = parts[0];
            if (!int.TryParse(parts[1], out _portNumber))
                throw new Exception($"Invalid Port Number '{parts[1]}'");
            _serviceName = parts[2];
        }

        internal void Connect()
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(_machineName, _portNumber);
                if (!SendRequest<bool>("VRInterAppCommunicationServiceManager", "PrivateSendRequest", this._serviceName))
                    throw new Exception($"Service '{this._serviceName}' not registered on machine '{_machineName}' port number '{_portNumber}'");
                tcpClient.Close();
            }
        }

        public T SendRequest<T>(string methodName, params Object[] parameters)
        {
            var serializedResponse = PrivateSendRequest(methodName, true, parameters);
            return serializedResponse != null ? Serializer.Deserialize<T>(serializedResponse) : default(T);
        }

        public void SendRequest(string methodName, params Object[] parameters)
        {
            PrivateSendRequest(methodName, false, parameters);
        }

        string PrivateSendRequest(string methodName, bool withResponse, params Object[] parameters)
        {
            return PrivateSendRequest(_serviceName, methodName, withResponse, parameters);
        }

        string PrivateSendRequest(string serviceName, string methodName, bool withResponse, params Object[] parameters)
        {
            VRTCPRequest tcpRequest = new VRTCPRequest
            {
                ServiceName = serviceName,
                MethodName = methodName,
                Parameters = parameters
            };
            string response = null;
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(_machineName, _portNumber);
                using (var stream = tcpClient.GetStream())
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            streamWriter.AutoFlush = true;
                            streamWriter.WriteLine(Serializer.Serialize(tcpRequest));
                            string serializedResponse = streamReader.ReadLine();
                            VRTCPResponse tcpResponse = Serializer.Deserialize<VRTCPResponse>(serializedResponse);
                            tcpResponse.ThrowIfNull("tcpResponse");
                            if (tcpResponse.IsSucceeded)
                            {
                                if (withResponse)
                                {
                                    response = tcpResponse.Response;
                                }
                            }
                            else
                            {
                                throw new Exception($"Error when calling method '{methodName}' on serviceUrl '{_serviceURL}'. Error is '{tcpResponse.Response}'");
                            }
                        }
                    }
                    stream.Close();
                }
                tcpClient.Close();
            }
            return response;
        }
    }
}
