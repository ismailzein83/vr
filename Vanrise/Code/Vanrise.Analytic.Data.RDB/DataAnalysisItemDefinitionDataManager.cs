using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
    public class DataAnalysisItemDefinitionDataManager : IDataAnalysisItemDefinitionDataManager
    {

        static string TABLE_NAME = "Analytic_DataAnalysisItemDefinition";
        static string TABLE_ALIAS = "vrDataAnalysisItemDefinition";

        const string COL_ID = "ID";
        const string COL_DataAnalysisDefinitionID = "DataAnalysisDefinitionID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static DataAnalysisItemDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_DataAnalysisDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "DataAnalysisItemDefinition",
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
        DataAnalysisItemDefinition DataAnalysisItemDefinitionMapper(IRDBDataReader reader)
        {
            DataAnalysisItemDefinition dataAnalysisItemDefinition = new DataAnalysisItemDefinition
            {
                DataAnalysisItemDefinitionId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<DataAnalysisItemDefinitionSettings>(reader.GetString(COL_Settings)),
                DataAnalysisDefinitionId = reader.GetGuid(COL_DataAnalysisDefinitionID),
            };
            return dataAnalysisItemDefinition;
        }
        #endregion

        #region IAnalyticItemConfigDataManager
        public bool AreDataAnalysisItemDefinitionUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<DataAnalysisItemDefinition> GetDataAnalysisItemDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(DataAnalysisItemDefinitionMapper);
        }

        public bool Insert(DataAnalysisItemDefinition dataAnalysisDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            ifNotExists.EqualsCondition(COL_DataAnalysisDefinitionID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);

            insertQuery.Column(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisItemDefinitionId);
            insertQuery.Column(COL_DataAnalysisDefinitionID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);
            insertQuery.Column(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            if (dataAnalysisDefinitionItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(DataAnalysisItemDefinition dataAnalysisDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisItemDefinitionId);
            notExistsCondition.EqualsCondition(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            notExistsCondition.EqualsCondition(COL_DataAnalysisDefinitionID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);

            updateQuery.Column(COL_Name).Value(dataAnalysisDefinitionItem.Name);
            updateQuery.Column(COL_DataAnalysisDefinitionID).Value(dataAnalysisDefinitionItem.DataAnalysisDefinitionId);
            if (dataAnalysisDefinitionItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataAnalysisDefinitionItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(dataAnalysisDefinitionItem.DataAnalysisItemDefinitionId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

    }
}
