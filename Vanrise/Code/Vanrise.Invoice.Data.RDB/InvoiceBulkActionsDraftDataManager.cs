using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceBulkActionsDraftDataManager : IInvoiceBulkActionsDraftDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceBulkActionDraft";

        const string COL_ID = "ID";
        const string COL_InvoiceBulkActionIdentifier = "InvoiceBulkActionIdentifier";
        internal const string COL_InvoiceTypeId = "InvoiceTypeId";
        internal const string COL_InvoiceId = "InvoiceId";
        const string COL_CreatedTime = "CreatedTime";

        static InvoiceBulkActionsDraftDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_InvoiceBulkActionIdentifier, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InvoiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InvoiceId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceBulkActionDraft",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        private InvoiceBulkActionsDraftSummary InvoiceBulkActionsDraftSummary(IRDBDataReader reader)
        {
            return new InvoiceBulkActionsDraftSummary()
            {
                TotalCount = reader.GetInt("TotalCount"),
                MinimumFrom = reader.GetNullableDateTime("MinimumFrom"),
                MaximumTo = reader.GetNullableDateTime("MaximumTo")
            };
        }

        #endregion

        #region IInvoiceBulkActionsDraftDataManager

        public void LoadInvoicesFromInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Action<Entities.Invoice> onInvoiceReady)
        {
            InvoiceDataManager invoiceDataManager = new InvoiceDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bulkActionDrft");

            invoiceDataManager.AddJoinInvoiceToBulkActionDraft(selectQuery.Join(), "bulkActionDrft", "inv");

            selectQuery.SelectColumns().AllTableColumns("inv");

            selectQuery.Where().EqualsCondition(COL_InvoiceBulkActionIdentifier).Value(invoiceBulkActionIdentifier);

            queryContext.ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        onInvoiceReady(invoiceDataManager.InvoiceMapper(reader));
                    }
                });
        }

        public Entities.InvoiceBulkActionsDraftSummary UpdateInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Guid invoiceTypeId, bool isAllInvoicesSelected, List<long> targetInvoicesIds)
        {
            InvoiceDataManager invoiceDataManager = new InvoiceDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var deleteWhereCondition = deleteQuery.Where();
            deleteWhereCondition.EqualsCondition(COL_InvoiceBulkActionIdentifier).Value(invoiceBulkActionIdentifier);
            if(isAllInvoicesSelected)
            {
                if (targetInvoicesIds != null)
                {
                    deleteWhereCondition.ListCondition(COL_InvoiceId, RDBListConditionOperator.IN, targetInvoicesIds);
                }
            }
            else
            {
                if(targetInvoicesIds != null)
                {
                    foreach (var invoiceId in targetInvoicesIds)
                    {
                        var insertQuery = queryContext.AddInsertQuery();
                        insertQuery.IntoTable(TABLE_NAME);
                        insertQuery.Column(COL_InvoiceBulkActionIdentifier).Value(invoiceBulkActionIdentifier);
                        insertQuery.Column(COL_InvoiceTypeId).Value(invoiceTypeId);
                        insertQuery.Column(COL_InvoiceId).Value(invoiceId);
                    }
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bulkActionDrft");

            invoiceDataManager.AddJoinInvoiceToBulkActionDraft(selectQuery.Join(), "bulkActionDrft", "inv");

            var selectAggregates = selectQuery.SelectAggregates();
            selectAggregates.Count("TotalCount");
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, "inv", InvoiceDataManager.COL_FromDate, "MinimumFrom");
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, "inv", InvoiceDataManager.COL_ToDate, "MaximumTo");

            selectQuery.Where().EqualsCondition(COL_InvoiceBulkActionIdentifier).Value(invoiceBulkActionIdentifier);

            return queryContext.GetItem(InvoiceBulkActionsDraftSummary);
        }

        public void ClearInvoiceBulkActionDrafts(Guid invoiceBulkActionIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_InvoiceBulkActionIdentifier).Value(invoiceBulkActionIdentifier);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        internal void DeleteBulkActionDrafts(Guid bulkActionDraftIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_InvoiceBulkActionIdentifier).Value(bulkActionDraftIdentifier);

            queryContext.ExecuteNonQuery();
        }

        internal RDBInsertQuery CreateInsertQuery(RDBQueryContext queryContext)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            return insertQuery;
        }
    }
}
