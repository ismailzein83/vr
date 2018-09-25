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
    public class GenericRuleDefinitionDataManager : BaseSQLDataManager, IGenericRuleDefinitionDataManager
    {
        public GenericRuleDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods

        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions()
        {
            return GetItemsSP("genericdata.sp_GenericRuleDefinition_GetAll", GenericRuleDefinitionMapper);
        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericRuleDefinition", ref updateHandle);
        }

        public bool AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Insert", genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));

            return (affectedRows == 1);
        }

        public bool UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Update", genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));
            return (affectedRows == 1);
        }

        public void GenerateScript(List<GenericRuleDefinition> ruleDefinitions, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", ruleDefinition.GenericRuleDefinitionId, ruleDefinition.Name, Serializer.Serialize(ruleDefinition));
            }

            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[GenericRuleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);", scriptBuilder);
            addEntityScript("[genericdata].[GenericRuleDefinition]", script);
        }

        #endregion

        #region Mappers

        GenericRuleDefinition GenericRuleDefinitionMapper(IDataReader reader)
        {
            // The Details column doesn't allow null values
            GenericRuleDefinition details = Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>(reader["Details"] as string);
            return new GenericRuleDefinition()
            {
                GenericRuleDefinitionId = GetReaderValue<Guid>(reader, "ID"),
                Name = (string)reader["Name"],
                Title = details.Title,
                Objects = details.Objects,
                CriteriaDefinition = details.CriteriaDefinition,
                SettingsDefinition = details.SettingsDefinition,
                Security = details.Security
            };
        }

        #endregion
    }
}
