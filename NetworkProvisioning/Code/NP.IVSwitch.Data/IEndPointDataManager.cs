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
        bool Insert(EndPoint endPoint, int globalTariffTableId, List<EndPointInfo> userEndPointInfoList, List<EndPointInfo> aclEndPointInfoList, out int insertedId, string carrierAccountName);
        List<AccessList> GetAccessList();
    }
}
