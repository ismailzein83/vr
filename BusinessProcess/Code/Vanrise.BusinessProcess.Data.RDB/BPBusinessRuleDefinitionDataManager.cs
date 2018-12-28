using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPBusinessRuleDefinitionDataManager : IBPBusinessRuleDefinitionDataManager
    {
        #region Constructor

        static string TABLE_NAME = "bp_BPBusinessRuleDefinition";
        static string TABLE_ALIAS = "BusinessRuleDef";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_BPDefintionId = "BPDefintionId";
        const string COL_Settings = "Settings";
        const string COL_Rank = "Rank";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BPBusinessRuleDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_BPDefintionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Rank, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPBusinessRuleDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessConfig", "BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString");
        }

        #region Public Methods

        public List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions()
        {
            var query = new RDBQueryContext(GetDataProvider());
            var selectQuery = query.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Rank, RDBSortDirection.ASC);

            return query.GetItems(BPBusinessRuleDefintionMapper);
        }

        public bool AreBPBusinessRuleDefinitionsUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }
        #endregion

        #region Mappers
        BPBusinessRuleDefinition BPBusinessRuleDefintionMapper(IRDBDataReader reader)
        {
            var businessRuleDefinition = new BPBusinessRuleDefinition
            {
                BPBusinessRuleDefinitionId = reader.GetGuid(COL_ID),
                Settings = Serializer.Deserialize<BPBusinessRuleSettings>(reader.GetString(COL_Settings)),
                BPDefintionId = reader.GetGuid(COL_BPDefintionId),
                Name = reader.GetString(COL_Name),
                Rank = reader.GetInt(COL_Rank)
            };

            return businessRuleDefinition;
        }
        #endregion
    }
}
