using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities;

namespace NP.IVSwitch.Data
{
    public interface ICustomerRouteDataManager : IDataManager
    {
        List<CustomerRoute> GetCustomerRoutes(List<EndPointInfo> acls, int top, string orderBy, string codePrefix);
    }
}
