using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
    public class DataAnalysisDefinitionDataManager : IDataAnalysisDefinitionDataManager
    {

        static string TABLE_NAME = "VR_Analytic_DataAnalysisDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";

        static DataAnalysisDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "DataAnalysisDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
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
            throw new NotImplementedException();
        }

        public List<DataAnalysisDefinition> GetDataAnalysisDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "vrDataAnalysisDefinition", null, true);
            selectQuery.SelectColumns().AllTableColumns("vrDataAnalysisDefinition");
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(DataAnalysisDefinitionMapper);
        }

        public bool Insert(DataAnalysisDefinition dataAnalysisDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists("vrDataAnalysisDefinition");
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

            var notExistsCondition = updateQuery.IfNotExists("vrDataAnalysisDefinition");
            notExistsCondition.NotEqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);
            notExistsCondition.EqualsCondition(COL_Name).Value(dataAnalysisDefinitionItem.Name);

            updateQuery.Column(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings));

            updateQuery.Where().EqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

    }
}
