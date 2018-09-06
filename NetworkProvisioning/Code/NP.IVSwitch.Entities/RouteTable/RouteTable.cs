using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities.RouteTable
{
    public class RouteTable
    {
        public int RouteTableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PScore { get; set; }
    }
    public class RouteTableInput
    {
        public RouteTableViewType RouteTableViewType { get; set; }
        public RouteTable RouteTable { get; set; }
        public List<RouteTableEndPoint> EndPoints { get; set; }
    }
    public class RuntimeEditorEntity
    {
        public RouteTableInput RouteTableInput { get; set; }
        public List<EndPointCarrierAccount> EndPointCarrierAccount { get; set; }
    }
    public class EndPointCarrierAccount
    {
        public int EndPointId { get; set; }
        public int? CarrierAccount { get; set; }
    }
    public class RouteTableEndPoint
    {
        public int EndPointId { get; set; }
        // public Int16 CLIRouting { get; set; }
    }
    public class RouteEndPoints
    {
        public RouteTable RouteTable { get; set; }
        public List<EndPoint> EndPoints { get; set; }
    }

}
