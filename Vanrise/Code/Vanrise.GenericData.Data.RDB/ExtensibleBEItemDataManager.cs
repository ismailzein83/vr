using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;
namespace Vanrise.GenericData.Data.RDB
{
    public class ExtensibleBEItemDataManager : IExtensibleBEItemDataManager
    {
        #region RDB

        static string TABLE_NAME = "genericdata_ExtensibleBEItem";
        static string TABLE_ALIAS = "extensibleBEItem";
        const string COL_ID = "ID";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static ExtensibleBEItemDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "ExtensibleBEItem",
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
        ExtensibleBEItem ExtensibleBEItemMapper(IRDBDataReader reader)
        {
            ExtensibleBEItem extensibleBEItem = Vanrise.Common.Serializer.Deserialize<ExtensibleBEItem>(reader.GetString(COL_Details));
            if (extensibleBEItem != null)
            {
                extensibleBEItem.ExtensibleBEItemId = reader.GetGuid(COL_ID);
            }
            return extensibleBEItem;
        }
        #endregion

        #region IExtensibleBEItemDataManager
        public bool AddExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            if (extensibleBEItem != null)
                insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(extensibleBEItem));
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreExtensibleBEItemUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<ExtensibleBEItem> GetExtensibleBEItems()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(ExtensibleBEItemMapper);
        }

        public bool UpdateExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (extensibleBEItem != null)
                updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(extensibleBEItem));
            else
                updateQuery.Column(COL_Details).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(extensibleBEItem.ExtensibleBEItemId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
