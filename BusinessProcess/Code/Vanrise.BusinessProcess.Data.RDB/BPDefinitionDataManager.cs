using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPDefinitionDataManager : IBPDefinitionDataManager
    {
        static string TABLE_NAME = "bp_BPDefinition";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_FQTN = "FQTN";
        const string COL_VRWorkflowId = "VRWorkflowId";
        const string COL_Config = "Config";
        const string COL_CreatedTime = "CreatedTime";

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
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess_BPDefinition", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }

        public List<BPDefinition> GetBPDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bp");
            selectQuery.SelectColumns().AllTableColumns("bp");

            return queryContext.GetItems(BPDefinitionMapper);
        }


        public bool InsertBPDefinition(BPDefinition bpDefinition)
        {
            if (!bpDefinition.VRWorkflowId.HasValue)
                throw new NullReferenceException("bpDefinition.VRWorkflowId");

            string serializedConfiguration = bpDefinition.Configuration != null ? Vanrise.Common.Serializer.Serialize(bpDefinition.Configuration) : null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(bpDefinition.BPDefinitionID);
            insertQuery.Column(COL_Name).Value(bpDefinition.Name);
            insertQuery.Column(COL_Title).Value(bpDefinition.Title);
            insertQuery.Column(COL_VRWorkflowId).Value(bpDefinition.VRWorkflowId.Value);

            insertQuery.Column(COL_Config).Value(serializedConfiguration);

            queryContext.ExecuteNonQuery();

            return true;
        }

        public bool UpdateBPDefinition(BPDefinition bpDefinition)
        {
            string serializedConfiguration = bpDefinition.Configuration != null ? Vanrise.Common.Serializer.Serialize(bpDefinition.Configuration) : null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Title).Value(bpDefinition.Title);

            if (bpDefinition.VRWorkflowId.HasValue)
                updateQuery.Column(COL_VRWorkflowId).Value(bpDefinition.VRWorkflowId.Value);
            else
                updateQuery.Column(COL_VRWorkflowId).Null();

            updateQuery.Column(COL_Config).Value(serializedConfiguration);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(bpDefinition.BPDefinitionID);

            queryContext.ExecuteNonQuery();

            return true;
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
                bpDefinition.WorkflowType = Type.GetType(workflowTypeAsString);

            string config = reader.GetString(COL_Config);
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);

            return bpDefinition;
        }
    }
}