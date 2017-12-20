using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.Teles.Data.SQL
{
    public class TelesAccountEnterpriseDataManager : BaseSQLDataManager, ITelesAccountEnterpriseDataManager
    {
     
        #region Constructors
        private const string AccountEnterpriseDID_TABLENAME = "AccountEnterpriseDID";
        public TelesAccountEnterpriseDataManager()
            : base(GetConnectionStringName("Retail_Teles_DBConnStringKey", "RetailTelesDBConnString"))
        {

        }
        #endregion
     
        public void SaveAccountEnterprisesDIDs(List<AccountEnterpriseDID> accountEnterprisesDIDs)
        {
            DataTable accountEnterpriseDIDTable = GetAccountEnterpriseDIDTable();
            foreach (var item in accountEnterprisesDIDs)
            {
                DataRow dr = accountEnterpriseDIDTable.NewRow();
                FillAccountUsageTableRow(dr, item);
                accountEnterpriseDIDTable.Rows.Add(dr);
            }
            accountEnterpriseDIDTable.EndLoadData();
            if (accountEnterpriseDIDTable.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[TelesEnterprise].[sp_AccountEnterpriseDID_Insert]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@AccountEnterpriseDIDTable", SqlDbType.Structured);
                           dtPrm.Value = accountEnterpriseDIDTable;
                           cmd.Parameters.Add(dtPrm);
                       });
        }

        private DataTable GetAccountEnterpriseDIDTable()
        {
            DataTable dt = new DataTable(AccountEnterpriseDID_TABLENAME);
            dt.Columns.Add("AccountId", typeof(long));
            dt.Columns.Add("EnterpriseId", typeof(string));
            dt.Columns.Add("ScreenNumber", typeof(string));
            dt.Columns.Add("Type", typeof(int));
            dt.Columns.Add("MaxCalls", typeof(int));
            return dt;
        }
        private void FillAccountUsageTableRow(DataRow dr, AccountEnterpriseDID accountEnterpriseDID)
        {
            if (accountEnterpriseDID.AccountId.HasValue)
                dr["AccountId"] = accountEnterpriseDID.AccountId;
            dr["EnterpriseId"] = accountEnterpriseDID.EnterpriseId;
            dr["ScreenNumber"] = accountEnterpriseDID.ScreenNumber;
            dr["Type"] = accountEnterpriseDID.Type;
            dr["MaxCalls"] = accountEnterpriseDID.MaxCalls;
        }
    }
}
