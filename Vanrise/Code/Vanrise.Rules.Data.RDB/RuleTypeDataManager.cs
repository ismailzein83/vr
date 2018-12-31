using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Rules.Data.RDB
{
    public class RuleTypeDataManager
    {
        static string TABLE_NAME = "rules_RuleType";
        static string TABLE_ALIAS = "vrRuleType";
        const string COL_ID = "ID";
        const string COL_Type = "Type";

        static RuleTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "rules",
                DBTableName = "RuleType",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Rule", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        public int GetRuleTypeId(string ruleType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Type).Value(ruleType);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Type).Value(ruleType);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Column(COL_ID);
            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_Type).Value(ruleType);
            return queryContext.ExecuteScalar().IntValue;
        }
    }
}
