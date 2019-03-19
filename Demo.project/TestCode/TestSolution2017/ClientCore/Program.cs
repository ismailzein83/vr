using System;

namespace ClientCore
{
    class Program
    {
        static void Main(string[] args)
        {
            StandardOnlyLib.VRTCPCommunication tcpComm = new StandardOnlyLib.VRTCPCommunication();
            tcpComm.ConnectToServer();
            Console.WriteLine("Hello World!");
        }
    }
}
