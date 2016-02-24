using InterConnect.BusinessEntity.Entities;
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
        #endregion

        #region  Mappers
        private OperatorAccount OperatorProfileMapper(IDataReader reader)
        {
            OperatorAccount operationAccount = new OperatorAccount
            {
                ProfileId = (int)reader["ID"],
                Suffix = reader["Suffix"] as string
            };
            return operationAccount;
        }

        #endregion
    }
}
