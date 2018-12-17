using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPBusinessRuleSetDataManager : IBPBusinessRuleSetDataManager
    {
        static string TABLE_NAME = "bp_BPBusinessRuleSet";
        static string TABLE_ALIAS = "businessRuleSet";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_ParentID = "ParentID";
        const string COL_Details = "Details";
        const string COL_BPDefinitionId = "BPDefinitionId";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BPBusinessRuleSetDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ParentID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BPDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPBusinessRuleSet",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess_BPBusinessRuleSet", "BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString");
        }

        public List<BPBusinessRuleSet> GetBPBusinessRuleSets()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPBusinessRuleSetMapper);
        }

        public bool AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj, out int bpBusinessRuleSetId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(businessRuleSetObj.Name);

            insertQuery.Column(COL_Name).Value(businessRuleSetObj.Name);

            if (businessRuleSetObj.ParentId.HasValue)
                insertQuery.Column(COL_ParentID).Value(businessRuleSetObj.ParentId.Value);

            if (businessRuleSetObj.Details != null)
                insertQuery.Column(COL_Details).Value(Serializer.Serialize(businessRuleSetObj.Details));

            insertQuery.Column(COL_BPDefinitionId).Value(businessRuleSetObj.BPDefinitionId);

            bpBusinessRuleSetId = queryContext.ExecuteScalar().IntWithNullHandlingValue;

            return bpBusinessRuleSetId > 0;
        }

        public bool UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(businessRuleSetObj.BPBusinessRuleSetId);
            notExistsCondition.EqualsCondition(COL_Name).Value(businessRuleSetObj.Name);

            updateQuery.Column(COL_Name).Value(businessRuleSetObj.Name);

            if (businessRuleSetObj.ParentId.HasValue)
                updateQuery.Column(COL_ParentID).Value(businessRuleSetObj.ParentId.Value);
            else
                updateQuery.Column(COL_ParentID).Null();

            if (businessRuleSetObj.Details != null)
                updateQuery.Column(COL_Details).Value(Serializer.Serialize(businessRuleSetObj.Details));
            else
                updateQuery.Column(COL_Details).Null();

            updateQuery.Column(COL_BPDefinitionId).Value(businessRuleSetObj.BPDefinitionId);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(businessRuleSetObj.BPBusinessRuleSetId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreBPBusinessRuleSetsUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        BPBusinessRuleSet BPBusinessRuleSetMapper(IRDBDataReader reader)
        {
            string details = reader.GetString(COL_Details);
            return new BPBusinessRuleSet
            {
                BPBusinessRuleSetId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                ParentId = reader.GetNullableInt(COL_ParentID),
                Details = Serializer.Deserialize<BPBusinessRuleSetDetails>(details),
                BPDefinitionId = reader.GetGuid(COL_BPDefinitionId)
            };
        }
    }
}