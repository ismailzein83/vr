using NP.IVSwitch.Entities;
using NP.IVSwitch.Entities.RouteTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface IEndPointDataManager : IDataManager
    {
        List<EndPoint> GetEndPoints();
        bool Update(EndPoint endPoint);
        bool Insert(EndPoint endPoint, int globalTariffTableId, List<EndPointInfo> userEndPointInfoList, List<EndPointInfo> aclEndPointInfoList, out int insertedId, string carrierAccountName);
        List<AccessList> GetAccessList();
        bool EndPointAclUpdate(IEnumerable<int> endPointIds, int value, RouteTableViewType routeTableViewType,UserType userType  );
        bool RouteTableEndPointUpdate(RouteTableInput routeTableInput, int routeTableId);
    }
}
