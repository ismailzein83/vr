using Vanrise.DevTools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.DevTools.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Vanrise.DevTools.Business
{
    public class VRGeneratedScriptTableDataManager
    {

        #region Public Methods

        VRGeneratedScriptTableDataQuery query = new VRGeneratedScriptTableDataQuery();
        public IDataRetrievalResult<VRGeneratedScriptTableDataDetails> GetFilteredTableData(DataRetrievalInput<VRGeneratedScriptTableDataQuery> input)
        {
            IVRGeneratedScriptTableDataDataManager tableDataDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptTableDataDataManager>();

            Guid connectionId = input.Query.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;
            tableDataDataManager.Connection_String = connectionString;

            query = input.Query;
            string resultKey = input.ResultKey;
            List<GeneratedScriptItemTableRow> allTableData = tableDataDataManager.GetTableData(input.Query.SchemaName, input.Query.TableName, input.Query.WhereCondition);
            Func<GeneratedScriptItemTableRow, bool> filterExpression = (TableData) =>
            {
                return true;
            };

            VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();

            var cachedAccountWithSelectionHandling = bulkActionDraftManager.GetOrCreateCachedWithSelectionHandling<GeneratedScriptItemTableRow, CacheManager>(ref resultKey, input.Query.BulkActionState, () =>
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

        public IEnumerable<GeneratedScriptItemTableRow> GetSelectedTableData(VRGeneratedScriptTableDataQuery query)
        {
            IVRGeneratedScriptTableDataDataManager tableDataDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptTableDataDataManager>();

            Guid connectionId = query.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;
            tableDataDataManager.Connection_String = connectionString;

            List<GeneratedScriptItemTableRow> allTableData = tableDataDataManager.GetTableData(query.SchemaName, query.TableName, query.WhereCondition);
            VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();
            IEnumerable<VRBulkActionDraft> bulkaActionFinalState = bulkActionDraftManager.GetVRBulkActionDrafts(query.BulkActionFinalState);

            List<GeneratedScriptItemTableRow> selectedTableData = new List<GeneratedScriptItemTableRow>();

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
            IVRGeneratedScriptTableDataDataManager tableDataDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptTableDataDataManager>();
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
        public VRGeneratedScriptTableDataDetails TableDataDetailsMapper(GeneratedScriptItemTableRow tableData)
        {
            VRGeneratedScriptTableDataDetails tableDataDetails = new VRGeneratedScriptTableDataDetails();
            tableDataDetails.FieldValues = new Dictionary<string, object>();
            foreach (var field in tableData.FieldValues)
            {
                tableDataDetails.FieldValues.Add(field.Key, field.Value);

            }

            return tableDataDetails;
        }

    }
}
