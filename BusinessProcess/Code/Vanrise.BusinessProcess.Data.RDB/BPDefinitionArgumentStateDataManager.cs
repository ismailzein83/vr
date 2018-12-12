using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPDefinitionArgumentStateDataManager : IBPDefinitionArgumentStateDataManager
    {
        static string TABLE_NAME = "bp_BPDefintionArgumentState";
        static string TABLE_ALIAS = "bpArgState";
        const string COL_BPDefinitionID = "BPDefinitionID";
        const string COL_InputArgument = "InputArgument";

        static BPDefinitionArgumentStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BPDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InputArgument, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPDefintionArgumentState",
                Columns = columns,
                IdColumnName = COL_BPDefinitionID,
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess_BPDefinitionArgumentState", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }

        public List<BPDefinitionArgumentState> GetBPDefinitionArgumentStates()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPDefinitionArgumentStateMapper);
        }

        public bool InsertOrUpdateBPDefinitionArgumentState(BPDefinitionArgumentState bpDefinitionArgumentState)
        {
            var serializedInputArguments = Serializer.Serialize(bpDefinitionArgumentState.InputArgument);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_InputArgument).Value(serializedInputArguments);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_BPDefinitionID).Value(bpDefinitionArgumentState.BPDefinitionID);

            var effectedRows = queryContext.ExecuteNonQuery();

            if (effectedRows <= 0)
            {
                var queryContext2 = new RDBQueryContext(GetDataProvider());

                var insertQuery = queryContext2.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_BPDefinitionID).Value(bpDefinitionArgumentState.BPDefinitionID);
                insertQuery.Column(COL_InputArgument).Value(serializedInputArguments);

                effectedRows = queryContext2.ExecuteNonQuery();
            }
            return effectedRows > 0;
        }

        BPDefinitionArgumentState BPDefinitionArgumentStateMapper(IRDBDataReader reader)
        {
            string arguments = reader.GetString(COL_InputArgument);

            return new BPDefinitionArgumentState
            {
                BPDefinitionID = reader.GetGuid(COL_BPDefinitionID),
                InputArgument = !string.IsNullOrWhiteSpace(arguments) ? Serializer.Deserialize<BaseProcessInputArgument>(arguments) : null
            };
        }
    }
}
