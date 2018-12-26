using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Data;
using Vanrise.Data.RDB;

namespace Vanrise.Notification.Data.RDB
{

    public class VRAlertLevelDataManager : IVRAlertLevelDataManager
    {
        static string TABLE_NAME = "VR_Notification_VRAlertLevel";
        static string TABLE_ALIAS = "vrAlertLevel";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_BusinessEntityDefinitionID = "BusinessEntityDefinitionID";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static VRAlertLevelDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BusinessEntityDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VRNotification",
                DBTableName = "VRAlertLevel",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }


        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Notification", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }

        public VRAlertLevel VRAlertLevelMapper(IRDBDataReader reader)
        {
            VRAlertLevel vrAlertLevel = new VRAlertLevel
            {
                VRAlertLevelId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<VRAlertLevelSettings>(reader.GetString(COL_Settings)),
                BusinessEntityDefinitionId = reader.GetGuid(COL_BusinessEntityDefinitionID),
            };
            return vrAlertLevel;
        }
        #endregion

        #region IVRAlertLevelDataManager
        public bool AreAlertLevelUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<VRAlertLevel> GetAlertLevel()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(VRAlertLevelMapper);
        }

        public bool Insert(VRAlertLevel alertLevelItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(alertLevelItem.Name);
            ifNotExists.EqualsCondition(COL_BusinessEntityDefinitionID).Value(alertLevelItem.BusinessEntityDefinitionId);

            insertQuery.Column(COL_ID).Value(alertLevelItem.VRAlertLevelId);
            insertQuery.Column(COL_Name).Value(alertLevelItem.Name);
            insertQuery.Column(COL_BusinessEntityDefinitionID).Value(alertLevelItem.BusinessEntityDefinitionId);
            if (alertLevelItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(alertLevelItem.Settings));
            else
                insertQuery.Column(COL_Settings).Null();
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(VRAlertLevel alertLevelItem)
        {

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(alertLevelItem.VRAlertLevelId);
            notExistsCondition.EqualsCondition(COL_Name).Value(alertLevelItem.Name);
            notExistsCondition.EqualsCondition(COL_BusinessEntityDefinitionID).Value(alertLevelItem.BusinessEntityDefinitionId);

            updateQuery.Column(COL_Name).Value(alertLevelItem.Name);
            updateQuery.Column(COL_BusinessEntityDefinitionID).Value(alertLevelItem.BusinessEntityDefinitionId);
            if (alertLevelItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(alertLevelItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(alertLevelItem.VRAlertLevelId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
