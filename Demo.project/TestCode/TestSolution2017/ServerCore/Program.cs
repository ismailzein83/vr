using System;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            StandardOnlyLib.VRTCPCommunication tcpComm = new StandardOnlyLib.VRTCPCommunication();
            tcpComm.StartTCPServer();
            Console.WriteLine("Hello World!");
        }
    }
}
