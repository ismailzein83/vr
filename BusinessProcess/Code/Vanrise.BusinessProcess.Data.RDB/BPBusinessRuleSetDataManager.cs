﻿using System.Collections.Generic;
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

        static BPBusinessRuleSetDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ParentID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BPDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPBusinessRuleSet",
                Columns = columns,
                IdColumnName = COL_ID
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
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPBusinessRuleSetMapper);
        }

        public bool AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj, out int bpBusinessRuleSetId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_Name).Value(businessRuleSetObj.Name);

            if (businessRuleSetObj.ParentId.HasValue)
                insertQuery.Column(COL_ParentID).Value(businessRuleSetObj.ParentId.Value);

            if (businessRuleSetObj.Details != null)
                insertQuery.Column(COL_Details).Value(Serializer.Serialize(businessRuleSetObj.Details));

            insertQuery.Column(COL_BPDefinitionId).Value(businessRuleSetObj.BPDefinitionId);

            bpBusinessRuleSetId = queryContext.ExecuteScalar().IntWithNullHandlingValue;

            return bpBusinessRuleSetId != 0;
        }

        public bool UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

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

        BPBusinessRuleSet BPBusinessRuleSetMapper(IRDBDataReader reader)
        {
            string details = reader.GetString(COL_Details);
            return new BPBusinessRuleSet
            {
                BPBusinessRuleSetId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                ParentId = reader.GetNullableInt(COL_ParentID),
                Details = !string.IsNullOrEmpty(details) ? Serializer.Deserialize<BPBusinessRuleSetDetails>(details) : null,
                BPDefinitionId = reader.GetGuid(COL_BPDefinitionId)
            };
        }
    }
}