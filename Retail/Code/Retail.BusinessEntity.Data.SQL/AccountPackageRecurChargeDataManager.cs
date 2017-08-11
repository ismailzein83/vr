using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using Retail.BusinessEntity.Entities;
using System.Data.SqlClient;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountPackageRecurChargeDataManager : BaseSQLDataManager, IAccountPackageRecurChargeDataManager
    {

        #region Constructors

        public AccountPackageRecurChargeDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<AccountPackageRecurCharge> GetAccountPackageRecurChargesNotSent(DateTime effectiveDate)
        {
            return GetItemsSP("Retail_BE.sp_AccountPackageRecurCharge_GetNotSentData", AccountPackageRecurChargeMapper);
        }

        public void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime effectiveDate, long processInstanceId)
        {
            DataTable dtAccountPackageRecurCharge = BuildAccountPackageReccuringChargeTable(accountPackageRecurChargeList);
            int recordsEffected = ExecuteNonQuerySPCmd("[Retail_BE].[sp_AccountPackageRecurCharge_InsertOrUpdate]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@EffectiveDate", effectiveDate));
                cmd.Parameters.Add(new SqlParameter("@ProcessInstanceID", processInstanceId));
                var dtPrm = new SqlParameter("@AccountPackageRecurCharges", SqlDbType.Structured);
                dtPrm.Value = dtAccountPackageRecurCharge;
                cmd.Parameters.Add(dtPrm);
            });
        }

        #endregion

        #region Private Methods

        DataTable BuildAccountPackageReccuringChargeTable(List<AccountPackageRecurCharge> accountPackageRecurChargeList)
        {
            DataTable dtAccountPackageRecurCharge = GetAccountPackageRecurChargeTable();
            dtAccountPackageRecurCharge.BeginLoadData();
            foreach (var accountPackageRecurCharge in accountPackageRecurChargeList)
            {
                DataRow dr = dtAccountPackageRecurCharge.NewRow();
                dr["AccountPackageID"] = accountPackageRecurCharge.AccountPackageID;
                dr["ChargeableEntityID"] = accountPackageRecurCharge.ChargeableEntityID;

                if (accountPackageRecurCharge.BalanceAccountTypeID.HasValue)
                    dr["BalanceAccountTypeID"] = accountPackageRecurCharge.BalanceAccountTypeID.Value;
                else
                    dr["BalanceAccountTypeID"] = DBNull.Value;

                dr["BalanceAccountID"] = accountPackageRecurCharge.BalanceAccountID;
                dr["ChargeDay"] = accountPackageRecurCharge.ChargeDay;
                dr["ChargeAmount"] = accountPackageRecurCharge.ChargeAmount;
                dr["CurrencyID"] = accountPackageRecurCharge.CurrencyID;
                dr["TransactionTypeID"] = accountPackageRecurCharge.TransactionTypeID;
                dtAccountPackageRecurCharge.Rows.Add(dr);
            }
            dtAccountPackageRecurCharge.EndLoadData();
            return dtAccountPackageRecurCharge;
        }

        DataTable GetAccountPackageRecurChargeTable()
        {
            DataTable dtAccountPackageRecurCharge = new DataTable();
            dtAccountPackageRecurCharge.Columns.Add("AccountPackageID", typeof(int));
            dtAccountPackageRecurCharge.Columns.Add("ChargeableEntityID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("BalanceAccountTypeID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("BalanceAccountID", typeof(string));
            dtAccountPackageRecurCharge.Columns.Add("ChargeDay", typeof(DateTime));
            dtAccountPackageRecurCharge.Columns.Add("ChargeAmount", typeof(decimal));
            dtAccountPackageRecurCharge.Columns.Add("CurrencyID", typeof(int));
            dtAccountPackageRecurCharge.Columns.Add("TransactionTypeID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("ProcessInstanceID", typeof(long));
            return dtAccountPackageRecurCharge;
        }

        #endregion

        #region Mappers

        private AccountPackageRecurCharge AccountPackageRecurChargeMapper(IDataReader reader)
        {
            return new AccountPackageRecurCharge()
            {
                AccountPackageRecurChargeId = (long)reader["ID"],
                AccountPackageID = (int)reader["AccountPackageID"],
                ChargeableEntityID = (Guid)reader["ChargeableEntityID"],
                BalanceAccountTypeID = (Guid)reader["BalanceAccountTypeID"],
                BalanceAccountID = reader["BalanceAccountID"] as string,
                ChargeDay = (DateTime)reader["ChargeDay"],
                ChargeAmount = (decimal)reader["ChargeAmount"],
                CurrencyID = (int)reader["CurrencyID"],
                TransactionTypeID = (Guid)reader["TransactionTypeID"],
                ProcessInstanceID = (long)reader["ProcessInstanceID"],
                IsSentToAccountBalance = (bool)reader["IsSentToAccountBalance"],
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
        }

        #endregion
    }
}