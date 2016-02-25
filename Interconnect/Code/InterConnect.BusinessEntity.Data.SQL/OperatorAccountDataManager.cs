using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace InterConnect.BusinessEntity.Data.SQL
{
    public class OperatorAccountDataManager : BaseSQLDataManager, IOperatorAccountDataManager
    {

        #region ctor/Local Variables
        public OperatorAccountDataManager()
            : base(GetConnectionStringName("Interconnect_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public List<OperatorAccount> GetOperatorAccounts()
        {
            return GetItemsSP("InterConnect_BE.sp_OperatorAccount_GetAll", OperatorProfileMapper);
        }

        public bool AreOperatorAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[InterConnect_BE].[OperatorAccount]", ref updateHandle);
        }


        public bool Insert(OperatorAccount operatorAccount, out int operatorProfileId)
        {
            object objectId;

            int recordesEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorAccount_Insert", out objectId, operatorAccount.Suffix, operatorAccount.ProfileId);
            operatorProfileId = (recordesEffected > 0) ? (int)objectId : -1;

            return (recordesEffected > 0);
        }

        public bool Update(OperatorAccount operatorAccount)
        {
            int recordesEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorAccount_Update", operatorAccount.OperatorAccountId, operatorAccount.Suffix, operatorAccount.ProfileId);
            return (recordesEffected > 0);
        }
        #endregion

        #region  Mappers
        private OperatorAccount OperatorProfileMapper(IDataReader reader)
        {
            OperatorAccount operationAccount = new OperatorAccount
            {
                OperatorAccountId = (int)reader["ID"],
                ProfileId = (int)reader["ProfileID"],
                Suffix = reader["Suffix"] as string
            };
            return operationAccount;
        }

        #endregion
    }
}