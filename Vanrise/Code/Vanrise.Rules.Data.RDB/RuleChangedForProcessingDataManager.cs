using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Rules.Entities;
using Vanrise.Entities;
namespace Vanrise.Rules.Data.RDB
{
    public class RuleChangedForProcessingDataManager
    {
        static string TABLE_NAME = "rules_RuleChangedForProcessing";
        static string TABLE_ALIAS = "vrRuleChangedForProcessing";

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
       
        static RuleChangedForProcessingDataManager()
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
                DBTableName = "RuleChangedForProcessing",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
       
        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Rule", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region public Methods

        public void DeleteRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereCondition = deleteQuery.Where();
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
            queryContext.ExecuteNonQuery();
        }
        public void DeleteRulesChangedForProcessing(int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereCondition = deleteQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
            queryContext.ExecuteNonQuery();
        }
        public RuleChanged GetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            SetRuleChangedForProcessing(queryContext, ruleId, ruleTypeId);
            return queryContext.GetItem(RuleChangedDataManager.RuleChangedMapper);
        }
        public List<RuleChanged> GetRulesChangedForProcessing(int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            SetRulesChangedForProcessing(queryContext, ruleTypeId);
            return queryContext.GetItems(RuleChangedDataManager.RuleChangedMapper);
        }
        public Rules.Entities.RuleChanged FillAndGetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            RuleChangedDataManager ruleChangedDataManager = new RuleChangedDataManager();
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var fromSelect = insertQuery.FromSelect();
            ruleChangedDataManager.SetSelectRuleChanged(fromSelect, ruleId, ruleTypeId ,COL_ID, COL_RuleID, COL_RuleTypeID, COL_ActionType, COL_InitialRule, COL_AdditionalInformation, COL_CreatedTime);
            ruleChangedDataManager.SetDeleteRuleChanged(queryContext, ruleId, ruleTypeId);
            SetRuleChangedForProcessing(queryContext, ruleId, ruleTypeId);
            return queryContext.GetItem(RuleChangedDataManager.RuleChangedMapper);
        }
        public List<RuleChanged> FillAndGetRulesChangedForProcessing(int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            RuleChangedDataManager ruleChangedDataManager = new RuleChangedDataManager();
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var fromSelect = insertQuery.FromSelect();
            ruleChangedDataManager.SetSelectRuleChanged(fromSelect, ruleTypeId, COL_ID, COL_RuleID, COL_RuleTypeID, COL_ActionType, COL_InitialRule, COL_AdditionalInformation, COL_CreatedTime);
            ruleChangedDataManager.SetDeleteRulesChanged(queryContext, ruleTypeId);
            SetRulesChangedForProcessing(queryContext, ruleTypeId);
            return queryContext.GetItems(RuleChangedDataManager.RuleChangedMapper);
        }

        public void SetRuleChangedForProcessing(RDBQueryContext queryContext, int ruleId, int ruleTypeId)
        {
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
            whereCondition.EqualsCondition(COL_RuleID).Value(ruleId);
        }
        public void SetRulesChangedForProcessing(RDBQueryContext queryContext, int ruleTypeId)
        {
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_RuleTypeID).Value(ruleTypeId);
        }
        #endregion

    }
}
