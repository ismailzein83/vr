using NP.IVSwitch.Entities.RouteTableRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Data
{
   public interface IRouteTableRouteDataManager:IDataManager
   {
       List<RouteTableRoute> GetRouteTablesRoutes(int routeTableId,int limit,string aNumber,string bNumber);
       bool Insert(List<RouteTableRoute> routeTableRoutes,int routeTableId ,bool IsBlockedAccount);
       RouteTableRoutesToEdit GetRouteTableRoutesOptions(int routeTableId, string destination);
       bool Update(RouteTableRoute routeTableRoute, int routeTableId, bool IsBlockedAccount);
       bool DropRouteTableRoute(int routeTableId);
       bool DeleteRouteTableRoutes(int routeTableId, string destination);
       void CreateRouteTableRoute(int routeTableId);


    }
}
