using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewCallGenService
{
    public class SipAccount1
    {
        private string username = "103";
        private string login = "103";
        private string password = "103";
        private string proxyServer = "192.168.22.22";
        private string server = "192.168.22.22";
        private string displayName = "103";
        private int totalLines = 6;
        private string networkInterface = "Ethernet-IPV4-192.168.22.12";

        public string Username
        {
            get
            {
                return username;
            }
        }
        public string Login
        {
            get
            {
                return login;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
        }
        public string ProxyServer
        {
            get
            {
                return proxyServer;
            }
        }
        public string Server
        {
            get
            {
                return server;
            }
        }
        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }
        public int TotalLines
        {
            get
            {
                return totalLines;
            }
        }
        public string NetworkInterface
        {
            get
            {
                return networkInterface;
            }
        }
    }

}
