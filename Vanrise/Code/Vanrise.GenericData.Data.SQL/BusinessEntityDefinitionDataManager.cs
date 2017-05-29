using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class BusinessEntityDefinitionDataManager : BaseSQLDataManager, IBusinessEntityDefinitionDataManager
    {
        public BusinessEntityDefinitionDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey")) { }

        #region Public Methods
        public IEnumerable<BusinessEntityDefinition> GetBusinessEntityDefinitions()
        {
            return GetItemsSP("genericdata.sp_BusinessEntityDefinition_GetAll", BusinessEntityDefinitionMapper);
        }
        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.BusinessEntityDefinition", ref updateHandle);
        }
        public bool UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            string serializedObj = null;
            if (businessEntityDefinition != null && businessEntityDefinition.Settings != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(businessEntityDefinition.Settings);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityDefinition_Update", businessEntityDefinition.BusinessEntityDefinitionId, businessEntityDefinition.Name, businessEntityDefinition.Title, serializedObj);
            return (recordesEffected > 0);
        }

        public bool AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            string serializedObj = null;
            if (businessEntityDefinition != null && businessEntityDefinition.Settings != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(businessEntityDefinition.Settings);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityDefinition_Insert", businessEntityDefinition.BusinessEntityDefinitionId, businessEntityDefinition.Name, businessEntityDefinition.Title, serializedObj);


            return (recordesEffected > 0);
        }
        public void GenerateScript(List<BusinessEntityDefinition> beDefinitions, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var beDefinition in beDefinitions)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", beDefinition.BusinessEntityDefinitionId, beDefinition.Name, beDefinition.Title, Serializer.Serialize(beDefinition.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);", scriptBuilder);
            addEntityScript("[genericdata].[BusinessEntityDefinition]", script);
        }

        #endregion

        #region Mappers

        BusinessEntityDefinition BusinessEntityDefinitionMapper(IDataReader reader)
        {
            return new BusinessEntityDefinition()
            {
                BusinessEntityDefinitionId = GetReaderValue<Guid>( reader,"ID"),
                Name = (string)reader["Name"],
                Title = (string)reader["Title"],
                Settings = Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionSettings>((string)reader["Settings"])
            };
        }
        
        #endregion


       
    }
}
