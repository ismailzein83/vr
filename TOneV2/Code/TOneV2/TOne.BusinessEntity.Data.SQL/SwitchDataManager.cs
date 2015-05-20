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
                    Name = reader["Name"] as string//,
                    //Symbol = reader["Symbol"] as string,
                    //Description = reader["Description"] as string,
                    //LastCDRImportTag = reader["LastCDRImportTag"] as string,
                    //LastImport = GetReaderValue<DateTime?>(reader, "LastImport"),
                    //LastAttempt = GetReaderValue<DateTime?>(reader, "LastAttempt"),
                    //Enable_CDR_Import = GetReaderValue<char>(reader, "Enable_CDR_Import") == 'Y' ? true : false,
                    //Enable_Routing = GetReaderValue<char>(reader, "Enable_Routing") == 'Y' ? true : false,
                    //LastRouteUpdate = GetReaderValue<DateTime?>(reader, "LastRouteUpdate")
                };
            });
        }
    }
}
