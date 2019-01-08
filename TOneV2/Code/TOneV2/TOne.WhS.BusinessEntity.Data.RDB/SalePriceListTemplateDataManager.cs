using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListTemplateDataManager : ISalePriceListTemplateDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spt";
        static string TABLE_NAME = "TOneWhS_BE_SalePriceListTemplate";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SalePriceListTemplateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255}},
                {COL_Settings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceListTemplate",
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

        #region ISalePriceListTemplateDataManager Members

        public IEnumerable<SalePriceListTemplate> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SalePriceListTemplateMapper);
        }

        public bool Insert(SalePriceListTemplate salePriceListTemplate, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(salePriceListTemplate.Name);

            insertQuery.Column(COL_Name).Value(salePriceListTemplate.Name);

            if (salePriceListTemplate.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(salePriceListTemplate.Settings));

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool Update(SalePriceListTemplate salePriceListTemplate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(salePriceListTemplate.Name);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(salePriceListTemplate.SalePriceListTemplateId);

            updateQuery.Column(COL_Name).Value(salePriceListTemplate.Name);

            if (salePriceListTemplate.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(salePriceListTemplate.Settings));

            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(salePriceListTemplate.SalePriceListTemplateId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSalePriceListTemplatesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region Mappers

        private SalePriceListTemplate SalePriceListTemplateMapper(IRDBDataReader reader)
        {
            return new SalePriceListTemplate
            {
                SalePriceListTemplateId = reader.GetInt("ID"),
                Name = reader.GetString("Name"),
                Settings = Serializer.Deserialize<SalePriceListTemplateSettings>(reader.GetString("Settings"))
            };
        }

        #endregion
    }
}
