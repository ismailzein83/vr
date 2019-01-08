using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SwitchConnectivityDataManager : ISwitchConnectivityDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sc";
        static string TABLE_NAME = "TOneWhS_BE_SwitchConnectivity";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_CarrierAccountID = "CarrierAccountID";
        const string COL_SwitchID = "SwitchID";
        const string COL_Settings = "Settings";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_SourceID = "SourceID";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SwitchConnectivityDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 450}},
                {COL_CarrierAccountID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SwitchID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Settings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SwitchConnectivity",
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

        #region Members

        public List<SwitchConnectivity> GetSwitchConnectivities()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SwitchConnectivityMapper);
        }

        public bool AreSwitchConnectivitiesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool Insert(SwitchConnectivity switchConnectivity, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(switchConnectivity.Name);

            insertQuery.Column(COL_Name).Value(switchConnectivity.Name);
            insertQuery.Column(COL_CarrierAccountID).Value(switchConnectivity.CarrierAccountId);
            insertQuery.Column(COL_SwitchID).Value(switchConnectivity.SwitchId);
            insertQuery.Column(COL_BED).Value(switchConnectivity.BED);

            if (switchConnectivity.EED.HasValue)
                insertQuery.Column(COL_EED).Value(switchConnectivity.EED.Value);

            if (switchConnectivity.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(switchConnectivity.Settings));
            
            if (switchConnectivity.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(switchConnectivity.CreatedBy.Value);

            if (switchConnectivity.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(switchConnectivity.LastModifiedBy.Value);
            
            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool Update(SwitchConnectivityToEdit switchConnectivity)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(switchConnectivity.Name);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(switchConnectivity.SwitchConnectivityId);

            updateQuery.Column(COL_Name).Value(switchConnectivity.Name);
            updateQuery.Column(COL_SwitchID).Value(switchConnectivity.SwitchId);
            updateQuery.Column(COL_BED).Value(switchConnectivity.BED);
            updateQuery.Column(COL_CarrierAccountID).Value(switchConnectivity.CarrierAccountId);

            if(switchConnectivity.EED.HasValue)
                updateQuery.Column(COL_EED).Value(switchConnectivity.EED.Value);

            if (switchConnectivity.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(switchConnectivity.Settings));

            if (switchConnectivity.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(switchConnectivity.LastModifiedBy.Value);

            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(switchConnectivity.SwitchConnectivityId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        #region  Mappers

        private SwitchConnectivity SwitchConnectivityMapper(IRDBDataReader reader)
        {
            return new SwitchConnectivity
            {
                SwitchConnectivityId = reader.GetInt("ID"),
                Name = reader.GetString("Name"),
                SwitchId = reader.GetInt("SwitchId"),
                Settings = Serializer.Deserialize<SwitchConnectivitySettings>(reader.GetString("Settings")),
                CarrierAccountId = reader.GetInt("CarrierAccountID"),
                BED = reader.GetDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                CreatedTime = reader.GetDateTimeWithNullHandling("CreatedTime"),
                CreatedBy = reader.GetNullableInt("CreatedBy"),
                LastModifiedBy = reader.GetNullableInt("LastModifiedBy"),
                LastModifiedTime = reader.GetNullableDateTime("LastModifiedTime"),
            };
        }

        #endregion
    }
}
