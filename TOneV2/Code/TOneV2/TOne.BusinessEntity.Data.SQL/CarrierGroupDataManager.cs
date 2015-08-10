﻿using System;
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
        public CarrierGroup GetCarrierGroup(int carrierGroupId)
        {
            return GetItemSP("BEntity.sp_CarrierGroup_GetById", CarrierGroupMapper, carrierGroupId);
        }

        public List<CarrierAccount> GetCarrierGroupMembers(IEnumerable<int> carrierGroupIds)
        {
            DataTable dtCarrierGroupMembers = BuildCarrierAccountInfoTable(carrierGroupIds);

            return GetItemsSPCmd("[BEntity].[sp_CarrierGroupMember_GetByCarrierGroupIds]",
                  CarrierAccountDataManager.CarrierAccountMapper,
                  (cmd) =>
                  {
                      var dtPrm = new SqlParameter("@CarrierGroupIds", SqlDbType.Structured);
                      dtPrm.Value = dtCarrierGroupMembers;
                      cmd.Parameters.Add(dtPrm);

                  });
        }

        internal static DataTable BuildCarrierAccountInfoTable(IEnumerable<int> carrierGroupIds)
        {
            DataTable dtCarrierAccountInfo = new DataTable();
            dtCarrierAccountInfo.Columns.Add("ID", typeof(int));
            dtCarrierAccountInfo.BeginLoadData();
            foreach (var z in carrierGroupIds)
            {
                DataRow dr = dtCarrierAccountInfo.NewRow();
                dr["ID"] = z;
                dtCarrierAccountInfo.Rows.Add(dr);
            }
            dtCarrierAccountInfo.EndLoadData();
            return dtCarrierAccountInfo;
        }

        public bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds, out int insertedId)
        {
            object carrierGroupId;

            int carrGroupId = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Insert", out carrierGroupId, carrierGroup.Name, carrierGroup.ParentID);

            foreach (string carrAccountId in CarrierAccountIds)
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_Insert", carrGroupId, carrAccountId);
            }
            insertedId = (carrGroupId > 0) ? (Int16)carrierGroupId : -1;
            return (carrGroupId > 0);
        }

        //public bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup)
        //{
        //    int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Update", carrierGroup.CarrierGroupID, carrierGroup.CarrierGroupName, carrierGroup.ParentID,
        //        carrierGroup.ParentPath);

        //    if (recordesEffected > 0)
        //        return true;
        //    return false;
        //}

        Entities.CarrierGroup CarrierGroupMapper(IDataReader reader)
        {
            Entities.CarrierGroup module = new Entities.CarrierGroup
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                ParentID = GetReaderValue<int?>(reader, "ParentID")
            };
            return module;
        }



    }
}
