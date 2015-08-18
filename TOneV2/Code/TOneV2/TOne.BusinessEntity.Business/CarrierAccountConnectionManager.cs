using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
   public class CarrierAccountConnectionManager
    {
       public List<CarrierConnection> GetConnectionByCarrierType(CarrierType type)
       {

           ICarrierAccountConnectionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountConnectionDataManager>();
           List<CarrierConnection> data= dataManager.GetConnectionByCarrierType(type);
           foreach(CarrierConnection itm in data){
               itm.DisplayName = string.Format("[{0}] {1} - {2} : {3} - GW : {4}", itm.SwitchName, itm.ConnectionType, itm.TAG, itm.Value, itm.GateWay);
           }
           return data;
          
       }
    }
}
