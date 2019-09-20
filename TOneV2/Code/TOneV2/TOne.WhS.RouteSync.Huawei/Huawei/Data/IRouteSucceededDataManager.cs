using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei.Data
{
    public interface IRouteSucceededDataManager : IDataManager
    {
        string SwitchId { get; set; } 
        void SaveRoutesSucceededToDB(Dictionary<string, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN);
    }
}