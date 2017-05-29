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

        public void GenerateScript(List<AccountPartDefinition> accountParts, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var accountPart in accountParts)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", accountPart.AccountPartDefinitionId, accountPart.Title, accountPart.Name, Serializer.Serialize(accountPart));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Title],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Title],[Name],[Details]))
merge	[Retail_BE].[AccountPartDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Title] = s.[Title],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Title],[Name],[Details])
	values(s.[ID],s.[Title],s.[Name],s.[Details]);", scriptBuilder);
            addEntityScript("[Retail_BE].[AccountPartDefinition]", script);
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
