using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data.SQL
{
    public class CarrierDataManager : BaseTOneDataManager, ICarrierDataManager
    {
        public List<Entities.CarrierInfo> GetCarriers()
        {
            return GetItemsSP("BEntity.sp_Carriers_GetCarriers", (reader) =>
                {
                    return new Entities.CarrierInfo
                    {
                        CarrierAccountID = reader["CarrierAccountID"] as string,
                        Name = reader["Name"] as string
                    };
                });
        }
    }
}
