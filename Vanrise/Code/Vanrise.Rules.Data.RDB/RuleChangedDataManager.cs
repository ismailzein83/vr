using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Data.RDB
{
    public class RuleChangedDataManager
    {
        static string TABLE_NAME = "rules_RuleChanged";
        static string TABLE_ALIAS = "vrRuleChanged";
        const string COL_ID = "ID";
        const string COL_RuleID = "RuleID";
        const string COL_RuleTypeID = "RuleTypeID";
        const string COL_ActionType = "ActionType";
        const string COL_InitialRule = "InitialRule";
        const string COL_AdditionalInformation = "AdditionalInformation";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";

        #region Private Methods

        static RuleChangedDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RuleID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RuleTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ActionType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_InitialRule, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_AdditionalInformation, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "rules",
                DBTableName = "RuleChanged",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Rule", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region public Methods
        public static RuleChanged RuleChangedMapper(IRDBDataReader reader)
        {
            RuleChanged ruleChanged = new RuleChanged
            {
                RuleChangedId = reader.GetInt(COL_ID),
                RuleId = reader.GetInt(COL_RuleID),
                RuleTypeId = reader.GetInt(COL_RuleTypeID),
                ActionType = (ActionType)reader.GetInt(COL_ActionType),
                InitialRule = reader.GetString(COL_InitialRule),
                AdditionalInformation =reader.GetString(COL_AdditionalInformation),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
            };
            return ruleChanged;
        }
        public List<RuleChanged> GetRulesChanged(int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null,true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
            return queryContext.GetItems(RuleChangedMapper);
        }
        public RuleChanged GetRuleChanged(int ruleId, int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
            return queryContext.GetItem(RuleChangedMapper);
        }
        public void AddRuleChanged(int ruleId, Rules.Entities.ActionType actionType, int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_RuleID).Value(ruleId);
            insertQuery.Column(COL_RuleTypeID).Value(ruleTypeId);
            insertQuery.Column(COL_ActionType).Value((int)actionType);
            queryContext.ExecuteNonQuery();
        }
        public void SetDeleteRuleChangedQuery(RDBQueryContext queryContext, int ruleId, int ruleTypeId, Rules.Entities.ActionType actionType, string initialRule)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_ActionType).Value((int)actionType);
            updateQuery.Column(COL_InitialRule).Value(initialRule);
            updateQuery.Where().EqualsCondition(COL_RuleID).Value(ruleId);
            updateQuery.Where().EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);

            var insertQuery = queryContext.AddInsertQuery();

            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_RuleID).Value(ruleId);
            ifNotExists.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);

            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_RuleID).Value(ruleId);
            insertQuery.Column(COL_RuleTypeID).Value(ruleTypeId);
            insertQuery.Column(COL_ActionType).Value((int)actionType);
            insertQuery.Column(COL_InitialRule).Value(initialRule);
        }
        public void SetInsertRuleChangedQuery(RDBQueryContext queryContext, int ruleID, int ruleTypeID,  ActionType actionType, string initialRule,string additionalInformation)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_RuleID).Value(ruleID);
            insertQuery.Column(COL_RuleTypeID).Value(ruleTypeID);
            insertQuery.Column(COL_ActionType).Value((int)actionType);
            insertQuery.Column(COL_InitialRule).Value(initialRule);
            insertQuery.Column(COL_AdditionalInformation).Value(additionalInformation);
            var isNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            isNotExists.EqualsCondition(COL_RuleID).Value(ruleID);
            isNotExists.EqualsCondition(COL_RuleTypeID).Value(ruleTypeID);
        }
        public void SetUpdateRuleChangedQuery(RDBQueryContext queryContext, int ruleId, int ruleTypeId, string additionalInformation)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_AdditionalInformation).Value(additionalInformation);
            var whereCondition = updateQuery.Where();
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }
        public void SetDeleteRuleChanged(RDBQueryContext queryContext, int ruleId, int ruleTypeId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereCondition = deleteQuery.Where();
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }
        public void SetDeleteRulesChanged(RDBQueryContext queryContext, int ruleTypeId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereCondition = deleteQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }

        public void SetSelectRuleChanged(RDBSelectQuery selectQuery, int ruleId, int ruleTypeId, string colIDAlias, string ruleIDAlias, string ruleTypeIDAlias, string actionTypeAlias, string initialRuleAlias
            , string additionalInformationAlias, string createdTimeAlias)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectedColumns = selectQuery.SelectColumns();
            selectedColumns.Column(COL_ID, colIDAlias);
            selectedColumns.Column(COL_RuleID, ruleIDAlias);
            selectedColumns.Column(COL_RuleTypeID, ruleTypeIDAlias);
            selectedColumns.Column(COL_ActionType, actionTypeAlias);
            selectedColumns.Column(COL_InitialRule, initialRuleAlias);
            selectedColumns.Column(COL_AdditionalInformation, additionalInformationAlias);
            selectedColumns.Column(COL_CreatedTime, createdTimeAlias);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }
        public void SetSelectRuleChanged(RDBSelectQuery selectQuery, int ruleTypeId, string colIDAlias, string ruleIDAlias, string ruleTypeIDAlias, string actionTypeAlias, string initialRuleAlias
            , string additionalInformationAlias, string createdTimeAlias)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectedColumns = selectQuery.SelectColumns();
            selectedColumns.Column(COL_ID, colIDAlias);
            selectedColumns.Column(COL_RuleID, ruleIDAlias);
            selectedColumns.Column(COL_RuleTypeID, ruleTypeIDAlias);
            selectedColumns.Column(COL_ActionType, actionTypeAlias);
            selectedColumns.Column(COL_InitialRule, initialRuleAlias);
            selectedColumns.Column(COL_AdditionalInformation, additionalInformationAlias);
            selectedColumns.Column(COL_CreatedTime, createdTimeAlias);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }
        #endregion
    }
}
