using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SwitchReleaseCauseDataManager : ISwitchReleaseCauseDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "src";
        static string TABLE_NAME = "TOneWhS_BE_SwitchReleaseCause";
        const string COL_ID = "ID";
        const string COL_SwitchID = "SwitchID";
        const string COL_ReleaseCode = "ReleaseCode";
        const string COL_Settings = "Settings";
        const string COL_SourceID = "SourceID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SwitchReleaseCauseDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SwitchID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ReleaseCode, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SwitchReleaseCause",
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

        #region ISwitchReleaseCauseDataManager Members
        public List<SwitchReleaseCause> GetSwitchReleaseCauses()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SwitchReleaseCauseMapper);
        }

        public bool AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_SwitchID).Value(switchReleaseCause.SwitchId);
            notExistsCondition.EqualsCondition(COL_ReleaseCode).Value(switchReleaseCause.ReleaseCode);

            insertQuery.Column(COL_SwitchID).Value(switchReleaseCause.SwitchId);
            insertQuery.Column(COL_ReleaseCode).Value(switchReleaseCause.ReleaseCode);

            if (switchReleaseCause.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(switchReleaseCause.Settings));

            insertQuery.Column(COL_SourceID).Value(switchReleaseCause.SourceId);

            if (switchReleaseCause.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(switchReleaseCause.CreatedBy.Value);

            if (switchReleaseCause.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(switchReleaseCause.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_ReleaseCode).Value(switchReleaseCause.ReleaseCode);
            notExistsCondition.EqualsCondition(COL_SwitchID).Value(switchReleaseCause.SwitchId);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(switchReleaseCause.SwitchReleaseCauseId);

            updateQuery.Column(COL_ReleaseCode).Value(switchReleaseCause.ReleaseCode);
            updateQuery.Column(COL_SwitchID).Value(switchReleaseCause.SwitchId);
            updateQuery.Column(COL_SourceID).Value(switchReleaseCause.SourceId);

            if (switchReleaseCause.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(switchReleaseCause.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            if (switchReleaseCause.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(switchReleaseCause.LastModifiedBy.Value);
            else
                updateQuery.Column(COL_LastModifiedBy).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(switchReleaseCause.SwitchReleaseCauseId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSwitchReleaseCausesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Mapper
        SwitchReleaseCause SwitchReleaseCauseMapper(IRDBDataReader reader)
        {
            return new SwitchReleaseCause
            {
                ReleaseCode = reader.GetString(COL_ReleaseCode),
                SwitchReleaseCauseId = reader.GetInt(COL_ID),
                SwitchId = reader.GetIntWithNullHandling(COL_SwitchID),
                Settings = Serializer.Deserialize<SwitchReleaseCauseSetting>(reader.GetString(COL_Settings)),
                SourceId = reader.GetString(COL_SourceID),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetDateTimeWithNullHandling(COL_LastModifiedTime)
            };
        }
        #endregion
    }
}
