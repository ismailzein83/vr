using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.BEBridge.Data.RDB
{
    public class BEReceiveDefinitionDataManager : IBEReceiveDefinitionDataManager
    {
        #region RDB
        static string TABLE_NAME = "VR_BEBridge_BEReceiveDefinition";
        static string TABLE_ALIAS = "definition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BEReceiveDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_BEBridge",
                DBTableName = "BEReceiveDefinition",
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
            return RDBDataProviderFactory.CreateProvider("VR_BEBridge", "ConfigurationDBConnString", "ConfigurationDBConnString");
        }
        #endregion

        #region Mappers
        BEReceiveDefinition BEReceiveDefinitionMapper(IRDBDataReader reader)
        {
            return new BEReceiveDefinition
            {
                BEReceiveDefinitionId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Common.Serializer.Deserialize<BEReceiveDefinitionSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion
        #region IBEReceiveDefinitionDataManager
        public bool AreBEReceiveDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<BEReceiveDefinition> GetBEReceiveDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(BEReceiveDefinitionMapper);
        }

        public bool Insert(BEReceiveDefinition statusDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(statusDefinitionItem.Name);
            insertQuery.Column(COL_ID).Value(statusDefinitionItem.BEReceiveDefinitionId);
            insertQuery.Column(COL_Name).Value(statusDefinitionItem.Name);
            if (statusDefinitionItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(statusDefinitionItem.Settings));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(BEReceiveDefinition beReceiveDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(beReceiveDefinition.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(beReceiveDefinition.BEReceiveDefinitionId);
            updateQuery.Column(COL_Name).Value(beReceiveDefinition.Name);
            if (beReceiveDefinition.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(beReceiveDefinition.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(beReceiveDefinition.BEReceiveDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
