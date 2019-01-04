using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data.RDB
{
    public class VRAlertRuleDataManager : IVRAlertRuleDataManager
    {
        #region RDB

        static string TABLE_NAME = "VRNotification_VRAlertRule";
        static string TABLE_ALIAS = "vrAlertRule";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_RuleTypeId = "RuleTypeId";
        const string COL_UserId = "UserId";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_IsDisabled = "IsDisabled";


        static VRAlertRuleDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_RuleTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_UserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IsDisabled, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VRNotification",
                DBTableName = "VRAlertRule",
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
        public VRAlertRule VRAlertRuleMapper(IRDBDataReader reader)
        {
            return new VRAlertRule()
            {
                VRAlertRuleId = reader.GetLong(COL_ID),
                Name = reader.GetString(COL_Name),
                RuleTypeId = reader.GetGuid(COL_RuleTypeId),
                UserId = reader.GetInt(COL_UserId),
                Settings = Vanrise.Common.Serializer.Deserialize<VRAlertRuleSettings>(reader.GetString(COL_Settings)),
                IsDisabled = reader.GetBoolean(COL_IsDisabled),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };

        }
        #endregion

        #region IVRAlertRuleDataManager
        public List<VRAlertRule> GetVRAlertRules()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(VRAlertRuleMapper);

        }
        public bool AreVRAlertRuleUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool Insert(VRAlertRule VRAlertRuleItem, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(VRAlertRuleItem.Name);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_Name).Value(VRAlertRuleItem.Name);
            insertQuery.Column(COL_IsDisabled).Value(VRAlertRuleItem.IsDisabled);
            insertQuery.Column(COL_RuleTypeId).Value(VRAlertRuleItem.RuleTypeId);
            insertQuery.Column(COL_UserId).Value(VRAlertRuleItem.UserId);
            if (VRAlertRuleItem.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(VRAlertRuleItem.CreatedBy.Value);
            if (VRAlertRuleItem.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(VRAlertRuleItem.LastModifiedBy.Value);
            if (VRAlertRuleItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(VRAlertRuleItem.Settings));
            var insertedID = queryContext.ExecuteScalar().NullableLongValue;
            if (insertedID.HasValue)
            {
                insertedId = (long)insertedID;
                return true;
            }
            insertedId = -1;
            return false;
        }

        public bool Update(VRAlertRule VRAlertRuleItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(VRAlertRuleItem.VRAlertRuleId);
            ifNotExists.EqualsCondition(COL_Name).Value(VRAlertRuleItem.Name);
            updateQuery.Column(COL_Name).Value(VRAlertRuleItem.Name);
            updateQuery.Column(COL_RuleTypeId).Value(VRAlertRuleItem.RuleTypeId);
            if (VRAlertRuleItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(VRAlertRuleItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            if (VRAlertRuleItem.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(VRAlertRuleItem.LastModifiedBy.Value);
            else
                updateQuery.Column(COL_LastModifiedBy).Null();
            updateQuery.Column(COL_IsDisabled).Value(VRAlertRuleItem.IsDisabled);
            updateQuery.Where().EqualsCondition(COL_ID).Value(VRAlertRuleItem.VRAlertRuleId);
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool DisableAlertRule(long vrAlertRuleId, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_IsDisabled).Value(true);
            updateQuery.Where().EqualsCondition(COL_ID).Value(vrAlertRuleId);
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool EnableAlertRule(long vrAlertRuleId, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_IsDisabled).Value(false);
            updateQuery.Where().EqualsCondition(COL_ID).Value(vrAlertRuleId);
            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
