using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using Retail.BusinessEntity.Entities;
using System.Data.SqlClient;

namespace Retail.BusinessEntity.Data.SQL
{
    public class RecurChargeBalanceUpdateSummaryDataManager : BaseSQLDataManager, IRecurChargeBalanceUpdateSummaryDataManager
    {

        #region Constructors

        public RecurChargeBalanceUpdateSummaryDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<RecurChargeBalanceUpdateSummary> GetRecurChargeBalanceUpdateSummaryList(DateTime chargeDay)
        {
            return GetItemsSP("[Retail_BE].[sp_RecurChargeBalanceUpdateSummary_GetByChargeDay]", RecurChargeBalanceUpdateSummaryMapper, chargeDay);
        }

        public void Insert(RecurChargeBalanceUpdateSummary recurChargeBalanceUpdateSummary)
        {
            ExecuteNonQuerySP("[Retail_BE].[sp_RecurChargeBalanceUpdateSummary_Insert]", recurChargeBalanceUpdateSummary.ChargeDay, Vanrise.Common.Serializer.Serialize(recurChargeBalanceUpdateSummary.AccountPackageRecurChargeKeys));
        }

        public DateTime? GetMaximumChargeDay()
        {
            object maximumChargeDay = ExecuteScalarSP("[Retail_BE].[sp_RecurChargeBalanceUpdateSummary_GetMaximumChargeDay]");
            if (maximumChargeDay != null)
                return (DateTime)maximumChargeDay;
            return null;
        }

        public void Delete(DateTime chargeDay)
        {
            ExecuteNonQuerySP("[Retail_BE].[sp_RecurChargeBalanceUpdateSummary_DeleteByChargeDay]", chargeDay);
        }
        #endregion

        #region Mappers
        private RecurChargeBalanceUpdateSummary RecurChargeBalanceUpdateSummaryMapper(IDataReader reader)
        {
            return new RecurChargeBalanceUpdateSummary()
            {
                ChargeDay = (DateTime)reader["ChargeDay"],
                AccountPackageRecurChargeKeys = Vanrise.Common.Serializer.Deserialize<HashSet<AccountPackageRecurChargeKey>>(reader["Data"] as string)
            };
        }
        #endregion
    }
}