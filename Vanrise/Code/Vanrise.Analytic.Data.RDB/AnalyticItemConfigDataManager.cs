using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
    public class AnalyticItemConfigDataManager : IAnalyticItemConfigDataManager
    {
        static string TABLE_NAME = "VR_Analytic_AnalyticItemConfig";
        static string TABLE_ALIAS = "vrAnalyticItemConfig";

        const string COL_ID = "ID";
        const string COL_TableId = "TableId";
        const string COL_ItemType = "ItemType";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Config = "Config";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static AnalyticItemConfigDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_TableId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ItemType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Config, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "AnalyticItemConfig",
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

        AnalyticItemConfig<T> AnalyticItemConfigReader<T>(IRDBDataReader reader) where T : class
        {
            return new AnalyticItemConfig<T>
            {
                AnalyticItemConfigId = reader.GetGuid(COL_ID),
                Config = Vanrise.Common.Serializer.Deserialize<T>(reader.GetString(COL_Config)),
                ItemType = (AnalyticItemType)reader.GetInt(COL_ItemType),
                Name = reader.GetString(COL_Name),
                TableId = reader.GetGuid(COL_TableId),
                Title = reader.GetString(COL_Title),
            };
        }

        #endregion

        #region IAnalyticItemConfigDataManager

        public bool AddAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(analyticItemConfig.Name);
            ifNotExists.EqualsCondition(COL_TableId).Value(analyticItemConfig.TableId);
            ifNotExists.EqualsCondition(COL_ItemType).Value((int)analyticItemConfig.ItemType);

            insertQuery.Column(COL_ID).Value(analyticItemConfig.AnalyticItemConfigId);
            insertQuery.Column(COL_Name).Value(analyticItemConfig.Name);
            insertQuery.Column(COL_Title).Value(analyticItemConfig.Title);
            insertQuery.Column(COL_TableId).Value(analyticItemConfig.TableId);
            insertQuery.Column(COL_ItemType).Value((int)analyticItemConfig.ItemType);
            if (analyticItemConfig.Config != null)
                insertQuery.Column(COL_Config).Value(Vanrise.Common.Serializer.Serialize(analyticItemConfig.Config));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreAnalyticItemConfigUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME,ref updateHandle);
        }

        public List<AnalyticItemConfig<T>> GetItemConfigs<T>(Guid tableId, AnalyticItemType itemType) where T : class
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_TableId).Value(tableId);
            whereCondition.EqualsCondition(COL_ItemType).Value((int)itemType);

            return queryContext.GetItems(AnalyticItemConfigReader<T>);
        }

        public bool UpdateAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(analyticItemConfig.AnalyticItemConfigId);
            notExistsCondition.EqualsCondition(COL_Name).Value(analyticItemConfig.Name);
            notExistsCondition.EqualsCondition(COL_TableId).Value(analyticItemConfig.TableId);
            notExistsCondition.EqualsCondition(COL_ItemType).Value((int)analyticItemConfig.ItemType);

            updateQuery.Column(COL_Name).Value(analyticItemConfig.Name);
            updateQuery.Column(COL_Title).Value(analyticItemConfig.Title);
            if (analyticItemConfig.Config != null)
                updateQuery.Column(COL_Config).Value(Vanrise.Common.Serializer.Serialize(analyticItemConfig.Config));
            else
                updateQuery.Column(COL_Config).Null();

            updateQuery.Column(COL_TableId).Value(analyticItemConfig.TableId);
            updateQuery.Column(COL_ItemType).Value((int)analyticItemConfig.ItemType);

            updateQuery.Where().EqualsCondition(COL_ID).Value(analyticItemConfig.AnalyticItemConfigId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

    }
}
