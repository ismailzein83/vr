using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class AccountManagerCarrierDataManager : BaseTOneDataManager, IAccountManagerCarrierDataManager
    {
        #region Fields / Constructors

        public AccountManagerCarrierDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "MainDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<AssignedCarrier> GetAssignedCarriers()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_AccountManager_GetAll]", AssignedCarrierMapper);
        }
        public bool AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            DataTable table = this.BuildUpdatedCarriersTable(updatedCarriers);

            int recordsEffected = ExecuteNonQuerySPCmd("[TOneWhS_BE].[sp_AccountManager_AssignCarriers]", (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCarriers", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);
            });

            return recordsEffected > 0;
        }
        public bool AreAssignedCarriersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.AccountManager", ref updateHandle);
        }

        #endregion

        #region Private Methods

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
        
        #endregion

        #region  Mappers

        AssignedCarrier AssignedCarrierMapper(IDataReader reader)
        {
            AssignedCarrier accountManager = new AssignedCarrier
            {
                CarrierAccountId = GetReaderValue<int>(reader, "CarrierAccountId"),
                UserId = GetReaderValue<int>(reader, "UserId"),
                RelationType = (CarrierAccountType)reader["RelationType"]
            };

            return accountManager;
        }

        #endregion
    }
}
