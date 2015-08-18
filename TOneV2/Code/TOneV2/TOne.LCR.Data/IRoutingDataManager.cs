using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;
using TOne.LCR.Entities.Routing;

namespace TOne.LCR.Data
{
    public interface IRoutingDataManager : IDataManager
    {
        int DatabaseId { get; set; }
        RoutingDatabaseType RoutingDatabaseType { set; }
    }
}
