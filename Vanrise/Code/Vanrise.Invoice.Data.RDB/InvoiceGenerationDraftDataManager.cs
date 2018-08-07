using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceGenerationDraftDataManager : IInvoiceGenerationDraftDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceGenerationDraft";

         static InvoiceGenerationDraftDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("InvoiceGenerationIdentifier", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("PartnerName", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add("FromDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ToDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("CustomPayload", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceGenerationDraft",
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

        private InvoiceGenerationDraft InvoiceGenerationDraftMapper(IRDBDataReader reader)
        {
            string customPayload = reader.GetString("CustomPayload");
            InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
            {
                InvoiceGenerationDraftId = reader.GetLong("ID"),
                InvoiceGenerationIdentifier = reader.GetGuid("invoiceGenerationIdentifier"),
                InvoiceTypeId = reader.GetGuid("InvoiceTypeID"),
                PartnerId = reader.GetString("PartnerID"),
                PartnerName = reader.GetString("PartnerName"),
                From = reader.GetDateTime("FromDate"),
                To = reader.GetDateTime("ToDate"),
                CustomPayload = !string.IsNullOrEmpty(customPayload) ? Vanrise.Common.Serializer.Deserialize(customPayload) : null
            };
            return invoiceGenerationDraft;
        }

        private InvoiceGenerationDraftSummary InvoiceGenerationDraftSummaryMapper(IRDBDataReader reader)
        {
            InvoiceGenerationDraftSummary invoiceGenerationDraftSummary = new InvoiceGenerationDraftSummary()
            {
                TotalCount = reader.GetInt("TotalCount"),
                MinimumFrom = reader.GetNullableDateTime("MinimumFrom"),
                MaximumTo = reader.GetNullableDateTime("MaximumTo"),
            };
            return invoiceGenerationDraftSummary;
        }

        #endregion

        #region IInvoiceGenerationDraftDataManager

        public List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "genDraft", null, true);
            selectQuery.SelectColumns().AllTableColumns("genDraft");

            selectQuery.Where().EqualsCondition("InvoiceGenerationIdentifier").Value(invoiceGenerationIdentifier);

            return queryContext.GetItems(InvoiceGenerationDraftMapper);
        }

        public bool InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.GenerateIdAndAssignToParameter("Id");

            insertQuery.Column("InvoiceGenerationIdentifier").Value(invoiceGenerationDraft.InvoiceGenerationIdentifier);
            insertQuery.Column("InvoiceTypeID").Value(invoiceGenerationDraft.InvoiceTypeId);
            insertQuery.Column("PartnerID").Value(invoiceGenerationDraft.PartnerId);
            insertQuery.Column("PartnerName").Value(invoiceGenerationDraft.PartnerName);
            insertQuery.Column("FromDate").Value(invoiceGenerationDraft.From);
            insertQuery.Column("ToDate").Value(invoiceGenerationDraft.To);
            if (invoiceGenerationDraft.CustomPayload != null)
                insertQuery.Column("CustomPayload").Value(Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload));

            insertedId = queryContext.ExecuteScalar().LongValue;
            return true;
        }

        public bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraftToEdit invoiceGenerationDraft)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("FromDate").Value(invoiceGenerationDraft.From);
            updateQuery.Column("ToDate").Value(invoiceGenerationDraft.To);
            updateQuery.Column("CustomPayload").Value(Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload));

            updateQuery.Where().EqualsCondition("ID").Value(invoiceGenerationDraft.InvoiceGenerationDraftId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public void DeleteInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition("ID").Value(invoiceGenerationDraftId);

            queryContext.ExecuteNonQuery();
        }

        public InvoiceGenerationDraft GetInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "genDraft", null, true);
            selectQuery.SelectColumns().AllTableColumns("genDraft");

            selectQuery.Where().EqualsCondition("ID").Value(invoiceGenerationDraftId);

            return queryContext.GetItem(InvoiceGenerationDraftMapper);
        }

        public void ClearInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition("InvoiceGenerationIdentifier").Value(invoiceGenerationIdentifier);

            queryContext.ExecuteNonQuery();
        }

        public InvoiceGenerationDraftSummary GetInvoiceGenerationDraftsSummary(Guid invoiceGenerationIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "genDraft", null, true);

            var aggregates = selectQuery.SelectAggregates();
            aggregates.Count("TotalCount");
            aggregates.Aggregate(RDBNonCountAggregateType.MIN, "FromDate", "MinimumFrom");
            aggregates.Aggregate(RDBNonCountAggregateType.MAX, "ToDate", "MaximumTo");

            selectQuery.Where().EqualsCondition("InvoiceGenerationIdentifier").Value(invoiceGenerationIdentifier);

            return queryContext.GetItem(InvoiceGenerationDraftSummaryMapper);
        }

        #endregion
    }
}
