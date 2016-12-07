using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface IRouteDataManager : IDataManager
    {
        List<Route> GetRoutes();
        bool Update(Route route);
        bool Insert(Route route, out int insertedId);
    }
}
