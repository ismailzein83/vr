using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceBulkActionsDraftDataManager : IInvoiceBulkActionsDraftDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceBulkActionDraft";

        static InvoiceBulkActionsDraftDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("InvoiceBulkActionIdentifier", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceId", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceBulkActionDraft",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        #endregion

        #region IInvoiceBulkActionsDraftDataManager

        public void LoadInvoicesFromInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Action<Entities.Invoice> onInvoiceReady)
        {
            InvoiceDataManager invoiceDataManager = new InvoiceDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bulkActionDrft");
            selectQuery.SelectColumns().AllTableColumns("inv");

            invoiceDataManager.AddJoinInvoiceToBulkActionDraft(selectQuery.Join(), "bulkActionDrft", "inv");

            selectQuery.Where().EqualsCondition("InvoiceBulkActionIdentifier").Value(invoiceBulkActionIdentifier);

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
            throw new NotImplementedException();
        }

        public void ClearInvoiceBulkActionDrafts(Guid invoiceBulkActionIdentifier)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal void DeleteBulkActionDrafts(Guid bulkActionDraftIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition("InvoiceBulkActionIdentifier").Value(bulkActionDraftIdentifier);

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
