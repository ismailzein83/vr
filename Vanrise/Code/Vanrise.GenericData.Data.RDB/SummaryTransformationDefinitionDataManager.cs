using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class SummaryTransformationDefinitionDataManager : ISummaryTransformationDefinitionDataManager
    {

        #region RDB
        static string TABLE_NAME = "genericdata_SummaryTransformationDefinition";
        static string TABLE_ALIAS = "summaryTransformationDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SummaryTransformationDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "SummaryTransformationDefinition",
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
        SummaryTransformationDefinition SummaryTransformationDefinitionMapper(IRDBDataReader reader)
        {
            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinition();
            string details = reader.GetString(COL_Details);
            if (details != null && details != string.Empty)
                summaryTransformationDefinition = Vanrise.Common.Serializer.Deserialize<SummaryTransformationDefinition>(details);

            summaryTransformationDefinition.SummaryTransformationDefinitionId = reader.GetGuid(COL_ID);

            return summaryTransformationDefinition;
        }
        #endregion

        #region ISummaryTransformationDefinitionDataManager
        public bool AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(summaryTransformationDefinition.Name);
            insertQuery.Column(COL_ID).Value(summaryTransformationDefinition.SummaryTransformationDefinitionId);
            insertQuery.Column(COL_Name).Value(summaryTransformationDefinition.Name);
            insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(summaryTransformationDefinition));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSummaryTransformationDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<SummaryTransformationDefinition> GetSummaryTransformationDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(SummaryTransformationDefinitionMapper);
        }

        public bool UpdateSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(summaryTransformationDefinition.SummaryTransformationDefinitionId);
            ifNotExists.EqualsCondition(COL_Name).Value(summaryTransformationDefinition.Name);
            updateQuery.Column(COL_Name).Value(summaryTransformationDefinition.Name);
            updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(summaryTransformationDefinition));
            updateQuery.Where().EqualsCondition(COL_ID).Value(summaryTransformationDefinition.SummaryTransformationDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
