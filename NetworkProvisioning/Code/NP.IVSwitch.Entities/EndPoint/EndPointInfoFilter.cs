using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class EndPointInfoFilter
    {
        public int? AssignableToCarrierAccountId { get; set; } // Account Id in postgres
        public List<int> CustomerIds { get; set; }
        public IEnumerable<IEndPointFilter> Filters { get; set; }
        public int? RouteTableId { get; set; }

    }
    public interface IEndPointFilter
    {
        bool IsMatched(IEndPointFilterContext context);
    }
    public interface IEndPointFilterContext
    {
        EndPoint EndPoint { get; set; }
    }
    public class EndPointFilterContext : IEndPointFilterContext
    {
        public EndPoint EndPoint { get; set; }

    }

}
