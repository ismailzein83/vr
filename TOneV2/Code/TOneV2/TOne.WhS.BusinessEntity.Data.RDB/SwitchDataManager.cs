using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SwitchDataManager : ISwitchDataManager
    {
        #region RDB
        static string TABLE_ALIAS = "s";
        static string TABLE_NAME = "TOneWhS_BE_Switch";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_SourceID = "SourceID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SwitchDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50});
            columns.Add(COL_Settings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar});
            columns.Add(COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "Switch",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region ISwitchDataManager Members

        public List<Switch> GetSwitches()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SwitchMapper);
        }

        public bool Insert(Switch whsSwitch, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_Name).Value(whsSwitch.Name);

            if (whsSwitch.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(whsSwitch.Settings));

            if (whsSwitch.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(whsSwitch.CreatedBy.Value);

            if (whsSwitch.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(whsSwitch.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool Update(SwitchToEdit whsSwitch)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(whsSwitch.Name);

            if (whsSwitch.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(whsSwitch.Settings));

            if (whsSwitch.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(whsSwitch.LastModifiedBy.Value);

            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(whsSwitch.SwitchId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Delete(int switchId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(switchId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSwitchesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Mappers
        Switch SwitchMapper(IRDBDataReader reader)
        {
            return new Switch
            {
                SwitchId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                SourceId = reader.GetString(COL_SourceID),
                Settings = Serializer.Deserialize<SwitchSettings>(reader.GetString(COL_Settings)),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
        }
        #endregion
    }
}
