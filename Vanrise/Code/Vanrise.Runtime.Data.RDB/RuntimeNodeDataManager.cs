using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RuntimeNodeDataManager : IRuntimeNodeDataManager
    {
        static string TABLE_NAME = "runtime_RuntimeNode";
        static string TABLE_ALIAS = "RNode";

        const string COL_ID = "ID";
        const string COL_RuntimeNodeConfigurationID = "RuntimeNodeConfigurationID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";

        static RuntimeNodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_RuntimeNodeConfigurationID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RuntimeNode",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RuntimeConfig", "RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString");
        }

        #region Public Methods

        public bool AreRuntimeNodeUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public List<RuntimeNode> GetAllNodes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(RuntimeNodeMapper);
        }

        public bool Insert(RuntimeNode runtimeNode)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(runtimeNode.RuntimeNodeId);
            insertQuery.Column(COL_RuntimeNodeConfigurationID).Value(runtimeNode.RuntimeNodeConfigurationId);
            insertQuery.Column(COL_Name).Value(runtimeNode.Name);
            if (runtimeNode.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(runtimeNode.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(RuntimeNode runtimeNode)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(runtimeNode.Name);
            updateQuery.Column(COL_RuntimeNodeConfigurationID).Value(runtimeNode.RuntimeNodeConfigurationId);
            if (runtimeNode.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(runtimeNode.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            var notExistContext = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistContext.NotEqualsCondition(COL_ID).Value(runtimeNode.RuntimeNodeId);
            notExistContext.EqualsCondition(COL_Name).Value(runtimeNode.Name);

            updateQuery.Where().EqualsCondition(COL_ID).Value(runtimeNode.RuntimeNodeId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        #region Mappers

        private RuntimeNode RuntimeNodeMapper(IRDBDataReader reader)
        {
            var runtimeNode = new RuntimeNode
            {
                RuntimeNodeId = reader.GetGuid(COL_ID),
                RuntimeNodeConfigurationId = reader.GetGuid(COL_RuntimeNodeConfigurationID),
                Name = reader.GetString(COL_Name)
            };
            string serializedSettings = reader.GetString(COL_Settings);
            if (serializedSettings != null)
                runtimeNode.Settings = Serializer.Deserialize<RuntimeNodeSettings>(serializedSettings);
            return runtimeNode;
        }

        #endregion
    }
}
