using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class OperatorAccountDataManager : BaseSQLDataManager, IOperatorAccountDataManager
    {

        #region ctor/Local Variables
        public OperatorAccountDataManager(): base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {
        }
        #endregion

        #region Public Methods
        public bool Insert(OperatorAccount operatorAccount, out int insertedId)
        {

            object operatorAccountId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorAccount_Insert", out operatorAccountId, operatorAccount.NameSuffix, operatorAccount.OperatorProfileId, Vanrise.Common.Serializer.Serialize(operatorAccount.CustomerSettings),
                Vanrise.Common.Serializer.Serialize(operatorAccount.SupplierSettings), Vanrise.Common.Serializer.Serialize(operatorAccount.OperatorAccountSettings));
            insertedId = (int)operatorAccountId;
            return (recordsEffected > 0);
        }
        public bool Update(OperatorAccount operatorAccount)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorAccount_Update", operatorAccount.OperatorAccountId, operatorAccount.NameSuffix, operatorAccount.OperatorProfileId, Vanrise.Common.Serializer.Serialize(operatorAccount.CustomerSettings),
                 Vanrise.Common.Serializer.Serialize(operatorAccount.SupplierSettings), Vanrise.Common.Serializer.Serialize(operatorAccount.OperatorAccountSettings));
            return (recordsEffected > 0);
        }
        public List<OperatorAccount> GetOperatorAccounts()
        {
            return GetItemsSP("dbo.sp_OperatorAccount_GetAll", OperatorAccountMapper);
        }
        public bool AreOperatorAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.OperatorAccount", ref updateHandle);
        }
        #endregion


        #region  Mappers
        private OperatorAccount OperatorAccountMapper(IDataReader reader)
        {
            OperatorAccount operatorAccount = new OperatorAccount
            {
                OperatorAccountId = (int)reader["ID"],
                NameSuffix = reader["NameSuffix"] as string,
                SupplierSettings = Vanrise.Common.Serializer.Deserialize<Entities.OperatorAccountSupplierSettings>(reader["SupplierSettings"] as string),
                CustomerSettings = Vanrise.Common.Serializer.Deserialize<Entities.OperatorAccountCustomerSettings>(reader["CustomerSettings"] as string),
                OperatorProfileId = (int)reader["OperatorProfileId"],
                OperatorAccountSettings = Vanrise.Common.Serializer.Deserialize<Entities.OperatorAccountSettings>(reader["OperatorAccountSettings"] as string),
            };
            return operatorAccount;
        }
        #endregion

    }
}
