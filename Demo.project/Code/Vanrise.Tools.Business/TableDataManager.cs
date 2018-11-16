using Vanrise.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Tools.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Vanrise.Tools.Business
{
    public class TableDataManager
    {

        #region Public Methods

        TableDataQuery query = new TableDataQuery();
        public IDataRetrievalResult<TableDataDetails> GetFilteredTableData(DataRetrievalInput<TableDataQuery> input)
        {
            ITableDataDataManager tableDataDataManager = VRToolsFactory.GetDataManager<ITableDataDataManager>();

            Guid connectionId = input.Query.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;
            tableDataDataManager.Connection_String = connectionString;

            query = input.Query;
            string resultKey = input.ResultKey;
            List<TableData> allTableData = tableDataDataManager.GetTableData(input.Query.SchemaName, input.Query.TableName, input.Query.WhereCondition);
            Func<TableData, bool> filterExpression = (TableData) =>
            {
                return true;
            };

            VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();

            var cachedAccountWithSelectionHandling = bulkActionDraftManager.GetOrCreateCachedWithSelectionHandling<TableData, CacheManager>(ref resultKey, input.Query.BulkActionState, () =>
                 {
                     return allTableData.FindAllRecords(filterExpression);
                 }, (tableDatas) =>
             {
                     List<BulkActionItem> bulkActionItems = new List<BulkActionItem>();
                     foreach (var tableData in tableDatas)
                     {
                         string identifierKey = "";
                         foreach (var identifierColumn in input.Query.IdentifierColumns)
                         {
                             string key = identifierColumn.ColumnName;
                             if (tableData.FieldValues[key] != null)
                                 identifierKey += tableData.FieldValues[key] + "_";
                         }

                         bulkActionItems.Add(new BulkActionItem
                         {
                             ItemId = identifierKey.ToString()
                         });
                     }
                     return bulkActionItems;
                 });
            input.ResultKey = resultKey;

            return DataRetrievalManager.Instance.ProcessResult(input, allTableData.ToBigResult(input, filterExpression, TableDataDetailsMapper));

        }

        public IEnumerable<TableData> GetSelectedTableData(TableDataQuery query)
        {
            ITableDataDataManager tableDataDataManager = VRToolsFactory.GetDataManager<ITableDataDataManager>();

            Guid connectionId = query.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;
            tableDataDataManager.Connection_String = connectionString;

            List<TableData> allTableData = tableDataDataManager.GetTableData(query.SchemaName, query.TableName, query.WhereCondition);
            VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();
            IEnumerable<VRBulkActionDraft> bulkaActionFinalState = bulkActionDraftManager.GetVRBulkActionDrafts(query.BulkActionFinalState);

            List<TableData> selectedTableData = new List<TableData>();

            foreach (var row in allTableData)
            {

                string identifierKey = "";
                foreach (var identifierColumn in query.IdentifierColumns)
                {
                    string key = identifierColumn.ColumnName;
                    identifierKey += row.FieldValues[key] + "_";
                }
                if (bulkaActionFinalState.Any(x => x.ItemId == identifierKey))
                    selectedTableData.Add(row);
            }

            return selectedTableData;

        }



        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ITableDataDataManager tableDataDataManager = VRToolsFactory.GetDataManager<ITableDataDataManager>();
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return false;
            }
        }
        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        #endregion
        public TableDataDetails TableDataDetailsMapper(TableData tableData)
        {
            TableDataDetails tableDataDetails = new TableDataDetails();
            tableDataDetails.FieldValues = new Dictionary<string, object>();
            foreach (var field in tableData.FieldValues)
            {
                tableDataDetails.FieldValues.Add(field.Key, field.Value);

            }

            return tableDataDetails;
        }

    }
}
