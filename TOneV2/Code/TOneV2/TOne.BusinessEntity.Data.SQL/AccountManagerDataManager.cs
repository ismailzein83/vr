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
    class AccountManagerDataManager : BaseTOneDataManager, IAccountManagerDataManager
    {
        public List<AccountManagerCarrier> GetCarriers(int from, int to)
        {
            return GetItemsSP("BEntity.sp_AccountManager_GetCarriers", (reader) =>
            {
                return new AccountManagerCarrier
                {
                    CarrierAccountId = reader["CarrierAccountId"] as string,
                    Name = reader["Name"] as string,
                    NameSuffix = reader["NameSuffix"] as string,
                    IsCustomerAvailable = (bool)reader["IsCustomerAvailable"],
                    IsSupplierAvailable = (bool)reader["IsSupplierAvailable"]
                };
            }, from, to);
        }

        public List<AssignedAccountManagerCarrier> GetAssignedCarriers(int userId)
        {
            return GetItemsSP("BEntity.sp_AccountManager_GetAssignedCarriers", (reader) =>
            {
                return new AssignedAccountManagerCarrier
                {
                    CarrierAccountId = reader["CarrierAccountId"] as string,
                    RelationType = (int)reader["RelationType"]
                };
            }, userId);
        }

        public void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            DataTable table = this.BuildUpdatedCarriersTable(updatedCarriers);

            ExecuteNonQuerySPCmd("BEntity.sp_AccountManager_AssignCarriers", (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCarriers", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);
            });
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
    }
}
