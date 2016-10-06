using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountPartDefinitionDataManager: BaseSQLDataManager, IAccountPartDefinitionDataManager
    {
      
        #region Constructors

        public AccountPartDefinitionDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountPartDefinition> GetAccountPartDefinitions()
        {
            return GetItemsSP("Retail_BE.sp_AccountPartDefinition_GetAll", AccountPartDefinitionMapper);
        }

        public bool Insert(AccountPartDefinition accountPartDefinition)
        {
            string serializedSettings = accountPartDefinition != null ? Vanrise.Common.Serializer.Serialize(accountPartDefinition) : null;

            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountPartDefinition_Insert", accountPartDefinition.AccountPartDefinitionId, accountPartDefinition.Name,accountPartDefinition.Title, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(AccountPartDefinition accountPartDefinition)
        {
            string serializedSettings = accountPartDefinition != null ? Vanrise.Common.Serializer.Serialize(accountPartDefinition) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountPartDefinition_Update", accountPartDefinition.AccountPartDefinitionId, accountPartDefinition.Name, accountPartDefinition.Title, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreAccountPartDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.AccountPartDefinition", ref updateHandle);
        }

        #endregion

        #region Mappers

        private AccountPartDefinition AccountPartDefinitionMapper(IDataReader reader)
        {
            AccountPartDefinition accountPartDefinition = Vanrise.Common.Serializer.Deserialize<AccountPartDefinition>(reader["Details"] as string);
            accountPartDefinition.Name = reader["Name"] as string;
            accountPartDefinition.Title = reader["Title"] as string;
            accountPartDefinition.AccountPartDefinitionId = GetReaderValue<Guid>(reader,"ID");
            return accountPartDefinition;
        }

        #endregion
    }
}
