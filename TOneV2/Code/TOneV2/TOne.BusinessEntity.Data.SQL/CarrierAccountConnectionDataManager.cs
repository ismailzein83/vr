using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierAccountConnectionDataManager : BaseTOneDataManager, ICarrierAccountConnectionDataManager
    {
       public List<CarrierConnection> GetConnectionByCarrierType(CarrierType type)
       {
           return GetItemsSP("BEntity.sp_CarrierAccountConnection_GetByCarrierType", ConnectionMapper, type);

       }

       CarrierConnection ConnectionMapper(IDataReader reader)
       {
           CarrierConnection carrier = new CarrierConnection
           {
               ID = GetReaderValue<int>(reader,"ID"),
               SwitchName = reader["SwitchName"] as string,
               ConnectionType = (ConnectionType)int.Parse(reader["ConnectionType"] as string),
               TAG = reader["TAG"] as string,
               Value = reader["Value"] as string,
               GateWay = reader["GateWay"] as string,
           };

           return carrier;
       }
    }
}
