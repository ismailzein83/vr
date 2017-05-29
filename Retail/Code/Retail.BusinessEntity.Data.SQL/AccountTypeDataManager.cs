using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountTypeDataManager : BaseSQLDataManager, IAccountTypeDataManager
    {
        #region Constructors

        public AccountTypeDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountType> GetAccountTypes()
        {
            return GetItemsSP("Retail_BE.sp_AccountType_GetAll", AccountTypeMapper);
        }

        public bool Insert(AccountType accountType)
        {
            string serializedSettings = accountType.Settings != null ? Vanrise.Common.Serializer.Serialize(accountType.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountType_Insert", accountType.AccountTypeId, accountType.Name, accountType.Title,accountType.AccountBEDefinitionId, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(AccountTypeToEdit accountType)
        {
            string serializedSettings = accountType.Settings != null ? Vanrise.Common.Serializer.Serialize(accountType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountType_Update", accountType.AccountTypeId, accountType.Name, accountType.Title,accountType.AccountBEDefinitionId, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreAccountTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.AccountType", ref updateHandle);
        }

        public void GenerateScript(List<AccountType> accountTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var accountType in accountTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", accountType.AccountTypeId, accountType.Name, accountType.Title, accountType.AccountBEDefinitionId, Serializer.Serialize(accountType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionID],[Settings]))
merge	[Retail_BE].[AccountType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionID] = s.[AccountBEDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionID],s.[Settings]);", scriptBuilder);
            addEntityScript("[Retail_BE].[AccountType]", script);
        }

        #endregion

        #region Mappers

        private AccountType AccountTypeMapper(IDataReader reader)
        {
            return new AccountType()
            {
                AccountTypeId =  GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                AccountBEDefinitionId = GetReaderValue<Guid>(reader, "AccountBEDefinitionId"),
                Settings = Vanrise.Common.Serializer.Deserialize<AccountTypeSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
