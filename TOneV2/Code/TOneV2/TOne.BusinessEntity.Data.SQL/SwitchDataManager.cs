using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : BaseTOneDataManager, ISwitchDataManager
    {
        public List<SwitchInfo> GetSwitches()
        {
            return GetItemsSP("BEntity.SP_Switches_GetSwitches", (reader) =>
            {
                return new Entities.SwitchInfo
                {
                    SwitchId = Convert.ToInt32(reader["SwitchID"]),
                    Name = reader["Name"] as string
                };
            });
        }
    }
}
