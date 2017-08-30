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

        public void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime chargeDay)
        {
            DataTable dtAccountPackageRecurCharge = BuildAccountPackageReccuringChargeTable(accountPackageRecurChargeList);
            int recordsEffected = ExecuteNonQuerySPCmd("[Retail_BE].[sp_AccountPackageRecurCharge_InsertOrUpdate]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@ChargeDay", chargeDay));
                var dtPrm = new SqlParameter("@AccountPackageRecurCharges", SqlDbType.Structured);
                dtPrm.Value = dtAccountPackageRecurCharge;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<AccountPackageRecurCharge> GetAccountRecurringCharges(Guid acountBEDefinitionId, long accountId, DateTime includedFromDate, DateTime includedToDate)
        {
            return GetItemsSP("[Retail_BE].[sp_AccountPackageRecurCharge_GetByAccount]", AccountPackageRecurChargeMapper, acountBEDefinitionId, accountId, includedFromDate, includedToDate);
        }

        public List<AccountPackageRecurCharge> GetAccountRecurringCharges(List<AccountPackageRecurChargePeriod> accountPackageRecurChargePeriods)
        {
            DataTable dtAccountPackageRecurChargePeriod = BuildAccountPackageRecurChargePeriodTable(accountPackageRecurChargePeriods);
            return GetItemsSPCmd("[Retail_BE].[sp_AccountPackageRecurCharge_GetByAccountPackages]", AccountPackageRecurChargeMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@AccountPackageRecurChargePeriods", SqlDbType.Structured);
                dtPrm.Value = dtAccountPackageRecurChargePeriod;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public DateTime? GetMaximumChargeDay()
        {
            object maximumChargeDayAsObj = ExecuteScalarSP("[Retail_BE].[sp_AccountPackageRecurCharge_GetMaximumChargeDay]");

            DateTime? maximumChargeDay = null;
            if (maximumChargeDayAsObj != DBNull.Value)
                maximumChargeDay = (DateTime)maximumChargeDayAsObj;

            return maximumChargeDay;
        }

        public List<AccountPackageRecurChargeKey> GetAccountRecurringChargeKeys(DateTime chargeDay)
        {
            return GetItemsSP("[Retail_BE].[sp_AccountPackageRecurChargeKey_GetByChargeDay]", AccountPackageRecurChargeKeyMapper, chargeDay);
        }

        public int GetChargeAmountPrecision()
        {
            return 10;
        }
        #endregion

        #region Private Methods
        private DataTable BuildAccountPackageRecurChargePeriodTable(List<AccountPackageRecurChargePeriod> accountPackageRecurChargePeriods)
        {
            DataTable dtAccountPackageRecurChargePeriod = GetAccountPackageRecurChargePeriodTable();
            dtAccountPackageRecurChargePeriod.BeginLoadData();
            if (accountPackageRecurChargePeriods != null)
            {
                foreach (var accountPackageRecurChargePeriod in accountPackageRecurChargePeriods)
                {
                    DataRow dr = dtAccountPackageRecurChargePeriod.NewRow();
                    dr["AccountPackageId"] = accountPackageRecurChargePeriod.AccountPackageId;
                    dr["FromDate"] = accountPackageRecurChargePeriod.FromDate;
                    dr["ToDate"] = accountPackageRecurChargePeriod.ToDate;
                    dtAccountPackageRecurChargePeriod.Rows.Add(dr);
                }
            }
            dtAccountPackageRecurChargePeriod.EndLoadData();
            return dtAccountPackageRecurChargePeriod;
        }

        private DataTable GetAccountPackageRecurChargePeriodTable()
        {
            DataTable dtAccountPackageRecurChargePeriod = new DataTable();
            dtAccountPackageRecurChargePeriod.Columns.Add("AccountPackageId", typeof(long));
            dtAccountPackageRecurChargePeriod.Columns.Add("FromDate", typeof(DateTime));
            dtAccountPackageRecurChargePeriod.Columns.Add("ToDate", typeof(DateTime));
            return dtAccountPackageRecurChargePeriod;
        }

        DataTable BuildAccountPackageReccuringChargeTable(List<AccountPackageRecurCharge> accountPackageRecurChargeList)
        {
            DataTable dtAccountPackageRecurCharge = GetAccountPackageRecurChargeTable();
            dtAccountPackageRecurCharge.BeginLoadData();
            if (accountPackageRecurChargeList != null)
            {
                foreach (var accountPackageRecurCharge in accountPackageRecurChargeList)
                {
                    DataRow dr = dtAccountPackageRecurCharge.NewRow();
                    dr["AccountPackageID"] = accountPackageRecurCharge.AccountPackageID;
                    dr["ChargeableEntityID"] = accountPackageRecurCharge.ChargeableEntityID;

                    if (accountPackageRecurCharge.BalanceAccountTypeID.HasValue)
                        dr["BalanceAccountTypeID"] = accountPackageRecurCharge.BalanceAccountTypeID.Value;
                    else
                        dr["BalanceAccountTypeID"] = DBNull.Value;

                    if (!string.IsNullOrEmpty(accountPackageRecurCharge.BalanceAccountID))
                        dr["BalanceAccountID"] = accountPackageRecurCharge.BalanceAccountID;
                    else
                        dr["BalanceAccountID"] = DBNull.Value;

                    dr["ChargeDay"] = accountPackageRecurCharge.ChargeDay;
                    dr["ChargeAmount"] = accountPackageRecurCharge.ChargeAmount;
                    dr["CurrencyID"] = accountPackageRecurCharge.CurrencyID;

                    if (accountPackageRecurCharge.TransactionTypeID.HasValue)
                        dr["TransactionTypeID"] = accountPackageRecurCharge.TransactionTypeID.Value;
                    else
                        dr["TransactionTypeID"] = DBNull.Value;

                    dr["AccountID"] = accountPackageRecurCharge.AccountID;
                    dr["AccountBEDefinitionId"] = accountPackageRecurCharge.AccountBEDefinitionId;

                    dtAccountPackageRecurCharge.Rows.Add(dr);
                }
            }
            dtAccountPackageRecurCharge.EndLoadData();
            return dtAccountPackageRecurCharge;
        }

        DataTable GetAccountPackageRecurChargeTable()
        {
            DataTable dtAccountPackageRecurCharge = new DataTable();
            dtAccountPackageRecurCharge.Columns.Add("AccountPackageID", typeof(long));
            dtAccountPackageRecurCharge.Columns.Add("ChargeableEntityID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("BalanceAccountTypeID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("BalanceAccountID", typeof(string));
            dtAccountPackageRecurCharge.Columns.Add("ChargeDay", typeof(DateTime));
            dtAccountPackageRecurCharge.Columns.Add("ChargeAmount", typeof(decimal));
            dtAccountPackageRecurCharge.Columns.Add("CurrencyID", typeof(int));
            dtAccountPackageRecurCharge.Columns.Add("TransactionTypeID", typeof(Guid));
            dtAccountPackageRecurCharge.Columns.Add("AccountID", typeof(long));
            dtAccountPackageRecurCharge.Columns.Add("AccountBEDefinitionId", typeof(Guid));
            return dtAccountPackageRecurCharge;
        }

        #endregion

        #region Mappers

        private AccountPackageRecurCharge AccountPackageRecurChargeMapper(IDataReader reader)
        {
            return new AccountPackageRecurCharge()
            {
                AccountPackageRecurChargeId = (long)reader["ID"],
                AccountPackageID = (long)reader["AccountPackageID"],
                ChargeableEntityID = (Guid)reader["ChargeableEntityID"],
                BalanceAccountTypeID = GetReaderValue<Guid?>(reader, "BalanceAccountTypeID"),
                BalanceAccountID = reader["BalanceAccountID"] as string,
                AccountID = (long)reader["AccountID"],
                AccountBEDefinitionId = (Guid)reader["AccountBEDefinitionId"],
                ChargeDay = (DateTime)reader["ChargeDay"],
                ChargeAmount = (decimal)reader["ChargeAmount"],
                CurrencyID = (int)reader["CurrencyID"],
                TransactionTypeID = GetReaderValue<Guid?>(reader, "TransactionTypeID"),
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
        }

        private AccountPackageRecurChargeKey AccountPackageRecurChargeKeyMapper(IDataReader reader)
        {
            return new AccountPackageRecurChargeKey()
            {
                BalanceAccountTypeID = GetReaderValue<Guid?>(reader, "BalanceAccountTypeID"),
                ChargeDay = (DateTime)reader["ChargeDay"],
                TransactionTypeId = GetReaderValue<Guid?>(reader, "TransactionTypeID")
            };
        }
        #endregion
    }
}