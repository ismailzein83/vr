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

        public List<Switch> GetFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            return GetItemsSP("BEntity.SP_Switch_GetFilteredSwitches", SwitchMapper, switchName, rowFrom, rowTo);
        }

        public Switch GetSwitchDetails(int switchID)
        {
            return GetItemsSP("BEntity.SP_Switches_GetSwitch", SwitchMapper, switchID).FirstOrDefault();
        }




        public bool InsertSwitch(Switch switchObject, out int insertedId)
        {
            object switchID;
            int recordesEffected = ExecuteNonQuerySP("[BEntity].[sp_SwitchDefinition_Insert]", out switchID,
            !string.IsNullOrEmpty(switchObject.Name) ? switchObject.Name : null,
               !string.IsNullOrEmpty(switchObject.Symbol) ? switchObject.Symbol : null,
               !string.IsNullOrEmpty(switchObject.Description) ? switchObject.Description : null,
               switchObject.EnableCDRImport ? 1 : 0,
               switchObject.EnableRouting ? 1 : 0,
                switchObject.LastAttempt = DateTime.Now,
                switchObject.LastImport = DateTime.Now
            );
            insertedId = (int)switchID;
            if (recordesEffected > 0)
                return true;
            return false;
        }

        public bool UpdateSwitch(Switch switchObject)
        {
            int recordesEffected = ExecuteNonQuerySP("[BEntity].[sp_SwitchDefinition_Update]",
                 !string.IsNullOrEmpty(switchObject.Name) ? switchObject.Name : null,
               !string.IsNullOrEmpty(switchObject.Symbol) ? switchObject.Symbol : null,
               !string.IsNullOrEmpty(switchObject.Description) ? switchObject.Description : null,
                switchObject.EnableCDRImport,
                switchObject.EnableRouting,
                switchObject.LastAttempt = DateTime.Now,
                switchObject.LastImport = DateTime.Now,
                switchObject.SwitchId
            );
            if (recordesEffected > 0)
                return true;
            return false;
        }

        private Switch SwitchMapper(IDataReader reader)
        {
            return new Entities.Switch
                            {
                                SwitchId = Convert.ToInt32(reader["SwitchID"]),
                                Name = reader["Name"] as string,
                                Symbol = reader["Symbol"] as string,
                                Description = reader["Description"] as string,
                                LastCDRImportTag = reader["LastCDRImportTag"] as string,
                                LastImport = GetReaderValue<DateTime?>(reader, "LastImport"),
                                LastAttempt = GetReaderValue<DateTime?>(reader, "LastAttempt"),
                                EnableCDRImport = GetReaderValue<string>(reader, "Enable_CDR_Import") == "Y" ? true : false,
                                EnableRouting = GetReaderValue<string>(reader, "Enable_Routing") == "Y" ? true : false,
                                LastRouteUpdate = GetReaderValue<DateTime?>(reader, "LastRouteUpdate")
                            };
        }
    }
}
