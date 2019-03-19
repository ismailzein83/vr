using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StandardOnlyLib
{
    public class VRTCPCommunication
    {
        static int s_wcfPortRangeStart;
        static int s_wcfPortRangeEnd;
        static int s_wcfServiceHostingRetries;

        static VRTCPCommunication()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeStart"], out s_wcfPortRangeStart))
                s_wcfPortRangeStart = 40000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeEnd"], out s_wcfPortRangeEnd))
                s_wcfPortRangeEnd = 50000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFServiceHostingRetries"], out s_wcfServiceHostingRetries))
                s_wcfServiceHostingRetries = 1000;
        }
        public void StartTCPServer()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 3434);
            tcpListener.Start();
            while (true)
            {
                //Socket socket = tcpListener.AcceptSocket();
                //byte[] receivedBytes = new byte[1000000];
                //int receivedBytesCount = socket.Receive(receivedBytes);

                //string receivedValue = System.Text.Encoding.UTF8.GetString(receivedBytes, 0, receivedBytesCount);
                //Console.WriteLine($"Received: {receivedValue}");

                //string returnedValue = $"Server received {receivedValue} at {DateTime.Now}";
                //socket.Send(Encoding.UTF8.GetBytes(returnedValue));
                //socket.Close();

                //Stream s = new NetworkStream(socket);
                //StreamReader sr = new StreamReader(s);
                //StreamWriter sw = new StreamWriter(s);
                //sw.AutoFlush = true;
                //string receivedValue = sr.ReadToEnd();
                //Console.WriteLine($"Received: {receivedValue}");

                //string returnedValue = $"Server received {receivedValue} at {DateTime.Now}";
                //sw.Write(returnedValue);
                //s.Close();
                //socket.Close();

                //Console.WriteLine("Press Any Key to start listening...");
                //Console.ReadKey();
                //Console.WriteLine("Started Listening");


                using (TcpClient tcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream s = tcpClient.GetStream())
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            using (StreamWriter sw = new StreamWriter(s))
                            {
                                sw.AutoFlush = true; string receivedValue = sr.ReadLine();
                                Console.WriteLine($"Received: {receivedValue}");

                                string returnedValue = $"Server received {receivedValue} at {DateTime.Now}";
                                //System.Threading.Thread.Sleep(3000);
                                sw.WriteLine(returnedValue);
                            }
                        }
                        s.Close();
                    }
                }
            }
        }

        public void ConnectToServer()
        {
            while (true)
            {
                Console.WriteLine("Write any message");
                string line1 = Console.ReadLine();
                Console.WriteLine("Enter Number of Times");
                int nbOfTimes = int.Parse(Console.ReadLine());
                string line = "";
                for(int i =0;i<nbOfTimes;i++)
                {
                    line += line1 ;
                }
                DateTime start = DateTime.Now;
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(Environment.MachineName, 3434);
                    using (Stream s = client.GetStream())
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            using (StreamWriter sw = new StreamWriter(s))
                            {
                                sw.AutoFlush = true;
                                
                                sw.WriteLine(line);
                                Console.WriteLine($"Received From Server: {sr.ReadLine()}. time to complete is {DateTime.Now - start}");
                            }
                        }
                        s.Close();
                    }
                    client.Close();
                }
            }
        }

        static Dictionary<string, Object> s_servicesByName = new Dictionary<string, object>();

        public static void CreateAndOpenTCPServiceHost(Type serviceType, Type contractType, bool rethrowIfError, out string serviceUrl)
        {            
            StartTCPServiceIfNotStarted();
            string serviceName = string.Concat(serviceType.FullName, "/", contractType.FullName);
            serviceUrl = string.Concat(Environment.MachineName, ":", s_portNumber, ":", serviceName);
            lock (s_servicesByName)
            {
                s_servicesByName.Add(serviceName, Activator.CreateInstance(serviceType));
            }
        }

        static Object s_lockObj = new object();
        static TcpListener s_tcpListener;
        static string s_portNumber;

        private static void StartTCPServiceIfNotStarted()
        {
            if (s_tcpListener == null)
            {
                lock (s_lockObj)
                {
                    if(s_tcpListener == null)
                    {
                        var random = new Random();
                        for (int i = 0; i < s_wcfServiceHostingRetries; i++)
                        {
                            int portNumber = random.Next(s_wcfPortRangeStart, s_wcfPortRangeEnd);
                            bool rethrowIfError = (i == (s_wcfServiceHostingRetries - 1));//last iteration
                            s_tcpListener = new TcpListener(IPAddress.Any, portNumber);
                            s_tcpListener.Start();
                            s_portNumber = portNumber.ToString();
                            OpenThreadToListenToTCPRequests();
                            break;
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
                                sw.AutoFlush = true; string receivedValue = sr.ReadLine();
                                Console.WriteLine($"Received: {receivedValue}");



                                string returnedValue = $"Server received {receivedValue} at {DateTime.Now}";
                                //System.Threading.Thread.Sleep(3000);
                                sw.WriteLine(returnedValue);
                            }
                        }
                        s.Close();
                    }
                }
            });
            t.Start();
        }
    }

    public class VRTCPRequest
    {
        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public object[] Arguments { get; set; }
    }
}
