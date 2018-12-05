using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPBusinessRuleActionDataManager : IBPBusinessRuleActionDataManager
    {
        static string TABLE_NAME = "bp_BPBusinessRuleAction";
        static string TABLE_ALIAS = "businessRuleAct";
        const string COL_ID = "ID";
        const string COL_Settings = "Settings";
        const string COL_BusinessRuleDefinitionId = "BusinessRuleDefinitionId";

        static BPBusinessRuleActionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BusinessRuleDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPBusinessRuleAction",
                Columns = columns,
                IdColumnName = COL_ID,
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess_BPBusinessRuleAction", "BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString");
        }

        public List<BPBusinessRuleAction> GetBPBusinessRuleActions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPBusinessRuleActionMapper);
        }

        BPBusinessRuleAction BPBusinessRuleActionMapper(IRDBDataReader reader)
        {
            return  new BPBusinessRuleAction
            {
                BPBusinessRuleActionId = reader.GetInt(COL_ID),
                Details = new BPBusinessRuleActionDetails
                {
                    BPBusinessRuleDefinitionId = reader.GetGuid(COL_BusinessRuleDefinitionId),
                    Settings = Serializer.Deserialize<BPBusinessRuleActionSettings>(reader.GetString(COL_Settings))
                }
            };
        }
    }
}
