using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDataManager : IDataManager
    {
        int DatabaseId { get; set; }
        RoutingDatabaseType RoutingDatabaseType { set; }
    }
}
