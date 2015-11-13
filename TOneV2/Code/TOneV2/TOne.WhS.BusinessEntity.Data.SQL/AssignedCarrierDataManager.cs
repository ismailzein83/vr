﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class AssignedCarrierDataManager : BaseSQLDataManager, IAssignedCarrierDataManager
    {
        public List<Entities.AssignedCarrier> GetAssignedCarriers(List<int> userIds, CarrierAccountType carrierType)
        {
            DataTable dtMembers = this.BuildUserIdsTable(userIds);
            return GetItemsSPCmd("BEntity.sp_AccountManager_GetAssignedCarriers", AssigendCarrier, (cmd) =>
            {
                var dtPrm = new SqlParameter("@UserIds", SqlDbType.Structured);
                dtPrm.Value = dtMembers;
                cmd.Parameters.Add(dtPrm);

                var carrierTypeParameter = new SqlParameter("@CarrierType", SqlDbType.SmallInt);
                carrierTypeParameter.Value = carrierType;
                cmd.Parameters.Add(carrierTypeParameter);
            });
        }

        public Vanrise.Entities.BigResult<AssignedCarrierFromTempTable> GetAssignedCarriersFromTempTable(Vanrise.Entities.DataRetrievalInput<AssignedCarrierQuery> input, List<int> userIds)
        {
            DataTable dtMembers = this.BuildUserIdsTable(userIds);
            
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySPCmd("BEntity.sp_AccountManager_CreateTempByFiltered", (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TempTableName", tempTableName));

                    cmd.Parameters.Add(new SqlParameter("@ManagerId", input.Query.ManagerId));

                    var dtParameter = new SqlParameter("@UserIds", SqlDbType.Structured);
                    dtParameter.Value = dtMembers;
                    cmd.Parameters.Add(dtParameter);
                });
            };

            return RetrieveData(input, createTempTableAction, AssignedCarrierFromTempTable);
        }

        public bool AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            DataTable table = this.BuildUpdatedCarriersTable(updatedCarriers);

            int recordsEffected = ExecuteNonQuerySPCmd("BEntity.sp_AccountManager_AssignCarriers", (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCarriers", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);
            });

            return recordsEffected > 0;
        }

        private DataTable BuildUpdatedCarriersTable(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            DataTable table = new DataTable();

            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("CarrierAccountId", typeof(string));
            table.Columns.Add("RelationType", typeof(int));
            table.Columns.Add("Status", typeof(bool));

            table.BeginLoadData();
            foreach (var updatedCarrier in updatedCarriers)
            {
                DataRow row = table.NewRow();

                row["UserId"] = updatedCarrier.UserId;
                row["CarrierAccountId"] = updatedCarrier.CarrierAccountId;
                row["RelationType"] = updatedCarrier.RelationType;
                row["Status"] = updatedCarrier.Status;

                table.Rows.Add(row);
            }
            table.EndLoadData();

            return table;
        }

        private DataTable BuildUserIdsTable(List<int> userIds)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.BeginLoadData();

            foreach (var userId in userIds)
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = userId;
                dt.Rows.Add(dr);
            }

            dt.EndLoadData();
            return dt;
        }

        Entities.AssignedCarrier AssigendCarrier(IDataReader reader)
        {
            AssignedCarrier assignedCarrier = new AssignedCarrier
            {
                UserId = (int)reader["UserId"],
                CarrierAccountId = reader["CarrierAccountId"] as string,
                RelationType = (CarrierAccountType)reader["RelationType"]
            };

            return assignedCarrier;
        }

        Entities.AssignedCarrierFromTempTable AssignedCarrierFromTempTable(IDataReader reader)
        {
            AssignedCarrierFromTempTable assignedCarrier = new AssignedCarrierFromTempTable
            {
                CarrierAccountID = reader["CarrierAccountID"] as string,
                IsCustomerAssigned = (bool)reader["IsCustomerAssigned"],
                IsSupplierAssigned = (bool)reader["IsSupplierAssigned"],
                IsCustomerIndirect = (bool)reader["IsCustomerIndirect"],
                IsSupplierIndirect = (bool)reader["IsSupplierIndirect"]
            };

            return assignedCarrier;
        }
    }
}
