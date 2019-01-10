using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Entities.ExpressionBuilder;
using Vanrise.Entities;
namespace Vanrise.GenericData.Data.RDB
{
    public class DataRecordTypeDataManager : IDataRecordTypeDataManager
    {
        #region RDB

        static string TABLE_NAME = "genericdata_DataRecordType";
        static string TABLE_ALIAS = "drt";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_ParentID = "ParentID";
        const string COL_Fields = "Fields";
        const string COL_ExtraFieldsEvaluator = "ExtraFieldsEvaluator";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static DataRecordTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_ParentID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Fields, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExtraFieldsEvaluator, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataRecordType",
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
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion
        #region Mappers
        DataRecordType DataRecordTypeMapper(IRDBDataReader reader)
        {
            return new DataRecordType
            {
                DataRecordTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                ParentId = reader.GetNullableGuid(COL_ParentID),
                Fields = Vanrise.Common.Serializer.Deserialize<List<DataRecordField>>(reader.GetString(COL_Fields)),
                ExtraFieldsEvaluator = Vanrise.Common.Serializer.Deserialize<DataRecordTypeExtraField>(reader.GetString(COL_ExtraFieldsEvaluator)),
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordTypeSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion

        #region IDataRecordTypeDataManager
        public bool AddDataRecordType(DataRecordType dataRecordType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordType.Name);
            insertQuery.Column(COL_ID).Value(dataRecordType.DataRecordTypeId);
            insertQuery.Column(COL_Name).Value(dataRecordType.Name);
            if (dataRecordType.ParentId.HasValue)
                insertQuery.Column(COL_ParentID).Value(dataRecordType.ParentId.Value);
            if (dataRecordType.Fields != null)
                insertQuery.Column(COL_Fields).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.Fields));
            if (dataRecordType.ExtraFieldsEvaluator != null)
                insertQuery.Column(COL_ExtraFieldsEvaluator).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.ExtraFieldsEvaluator));
            if (dataRecordType.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.Settings));
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreDataRecordTypeUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<DataRecordType> GetDataRecordTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(DataRecordTypeMapper);
        }

        public void SetDataRecordTypeCacheExpired()
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var item = selectQueryContext.GetItem(DataRecordTypeMapper);
            if (item != null)
            {
                var updateQueryContext = new RDBQueryContext(GetDataProvider());
                var updateQuery = updateQueryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                updateQuery.Column(COL_Name).Column(COL_Name);
                updateQuery.Where().EqualsCondition(COL_ID).Value(item.DataRecordTypeId);
                updateQueryContext.ExecuteNonQuery();
            }
        }

        public bool UpdateDataRecordType(DataRecordType dataRecordType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(dataRecordType.DataRecordTypeId);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordType.Name);
            updateQuery.Column(COL_Name).Value(dataRecordType.Name);
            if (dataRecordType.ParentId.HasValue)
                updateQuery.Column(COL_ParentID).Value(dataRecordType.ParentId.Value);
            else
                updateQuery.Column(COL_ParentID).Null();
            if (dataRecordType.Fields != null)
                updateQuery.Column(COL_Fields).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.Fields));
            else
                updateQuery.Column(COL_Fields).Null();
            if (dataRecordType.ExtraFieldsEvaluator != null)
                updateQuery.Column(COL_ExtraFieldsEvaluator).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.ExtraFieldsEvaluator));
            else
                updateQuery.Column(COL_ExtraFieldsEvaluator).Null();
            if (dataRecordType.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordType.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataRecordType.DataRecordTypeId);
            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
