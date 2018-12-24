using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Notification.Entities;

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


        static VRAlertRuleTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VRNotification",
                DBTableName = "VRAlertRuleType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Notification_VRAlertRuleType", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
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
            throw new NotImplementedException();
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
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrAlertRuleTypeItem.Settings));
            updateQuery.Where().EqualsCondition(COL_ID).Value(vrAlertRuleTypeItem.VRAlertRuleTypeId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void GenerateScript(List<VRAlertRuleType> ruleTypes, Action<string, string> addEntityScript)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
