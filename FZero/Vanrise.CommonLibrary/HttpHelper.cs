using System;
using System.Net.Sockets;    

namespace Vanrise.CommonLibrary
{
    public static class HttpHelper
    {
        public static bool CheckInternetConnection(string URL, int port)
        {
            bool Success = false;

            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(URL, port);
                Success = true;
            }
            catch (Exception)
            {

                Success = false;
            }

            return Success;
        }
    }

}
