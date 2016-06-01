using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public enum ConnectionType { WIFI = 0, Ethernet = 1, EthernetFiber = 2 }
    public class DataService : PackageService
    {
        public ConnectionType ConnectionType { get; set; }
        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }
    }
}
