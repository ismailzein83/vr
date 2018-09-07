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

        const string COL_ID = "ID";
        const string COL_InvoiceGenerationIdentifier = "InvoiceGenerationIdentifier";
        const string COL_InvoiceTypeId = "InvoiceTypeId";
        const string COL_PartnerID = "PartnerID";
        const string COL_PartnerName = "PartnerName";
        const string COL_FromDate = "FromDate";
        const string COL_ToDate = "ToDate";
        const string COL_CustomPayload = "CustomPayload";
        const string COL_CreatedTime = "CreatedTime";

         static InvoiceGenerationDraftDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_InvoiceGenerationIdentifier, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InvoiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_PartnerName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_FromDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ToDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CustomPayload, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceGenerationDraft",
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

        private InvoiceGenerationDraft InvoiceGenerationDraftMapper(IRDBDataReader reader)
        {
            string customPayload = reader.GetString(COL_CustomPayload);
            InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
            {
                InvoiceGenerationDraftId = reader.GetLong(COL_ID),
                InvoiceGenerationIdentifier = reader.GetGuid(COL_InvoiceGenerationIdentifier),
                InvoiceTypeId = reader.GetGuid(COL_InvoiceTypeId),
                PartnerId = reader.GetString(COL_PartnerID),
                PartnerName = reader.GetString(COL_PartnerName),
                From = reader.GetDateTime(COL_FromDate),
                To = reader.GetDateTime(COL_ToDate),
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

            selectQuery.Where().EqualsCondition(COL_InvoiceGenerationIdentifier).Value(invoiceGenerationIdentifier);

            return queryContext.GetItems(InvoiceGenerationDraftMapper);
        }

        public bool InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_InvoiceGenerationIdentifier).Value(invoiceGenerationDraft.InvoiceGenerationIdentifier);
            insertQuery.Column(COL_InvoiceTypeId).Value(invoiceGenerationDraft.InvoiceTypeId);
            insertQuery.Column(COL_PartnerID).Value(invoiceGenerationDraft.PartnerId);
            insertQuery.Column(COL_PartnerName).Value(invoiceGenerationDraft.PartnerName);
            insertQuery.Column(COL_FromDate).Value(invoiceGenerationDraft.From);
            insertQuery.Column(COL_ToDate).Value(invoiceGenerationDraft.To);
            if (invoiceGenerationDraft.CustomPayload != null)
                insertQuery.Column(COL_CustomPayload).Value(Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload));

            insertedId = queryContext.ExecuteScalar().LongValue;
            return true;
        }

        public bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraftToEdit invoiceGenerationDraft)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_FromDate).Value(invoiceGenerationDraft.From);
            updateQuery.Column(COL_ToDate).Value(invoiceGenerationDraft.To);
            updateQuery.Column(COL_CustomPayload).Value(Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload));

            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceGenerationDraft.InvoiceGenerationDraftId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public void DeleteInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(invoiceGenerationDraftId);

            queryContext.ExecuteNonQuery();
        }

        public InvoiceGenerationDraft GetInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "genDraft", null, true);
            selectQuery.SelectColumns().AllTableColumns("genDraft");

            selectQuery.Where().EqualsCondition(COL_ID).Value(invoiceGenerationDraftId);

            return queryContext.GetItem(InvoiceGenerationDraftMapper);
        }

        public void ClearInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_InvoiceGenerationIdentifier).Value(invoiceGenerationIdentifier);

            queryContext.ExecuteNonQuery();
        }

        public InvoiceGenerationDraftSummary GetInvoiceGenerationDraftsSummary(Guid invoiceGenerationIdentifier)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "genDraft", null, true);

            var aggregates = selectQuery.SelectAggregates();
            aggregates.Count("TotalCount");
            aggregates.Aggregate(RDBNonCountAggregateType.MIN, COL_FromDate, "MinimumFrom");
            aggregates.Aggregate(RDBNonCountAggregateType.MAX, COL_ToDate, "MaximumTo");

            selectQuery.Where().EqualsCondition(COL_InvoiceGenerationIdentifier).Value(invoiceGenerationIdentifier);

            return queryContext.GetItem(InvoiceGenerationDraftSummaryMapper);
        }

        #endregion
    }
}
