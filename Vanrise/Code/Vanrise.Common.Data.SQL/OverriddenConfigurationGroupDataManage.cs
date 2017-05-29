using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class OverriddenConfigurationGroupDataManager : BaseSQLDataManager, IOverriddenConfigurationGroupDataManager
    {
        #region ctor/Local Variables
        public OverriddenConfigurationGroupDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnString", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods
       public List<OverriddenConfigurationGroup> GetOverriddenConfigurationGroup()
        {
            return GetItemsSP("Common.sp_OverriddenConfigurationGroup_GetAll", OverriddenConfigurationGroupMapper);
        }
        public bool AreOverriddenConfigurationGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Common.OverriddenConfigurationGroup", ref updateHandle);
        }
        public bool Insert(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            int affectedRecords = ExecuteNonQuerySP("Common.sp_OverriddenConfigurationGroup_Insert", overriddenConfigurationGroup.OverriddenConfigurationGroupId, overriddenConfigurationGroup.Name);
            return (affectedRecords > 0);

        }

        public void GenerateScript(List<OverriddenConfigurationGroup> groups, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var itm in groups)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}')", itm.OverriddenConfigurationGroupId, itm.Name);
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[common].[OverriddenConfigurationGroup] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name]);", scriptBuilder);
            addEntityScript("[common].[OverriddenConfigurationGroup]", script);
        }
       
        #endregion

        #region Mappers
        OverriddenConfigurationGroup OverriddenConfigurationGroupMapper(IDataReader reader)
        {
            OverriddenConfigurationGroup overriddenConfigurationGroup = new OverriddenConfigurationGroup
            {
                OverriddenConfigurationGroupId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
            };
            return overriddenConfigurationGroup;
        }

        #endregion
    }
}
