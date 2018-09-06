using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities;
namespace NP.IVSwitch.Business
{
    public class EndPointViewFilter : IEndPointFilter
    {
        public RouteTableViewType RouteTableViewType { get; set; }
        public int? RouteTableId { get; set; }
        public bool IsMatched(IEndPointFilterContext context)
        {
            if (RouteTableViewType == Entities.RouteTableViewType.ANumber)
                return (context.EndPoint.CliRouting == 0 || (RouteTableId.HasValue && RouteTableId.Value == context.EndPoint.CliRouting));
            else
                return (context.EndPoint.DstRouting == 0 || (RouteTableId.HasValue && RouteTableId.Value == context.EndPoint.DstRouting));
        }
    }
}
