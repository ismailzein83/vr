using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data.SQL
{
    public class BusinessEntityDataManager : BaseTOneDataManager, IBusinessEntityDataManager
    {
        public List<Entities.SwitchInfo> GetSwitches()
        {
            return GetItemsSP("BEntity.SP_Switches_GetSwitches", (reader) =>
            {
                return new Entities.SwitchInfo
                {
                    ID = Convert.ToInt32(reader["SwitchID"]),
                    Name = reader["Name"] as string
                };
            });
        }
    }
}
