using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class ZoneServiceConfigDataManager : IZoneServiceConfigDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "zsc";
        static string TABLE_NAME = "TOneWhS_BE_ZoneServiceConfig";
        const string COL_ID = "ID";
        const string COL_Symbol = "Symbol";
        const string COL_Settings = "Settings";
        const string COL_SourceID = "SourceID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static ZoneServiceConfigDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Symbol, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 50}},
                {COL_Settings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "ZoneServiceConfig",
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

        #region IZoneServiceConfigDataManager Members
        public List<ZoneServiceConfig> GetZoneServiceConfigs()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(ZoneServiceConfigMapper);
        }

        public bool Update(ZoneServiceConfig zoneServiceFlag)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Symbol).Value(zoneServiceFlag.Symbol);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(zoneServiceFlag.ZoneServiceConfigId);

            updateQuery.Column(COL_Symbol).Value(zoneServiceFlag.Symbol);

            if (zoneServiceFlag.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(zoneServiceFlag.Settings));

            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(zoneServiceFlag.ZoneServiceConfigId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Insert(ZoneServiceConfig zoneServiceFlag, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Symbol).Value(zoneServiceFlag.Symbol);

            insertQuery.Column(COL_Symbol).Value(zoneServiceFlag.Symbol);

            if (zoneServiceFlag.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(zoneServiceFlag.Settings));

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool AreZoneServiceConfigsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion
        #region  Mappers
        ZoneServiceConfig ZoneServiceConfigMapper(IRDBDataReader reader)
        {
            ZoneServiceConfig zoneServiceConfig = new ZoneServiceConfig
            {
                ZoneServiceConfigId = reader.GetInt(COL_ID),
                Symbol = reader.GetString(COL_Symbol),
                Settings = Serializer.Deserialize<ServiceConfigSetting>(reader.GetString(COL_Settings)),
                SourceId = reader.GetString(COL_SourceID)
            };
            return zoneServiceConfig;
        }

        #endregion
    }
}
