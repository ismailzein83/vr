using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public enum ConnectionTypes { WIFI = 0, Ethernet = 1, EthernetFiber = 2 }
    public class DataService : PackageServiceSettings
    {
        public ConnectionTypes ConnectionType { get; set; }

      //  public LineType LineType { get; set; } 
        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }
    }
    public abstract class LineType
    {
        public int ConfigId { get; set; }
    }
    public abstract class ConnectionType
    {
        public int ConfigId { get; set; }
    }
   
}
