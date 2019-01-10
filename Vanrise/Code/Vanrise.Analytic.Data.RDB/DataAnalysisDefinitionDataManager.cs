using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Analytic.Data.RDB
{
    public class DataAnalysisDefinitionDataManager : IDataAnalysisDefinitionDataManager
    {

        static string TABLE_NAME = "VR_Analytic_DataAnalysisDefinition";
        static string TABLE_ALIAS = "vrDataAnalysisDefinition";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static DataAnalysisDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "DataAnalysisDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Analytic", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        DataAnalysisDefinition DataAnalysisDefinitionMapper(IRDBDataReader reader)
        {
            DataAnalysisDefinition dataAnalysisDefinition = new DataAnalysisDefinition
            {
                DataAnalysisDefinitionId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<DataAnalysisDefinitionSettings>(reader.GetString(COL_Settings)),
            };
            return dataAnalysisDefinition;
        }
        #endregion

        #region IAnalyticReportDataManager

        public bool AreDataAnalysisDefinitionUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<DataAnalysisDefinition> GetDataAnalysisDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(DataAnalysisDefinitionMapper);
        }

        public bool Insert(DataAnalysisDefinition dataAnalysisDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataAnalysisDefinitionItem.Name);

            insertQuery.Column(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);
            insertQuery.Column(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            if (dataAnalysisDefinitionItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(DataAnalysisDefinition dataAnalysisDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);
            notExistsCondition.EqualsCondition(COL_Name).Value(dataAnalysisDefinitionItem.Name);

            updateQuery.Column(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            if (dataAnalysisDefinitionItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

    }
}
