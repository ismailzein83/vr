using NP.IVSwitch.Entities;
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
        bool Insert(EndPoint endPoint, List<EndPointInfo> userEndPointInfoList, List<EndPointInfo> aclEndPointInfoList, out int insertedId, string carrierAccountName);
        bool AclUpdate(EndPoint endPoint);
        bool AclInsert(EndPoint endPoint, List<EndPointInfo> userEndPointInfoList, List<EndPointInfo> aclEndPointInfoList, out int insertedId, string carrierAccountName);
        bool SipUpdate(EndPoint endPoint);
        bool SipInsert(EndPoint endPoint, List<EndPointInfo> endPointInfoList, out int insertedId);
    }
}
