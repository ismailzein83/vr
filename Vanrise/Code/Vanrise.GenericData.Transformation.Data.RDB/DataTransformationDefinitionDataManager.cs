using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.Data.RDB
{
    public class DataTransformationDefinitionDataManager : IDataTransformationDefinitionDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_DataTransformationDefinition";
        static string TABLE_ALIAS = "dataTransformationDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        static DataTransformationDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataTransformationDefinition",
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
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_DataTransformationDefinition", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        DataTransformationDefinition DataTransformationDefinitionMapper(IRDBDataReader reader)
        {
            DataTransformationDefinition dataTransformationDefinition = Vanrise.Common.Serializer.Deserialize<DataTransformationDefinition>(reader.GetString(COL_Details));
            if (dataTransformationDefinition != null)
            {
                dataTransformationDefinition.DataTransformationDefinitionId = reader.GetGuid(COL_ID);
                dataTransformationDefinition.Name = reader.GetString(COL_Name);
                dataTransformationDefinition.Title = reader.GetString(COL_Title);
            }
            return dataTransformationDefinition;
        }
        #endregion

        #region IDataTransformationDefinitionDataManager
        public bool AddDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataTransformationDefinition.Name);
            insertQuery.Column(COL_ID).Value(dataTransformationDefinition.DataTransformationDefinitionId);
            insertQuery.Column(COL_Name).Value(dataTransformationDefinition.Name);
            insertQuery.Column(COL_Title).Value(dataTransformationDefinition.Title);
            insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(dataTransformationDefinition));
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreDataTransformationDefinitionUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<DataTransformationDefinition> GetDataTransformationDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems<DataTransformationDefinition>(DataTransformationDefinitionMapper);
        }

        public bool UpdateDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(dataTransformationDefinition.DataTransformationDefinitionId);
            ifNotExists.EqualsCondition(COL_Name).Value(dataTransformationDefinition.Name);
            updateQuery.Column(COL_Name).Value(dataTransformationDefinition.Name);
            updateQuery.Column(COL_Title).Value(dataTransformationDefinition.Title);
            updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(dataTransformationDefinition));
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataTransformationDefinition.DataTransformationDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
