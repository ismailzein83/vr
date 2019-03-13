using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.Interconnect.Data.RDB
{
    public class InterconnectInvoiceComparisonTemplateDataManager : IInterconnectInvoiceComparisonTemplateDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_Invoice_InvoiceComparisonTemplate";
        static string TABLE_ALIAS = "ict";
        const string COL_ID = "ID";
        const string COL_InvoiceTypeId = "InvoiceTypeId";
        const string COL_PartnerId = "PartnerId";
        const string COL_Details = "Details";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static InterconnectInvoiceComparisonTemplateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_InvoiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerId, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Retail_Interconnect",
                DBTableName = "InvoiceComparisonTemplate",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Retail_Interconnect", "Retail_BE_DBConnStringKey", "RetailDBConnString");
        }
        #endregion

        #region Mappers
        InvoiceComparisonTemplate InvoiceCompareTemplateMapper(IRDBDataReader reader)
        {
            return new InvoiceComparisonTemplate()
            {
                InvoiceComparisonTemplateId = reader.GetLong(COL_ID),
                InvoiceTypeId = reader.GetGuid(COL_InvoiceTypeId),
                PartnerId = reader.GetString(COL_PartnerId),
                Details = Serializer.Deserialize<InvoiceComparisonTemplateDetails>(reader.GetString(COL_Details))
            };
        }
        #endregion

        #region IInvoiceComparisonTemplateDataManager
        public bool AreInvoiceComparisonTemplatesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<InvoiceComparisonTemplate> GetInvoiceCompareTemplates()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(InvoiceCompareTemplateMapper);
        }

        public bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceComparisonTemplate)
        {
            var queryContext1 = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext1.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_InvoiceTypeId).Value(invoiceComparisonTemplate.InvoiceTypeId);
            ifNotExists.EqualsCondition(COL_PartnerId).Value(invoiceComparisonTemplate.PartnerId);
            insertQuery.Column(COL_InvoiceTypeId).Value(invoiceComparisonTemplate.InvoiceTypeId);
            insertQuery.Column(COL_PartnerId).Value(invoiceComparisonTemplate.PartnerId);
            if (invoiceComparisonTemplate.Details != null)
                insertQuery.Column(COL_Details).Value(Serializer.Serialize(invoiceComparisonTemplate.Details));
            if (queryContext1.ExecuteNonQuery() > 0)
                return true;
            var queryContext2 = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext2.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (invoiceComparisonTemplate.Details != null)
                updateQuery.Column(COL_Details).Value(Serializer.Serialize(invoiceComparisonTemplate.Details));
            else
                updateQuery.Column(COL_Details).Null();
            var whereStatement = updateQuery.Where();
            whereStatement.EqualsCondition(COL_InvoiceTypeId).Value(invoiceComparisonTemplate.InvoiceTypeId);
            whereStatement.EqualsCondition(COL_PartnerId).Value(invoiceComparisonTemplate.PartnerId);
            return queryContext2.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
