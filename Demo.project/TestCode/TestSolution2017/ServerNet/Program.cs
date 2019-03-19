using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerNet
{
    class Program
    {
        static void Main(string[] args)
        {
            //StandardOnlyLib.VRTCPCommunication tcpComm = new StandardOnlyLib.VRTCPCommunication();
            //tcpComm.StartTCPServer();
            string serviceURL;
            Vanrise.Common.VRInterAppCommunication.RegisterService(typeof(TestService), typeof(ITestService), out serviceURL);
            using (var sw = new StreamWriter(@"C:\Test\ServiceURLs.txt", true))
            {
                sw.WriteLine(serviceURL);
            }
            Console.WriteLine("SERVER: Service URL is: {0}", serviceURL);
            Console.ReadKey();
        }


    }

    public class TestService : ITestService
    {
        public void DoSomething(string value)
        {
            Console.WriteLine("Received Value: {0}", value);
        }

        public double Multiply(double v1, double v2)
        {
            if (v2 > 100)
                throw new Exception("v2 > 100");
            return v1 * v2;
        }

        public string PostInput(ServiceInput input)
        {
          //  Console.WriteLine($"PostInput: {input.StringProp}");
            return $"Modified - {input.StringProp}";
        }
    }

    public class ServiceInput
    {
        public string StringProp { get; set; }
    }

    public interface ITestService
    {
        void DoSomething(string value);

        double Multiply(double v1, double v2);

        string PostInput(ServiceInput input);
    }

    public interface ITestService2
    {
        void DoSomething(string value);

        double Multiply(double v1, double v2);
    }
}
