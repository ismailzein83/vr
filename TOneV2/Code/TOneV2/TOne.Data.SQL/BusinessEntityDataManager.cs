using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data.SQL
{
    public class BusinessEntityDataManager : BaseTOneDataManager, IBusinessEntityDataManager
    {
        public List<Entities.CarrierInfo> GetCarriers(string carrierType)
        {
            return GetItemsSP("BEntity.sp_Carriers_GetCarriers", (reader) =>
                {
                    return new Entities.CarrierInfo
                    {
                        CarrierAccountID = reader["CarrierAccountID"] as string,
                        Name = string.Format("{0}{1}", reader["Name"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty)
                    };
                }, carrierType);
        }


        public List<Entities.CodeGroupInfo> GetCodeGroups()
        {
            return GetItemsSP("BEntity.SP_CodeGroup_GetCodeGroups", (reader) =>
                {
                    return new Entities.CodeGroupInfo
                    {
                        Code = reader["Code"] as string,
                        Name = reader["Name"] as string
                    };
                });
        }

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
