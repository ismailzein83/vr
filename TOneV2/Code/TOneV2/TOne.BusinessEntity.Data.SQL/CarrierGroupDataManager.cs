using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierGroupDataManager : BaseTOneDataManager, ICarrierGroupDataManager 
    {
        public List<CarrierGroup> GetEntities()
        {
            return GetItemsSP("BEntity.sp_CarrierGroup_GetAll", EntityMapper);
        }

        Entities.CarrierGroup EntityMapper(IDataReader reader)
        {
            Entities.CarrierGroup module = new Entities.CarrierGroup
            {
                CarrierGroupID = (int)reader["CarrierGroupID"],
                CarrierGroupName = reader["CarrierGroupName"] as string,
                ParentID =GetReaderValue<int?>(reader,"ParentID"),
                ParentPath = reader["ParentPath"] as string,
                Path = reader["Path"] as string
            };
            return module;
        }

        CarrierAccount CarrierAccountMapper(IDataReader reader)
        {
            return new CarrierAccount
            {
                CarrierAccountId = reader["CarrierAccountId"] as string,
                ProfileName = reader["Name"] as string
            };
        }
        public List<CarrierAccount> GetCarriersByGroup(string groupId)
        {
            return GetItemsSP("BEntity.sp_CarrierGroup_GetCarriers", CarrierAccountMapper, groupId);
        }

        public CarrierGroup GetCarrierGroup(int carrierGroupId)
        {
            return GetItemSP("BEntity.sp_CarrierGroup_GetCarrierGroup", EntityMapper, carrierGroupId);
        }

        public bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, out int insertedId)
        {
            object carrierGroupId;

            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Insert", out carrierGroupId, carrierGroup.CarrierGroupName, carrierGroup.ParentID,
                carrierGroup.ParentPath);

            insertedId = (recordesEffected > 0) ? (Int16)carrierGroupId : -1;
            return (recordesEffected > 0);
        }

         public bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup)
        {
            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Update", carrierGroup.CarrierGroupID, carrierGroup.CarrierGroupName, carrierGroup.ParentID,
                carrierGroup.ParentPath);

            if (recordesEffected > 0)
                return true;
            return false;
        }

    }
}
