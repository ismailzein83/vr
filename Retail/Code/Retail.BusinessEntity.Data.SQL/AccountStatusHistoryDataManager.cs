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

        public void Insert(Guid accountDefinitionId, long accountId, Guid statusDefinitionId, Guid? previousStatusId)
        {
            ExecuteNonQuerySP("Retail_BE.sp_AccountStatusHistory_Insert", accountDefinitionId, accountId, statusDefinitionId, previousStatusId);
        }
    }
}
