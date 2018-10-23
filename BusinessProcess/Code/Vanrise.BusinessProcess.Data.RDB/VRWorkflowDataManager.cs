using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class VRWorkflowDataManager : IVRWorkflowDataManager
    {
        static string TABLE_NAME = "bp_VRWorkflow";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";


        static VRWorkflowDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "VRWorkflow",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess_VRWorkflow", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }

        public List<VRWorkflow> GetVRWorkflows()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "wf");
            selectQuery.SelectColumns().AllTableColumns("wf");

            return queryContext.GetItems(VRWorkflowMapper);
        }

        public bool InsertVRWorkflow(VRWorkflowToAdd vrWorkflow, Guid vrWorkflowId, int createdBy)
        {
            string serializedSettings = vrWorkflow.Settings != null ? Vanrise.Common.Serializer.Serialize(vrWorkflow.Settings) : null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(vrWorkflowId);
            insertQuery.Column(COL_Name).Value(vrWorkflow.Name);
            insertQuery.Column(COL_Title).Value(vrWorkflow.Title);
            insertQuery.Column(COL_Settings).Value(serializedSettings);
            insertQuery.Column(COL_CreatedBy).Value(createdBy);
            insertQuery.Column(COL_LastModifiedBy).Value(createdBy);

            queryContext.ExecuteNonQuery();

            return true;
        }

        public bool UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow, int lastModifiedBy)
        {
            string serializedSettings = vrWorkflow.Settings != null ? Vanrise.Common.Serializer.Serialize(vrWorkflow.Settings) : null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_Name).Value(vrWorkflow.Name);
            updateQuery.Column(COL_Title).Value(vrWorkflow.Title);
            updateQuery.Column(COL_Settings).Value(serializedSettings);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(vrWorkflow.VRWorkflowId);

            queryContext.ExecuteNonQuery();

            return true;
        }

        VRWorkflow VRWorkflowMapper(IRDBDataReader reader)
        {
            VRWorkflow vrWorkflow = new VRWorkflow
            {
                VRWorkflowId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                CreatedBy = reader.GetInt(COL_CreatedBy),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                LastModifiedBy = reader.GetInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime)
            };

            string settings = reader.GetStringWithEmptyHandling(COL_Settings);
            if (!string.IsNullOrEmpty(settings))
                vrWorkflow.Settings = Serializer.Deserialize<VRWorkflowSettings>(settings);

            return vrWorkflow;
        }
    }
}