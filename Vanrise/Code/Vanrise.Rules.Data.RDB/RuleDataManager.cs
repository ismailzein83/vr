using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Data.RDB
{
    public class RuleDataManager : IRuleDataManager
    {


        static string TABLE_NAME = "rules_Rule";
        static string TABLE_ALIAS = "vrRule";
        const string COL_ID = "ID";
        const string COL_TypeID = "TypeID";
        const string COL_RuleDetails = "RuleDetails";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";
        static RuleDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_TypeID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RuleDetails, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "rules",
                DBTableName = "Rule",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime,
                CachePartitionColumnName = COL_TypeID
            });
        }


        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Rule", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        private Entities.Rule RuleMapper(IRDBDataReader reader)
        {
            var isDeleted = reader.GetNullableBoolean(COL_IsDeleted);
            var createdTime = reader.GetNullableDateTime(COL_CreatedTime);
            Entities.Rule instance = new Entities.Rule
            {
                RuleId = reader.GetInt(COL_ID),
                TypeId = reader.GetInt(COL_TypeID),
                RuleDetails = reader.GetString(COL_RuleDetails),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                IsDeleted = isDeleted.HasValue && isDeleted.Value?true:false,
                CreatedTime = createdTime.HasValue? createdTime.Value:DateTime.MinValue,
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
            };
            return instance;
        }

        #endregion

        #region public Methods

        public bool AddRule(Rules.Entities.Rule rule, out int ruleId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_TypeID).Value(rule.TypeId);
            insertQuery.Column(COL_RuleDetails).Value(rule.RuleDetails);
            insertQuery.Column(COL_BED).Value(rule.BED);
            if(rule.EED.HasValue)
            insertQuery.Column(COL_EED).Value(rule.EED.Value);
            if(rule.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(rule.CreatedBy.Value);
            if (rule.LastModifiedBy.HasValue)
               insertQuery.Column(COL_LastModifiedBy).Value(rule.LastModifiedBy.Value);

            insertQuery.AddSelectGeneratedId();
            var insertedId = queryContext.ExecuteScalar().NullableIntValue;
            if(insertedId.HasValue)
            {
                ruleId = (int)insertedId.Value;
                return true;
            }
            ruleId = -1;
            return false;
        }
        public bool AreRulesUpdated(int ruleTypeId, ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ruleTypeId, ref updateHandle);
        }
        public bool UpdateRule(Rules.Entities.Rule ruleEntity)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_TypeID).Value(ruleEntity.TypeId);
            updateQuery.Column(COL_RuleDetails).Value(ruleEntity.RuleDetails);
            updateQuery.Column(COL_BED).Value(ruleEntity.BED);
            if (ruleEntity.EED.HasValue)
                updateQuery.Column(COL_EED).Value(ruleEntity.EED.Value);
            if (ruleEntity.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(ruleEntity.LastModifiedBy.Value);
            updateQuery.Where().EqualsCondition(COL_ID).Value(ruleEntity.RuleId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        public IEnumerable<Rules.Entities.Rule> GetRulesByType(int ruleTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_TypeID).Value(ruleTypeId);
            return queryContext.GetItems(RuleMapper);
        }
        public int GetRuleTypeId(string ruleType)
        {
            return new RuleTypeDataManager().GetRuleTypeId(ruleType);
        }
        public List<Rules.Entities.RuleChanged> GetRulesChanged(int ruleTypeId)
        {
            return new RuleChangedDataManager().GetRulesChanged(ruleTypeId);
        }
        public Rules.Entities.RuleChanged GetRuleChanged(int ruleId, int ruleTypeId)
        {
            return new RuleChangedDataManager().GetRuleChanged(ruleId,ruleTypeId);
        }
        public void DeleteRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            new RuleChangedForProcessingDataManager().DeleteRuleChangedForProcessing(ruleId, ruleTypeId);
        }
        public void DeleteRulesChangedForProcessing(int ruleTypeId)
        {
            new RuleChangedForProcessingDataManager().DeleteRulesChangedForProcessing(ruleTypeId);
        }
        public Rules.Entities.RuleChanged GetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            return new RuleChangedForProcessingDataManager().GetRuleChangedForProcessing(ruleId, ruleTypeId);
        }
        public List<Rules.Entities.RuleChanged> GetRulesChangedForProcessing(int ruleTypeId)
        {
            return new RuleChangedForProcessingDataManager().GetRulesChangedForProcessing(ruleTypeId);
        }
        public bool SetDeleted(List<int> rulesIds, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsDeleted).Value(true);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            var where = updateQuery.Where();
            where.ListCondition(COL_ID, RDBListConditionOperator.IN, rulesIds);
            return queryContext.ExecuteNonQuery() > 0;
        }
        public bool DeleteRuleAndRuleChanged(int ruleId, int ruleTypeId, int lastModifiedBy, Rules.Entities.ActionType actionType, string initialRule)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsDeleted).Value(true);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(ruleId);
            updateQuery.Where().EqualsCondition(COL_TypeID).Value(ruleTypeId);

            new RuleChangedDataManager().SetDeleteRuleChangedQuery(queryContext, ruleId, ruleTypeId, actionType, initialRule);

            return queryContext.ExecuteNonQuery(true) > 0;
        }
        public Rules.Entities.RuleChanged FillAndGetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            return new RuleChangedForProcessingDataManager().FillAndGetRuleChangedForProcessing(ruleId, ruleTypeId);
        }
        public List<Rules.Entities.RuleChanged> FillAndGetRulesChangedForProcessing(int ruleTypeId)
        {
            return new RuleChangedForProcessingDataManager().FillAndGetRulesChangedForProcessing(ruleTypeId);
        }
        public bool UpdateRuleAndRuleChanged(Rules.Entities.Rule rule, Rules.Entities.ActionType actionType, string initialRule, string additionalInformation)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_TypeID).Value(rule.TypeId);
            updateQuery.Column(COL_RuleDetails).Value(rule.RuleDetails);
            updateQuery.Column(COL_BED).Value(rule.BED);
            if (rule.EED.HasValue)
                updateQuery.Column(COL_EED).Value(rule.EED.Value);
            if (rule.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(rule.LastModifiedBy.Value);
            updateQuery.Where().EqualsCondition(COL_ID).Value(rule.RuleId);

            RuleChangedDataManager ruleChangedDataManager = new RuleChangedDataManager();
            ruleChangedDataManager.SetUpdateRuleChangedQuery(queryContext, rule.RuleId, rule.TypeId, additionalInformation);
            ruleChangedDataManager.SetInsertRuleChangedQuery( queryContext, rule.RuleId, rule.TypeId,  actionType,  initialRule,  additionalInformation);
            return queryContext.ExecuteNonQuery(true) > 0;
        }
        public bool AddRuleAndRuleChanged(Rules.Entities.Rule rule, Rules.Entities.ActionType actionType, out int ruleId)
        {
            var dataProvidor = GetDataProvider();

            var queryContext = new RDBQueryContext(dataProvidor);
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_TypeID).Value(rule.TypeId);
            insertQuery.Column(COL_RuleDetails).Value(rule.RuleDetails);
            insertQuery.Column(COL_BED).Value(rule.BED);
            if (rule.EED.HasValue)
                insertQuery.Column(COL_EED).Value(rule.EED.Value);
            if (rule.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(rule.CreatedBy.Value);
            if (rule.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(rule.LastModifiedBy.Value);
            insertQuery.AddSelectGeneratedId();

            var insertedId = queryContext.ExecuteScalar(true).NullableIntValue;
            if (!insertedId.HasValue)
            {
                ruleId = -1;
                return false;
            }
            ruleId = (int)insertedId.Value;

            new RuleChangedDataManager().AddRuleChanged(ruleId, actionType, rule.TypeId);
            return true;
        }

        #endregion


    }
}
