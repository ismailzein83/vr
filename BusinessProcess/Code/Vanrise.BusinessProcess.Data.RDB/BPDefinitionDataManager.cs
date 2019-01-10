using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPDefinitionDataManager : IBPDefinitionDataManager
    {
        static string TABLE_NAME = "bp_BPDefinition";
        static string TABLE_ALIAS = "bp";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_FQTN = "FQTN";
        const string COL_VRWorkflowId = "VRWorkflowId";
        const string COL_Config = "Config";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BPDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_FQTN, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
            columns.Add(COL_VRWorkflowId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Config, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessConfig", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }

        public List<BPDefinition> GetBPDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPDefinitionMapper);
        }

        public bool InsertBPDefinition(BPDefinition bpDefinition)
        {
            if (!bpDefinition.VRWorkflowId.HasValue)
                throw new NullReferenceException("bpDefinition.VRWorkflowId");

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Title).Value(bpDefinition.Title);

            insertQuery.Column(COL_ID).Value(bpDefinition.BPDefinitionID);
            insertQuery.Column(COL_Name).Value(bpDefinition.Name);
            insertQuery.Column(COL_Title).Value(bpDefinition.Title);
            insertQuery.Column(COL_VRWorkflowId).Value(bpDefinition.VRWorkflowId.Value);

            if (bpDefinition.Configuration != null)
                insertQuery.Column(COL_Config).Value(Serializer.Serialize(bpDefinition.Configuration));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateBPDefinition(BPDefinition bpDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(bpDefinition.BPDefinitionID);
            notExistsCondition.EqualsCondition(COL_Title).Value(bpDefinition.Title);

            updateQuery.Column(COL_Title).Value(bpDefinition.Title);

            if (bpDefinition.VRWorkflowId.HasValue)
                updateQuery.Column(COL_VRWorkflowId).Value(bpDefinition.VRWorkflowId.Value);
            else
                updateQuery.Column(COL_VRWorkflowId).Null();

            if (bpDefinition.Configuration != null)
                updateQuery.Column(COL_Config).Value(Serializer.Serialize(bpDefinition.Configuration));
            else
                updateQuery.Column(COL_Config).Null();

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(bpDefinition.BPDefinitionID);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreBPDefinitionsUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        BPDefinition BPDefinitionMapper(IRDBDataReader reader)
        {
            BPDefinition bpDefinition = new BPDefinition
            {
                BPDefinitionID = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                VRWorkflowId = reader.GetNullableGuid(COL_VRWorkflowId)
            };

            string workflowTypeAsString = reader.GetString(COL_FQTN);
            if (!string.IsNullOrEmpty(workflowTypeAsString))
            {
                try
                {
                    bpDefinition.WorkflowType = Type.GetType(workflowTypeAsString);
                }
                catch (Exception ex)
                {

                }
            }

            string config = reader.GetString(COL_Config);
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);

            return bpDefinition;
        }
    }
}