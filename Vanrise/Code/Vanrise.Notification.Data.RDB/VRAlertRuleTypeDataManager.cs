using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Notification.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Data.RDB
{
    public class VRAlertRuleTypeDataManager : IVRAlertRuleTypeDataManager
    {
        #region RDB
        static string TABLE_NAME = "VRNotification_VRAlertRuleType";
        static string TABLE_ALIAS = "vrAlertRuleType";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static VRAlertRuleTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VRNotification",
                DBTableName = "VRAlertRuleType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Notification", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        public VRAlertRuleType VRAlertRuleTypeMapper(IRDBDataReader reader)
        {
            return new VRAlertRuleType()
            {
                VRAlertRuleTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<VRAlertRuleTypeSettings>(reader.GetString(COL_Settings)),
            };
        }
        #endregion


        #region IVRAlertRuleTypeDataManager
        public List<VRAlertRuleType> GetVRAlertRuleTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(VRAlertRuleTypeMapper);
        }

        public bool AreVRAlertRuleTypeUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool Insert(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(vrAlertRuleTypeItem.Name);
            insertQuery.Column(COL_ID).Value(vrAlertRuleTypeItem.VRAlertRuleTypeId);
            insertQuery.Column(COL_Name).Value(vrAlertRuleTypeItem.Name);
            if (vrAlertRuleTypeItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrAlertRuleTypeItem.Settings));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(vrAlertRuleTypeItem.VRAlertRuleTypeId);
            ifNotExists.EqualsCondition(COL_Name).Value(vrAlertRuleTypeItem.Name);
            updateQuery.Column(COL_Name).Value(vrAlertRuleTypeItem.Name);
            if (vrAlertRuleTypeItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrAlertRuleTypeItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(vrAlertRuleTypeItem.VRAlertRuleTypeId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void GenerateScript(List<VRAlertRuleType> ruleTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var ruleType in ruleTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", ruleType.VRAlertRuleTypeId, ruleType.Name, Serializer.Serialize(ruleType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
            addEntityScript("[VRNotification].[VRAlertRuleType]", script);
        }
        #endregion
    }
}
