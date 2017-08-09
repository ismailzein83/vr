using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountStatusHistoryDataManager : BaseSQLDataManager, IAccountStatusHistoryDataManager
    {  
       
        #region Constructors
        public AccountStatusHistoryDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        public bool Insert(AccountStatusHistory accountStatusHistory, out long insertedId)
        {
            object accountStatusHistoryId;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountStatusHistory_Insert", out accountStatusHistoryId, accountStatusHistory.AccountId, accountStatusHistory.StatusId, accountStatusHistory.StatusChangedDate);
            if (affectedRecords > 0)
            {
                insertedId = (long)accountStatusHistoryId;
                return true;
            }
            insertedId = -1;
            return false;
        }
    }
}
