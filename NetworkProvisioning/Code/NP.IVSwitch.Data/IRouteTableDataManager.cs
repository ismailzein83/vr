using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface IRouteTableDataManager : IDataManager
    {
        List<RouteTable> GetRouteTables();

    }
}
