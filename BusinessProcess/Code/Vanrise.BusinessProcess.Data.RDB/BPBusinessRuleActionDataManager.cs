using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPBusinessRuleActionDataManager : IBPBusinessRuleActionDataManager
    {
        static string TABLE_NAME = "bp_BPBusinessRuleAction";
        static string TABLE_ALIAS = "businessRuleAct";
        const string COL_ID = "ID";
        const string COL_Settings = "Settings";
        const string COL_BusinessRuleDefinitionId = "BusinessRuleDefinitionId";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BPBusinessRuleActionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BusinessRuleDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPBusinessRuleAction",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessConfig", "BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString");
        }

        public List<BPBusinessRuleAction> GetBPBusinessRuleActions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPBusinessRuleActionMapper);
        }

        public bool AreBPBusinessRuleActionsUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        BPBusinessRuleAction BPBusinessRuleActionMapper(IRDBDataReader reader)
        {
            return new BPBusinessRuleAction
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
