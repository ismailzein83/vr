using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities.ExpressionBuilder;

namespace Vanrise.GenericData.Data.RDB
{
    public class ExpressionBuilderConfigDataManager : IExpressionBuilderConfigDataManager
    {
        #region RDB

        static string TABLE_NAME = "genericdata_ExpressionBuilderConfig";
        static string TABLE_ALIAS = "expressionBuilder";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static ExpressionBuilderConfigDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "ExpressionBuilderConfig",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        #region Mappers
        ExpressionBuilderConfig ExpressionBuilderConfigMapper(IRDBDataReader reader)
        {
            ExpressionBuilderConfig expressionBuilderConfig = Vanrise.Common.Serializer.Deserialize<ExpressionBuilderConfig>(reader.GetString(COL_Details));
            if (expressionBuilderConfig != null)
            {
                expressionBuilderConfig.ExpressionBuilderConfigId = reader.GetInt(COL_ID);
                expressionBuilderConfig.Name = reader.GetString(COL_Name);
            }
            return expressionBuilderConfig;
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion
        public bool AreExpressionBuilderConfigUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<ExpressionBuilderConfig> GetExpressionBuilderTemplates()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(ExpressionBuilderConfigMapper);
        }
    }
}
