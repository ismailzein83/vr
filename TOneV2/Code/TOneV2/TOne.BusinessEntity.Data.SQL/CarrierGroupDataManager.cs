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

        public CarrierGroup GetCarrierGroup(int carrierGroupId)
        {
            return GetItemSP("BEntity.sp_CarrierGroup_GetById", CarrierGroupMapper, carrierGroupId);
        }

        public List<CarrierAccount> GetCarrierGroupMembers(IEnumerable<int> carrierGroupIds)
        {
            DataTable dtCarrierGroupMembers = BuildCarrierAccountInfoTable<int>(carrierGroupIds, "ID");

            return GetItemsSPCmd("[BEntity].[sp_CarrierGroupMember_GetByCarrierGroupIds]",
                  CarrierAccountDataManager.CarrierAccountMapper,
                  (cmd) =>
                  {
                      var dtPrm = new SqlParameter("@CarrierGroupIds", SqlDbType.Structured);
                      dtPrm.Value = dtCarrierGroupMembers;
                      cmd.Parameters.Add(dtPrm);

                  });
        }

        public Vanrise.Entities.BigResult<CarrierAccount> GetCarrierGroupMembers(Vanrise.Entities.DataRetrievalInput<CarrierGroupQuery> input, IEnumerable<int> carrierGroupIds,List<string> filter)
        {
            string carrierGroupIDs = null;
            if (carrierGroupIds.Count() > 0)
                carrierGroupIDs = string.Join<int>(",", carrierGroupIds);
            string assignedCarriers = null;
            if (filter!=null && filter.Count() > 0)
                assignedCarriers = string.Join<string>(",", filter);

            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_CreateTempByCarrierGroupIds", tempTableName, carrierGroupIDs, assignedCarriers);

            }, CarrierAccountDataManager.CarrierAccountMapper);
        }


        internal static DataTable BuildCarrierAccountInfoTable<T>(IEnumerable<T> carrierGroupIds,string column)
        {
            DataTable dtCarrierAccountInfo = new DataTable();
           
                dtCarrierAccountInfo.Columns.Add(column, typeof(T));
                dtCarrierAccountInfo.BeginLoadData();
                foreach (var z in carrierGroupIds)
                {
                    DataRow dr = dtCarrierAccountInfo.NewRow();
                    dr[column] = z;
                    dtCarrierAccountInfo.Rows.Add(dr);
                }
                dtCarrierAccountInfo.EndLoadData();
            
            
            return dtCarrierAccountInfo;
        }

        public bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds, out int insertedId)
        {
            object carrierGroupId;

            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Insert", out carrierGroupId, carrierGroup.Name, carrierGroup.ParentID);

            foreach (string carrAccountId in CarrierAccountIds)
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_Insert", carrierGroupId, carrAccountId);
            }
            insertedId = (recordesEffected > 0) ? (Int16)carrierGroupId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds)
        {
            //update the Carrier group Information and delete the old carrier groups members
            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Update", carrierGroup.ID, carrierGroup.Name, carrierGroup.ParentID);

            //Insert the new carrier groups members
            foreach (string carrAccountId in CarrierAccountIds)
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_Insert", carrierGroup.ID, carrAccountId);
            }

            if (recordesEffected > 0)
                return true;
            return false;
        }

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
