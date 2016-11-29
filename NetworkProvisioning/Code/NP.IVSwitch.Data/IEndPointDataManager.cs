using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface IEndPointDataManager:IDataManager
    {
        List<EndPoint> GetEndPoints();

        bool  Update(EndPoint endPoint);
        bool  Insert(EndPoint endPoint, out int insertedId);

        bool AclUpdate(EndPoint endPoint);
        bool AclInsert(EndPoint endPoint, out int insertedId);

        bool SipUpdate(EndPoint endPoint);
        bool SipInsert(EndPoint endPoint, out int insertedId);
 

         void CheckTariffAndRouteTables(EndPoint  endPoint ,  string carrierAccountName);
    }
}
