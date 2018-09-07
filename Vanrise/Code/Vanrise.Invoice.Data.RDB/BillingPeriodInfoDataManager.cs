using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public class BillingPeriodInfoDataManager : IBillingPeriodInfoDataManager
    {
        static string TABLE_NAME = "VR_Invoice_BillingPeriodInfo";

        const string COL_InvoiceTypeId = "InvoiceTypeId";
        const string COL_PartnerId = "PartnerId";
        const string COL_NextPeriodStart = "NextPeriodStart";
        const string COL_CreatedTime = "CreatedTime";

        static BillingPeriodInfoDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_InvoiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_NextPeriodStart, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "BillingPeriodInfo",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        public Entities.BillingPeriodInfo BillingPeriodInfoMapper(IRDBDataReader reader)
        {
            Entities.BillingPeriodInfo invoice = new Entities.BillingPeriodInfo
            {
                NextPeriodStart = reader.GetDateTimeWithNullHandling(COL_NextPeriodStart),
                InvoiceTypeId = reader.GetGuidWithNullHandling(COL_InvoiceTypeId),
                PartnerId = reader.GetString(COL_PartnerId)
            };
            return invoice;
        }

        #endregion

        public Entities.BillingPeriodInfo GetBillingPeriodInfoById(string partnerId, Guid invoiceTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "billPer");
            selectQuery.SelectColumns().AllTableColumns("billPer");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeId).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerId).Value(partnerId);

            return queryContext.GetItem(BillingPeriodInfoMapper);
        }

        public bool InsertOrUpdateBillingPeriodInfo(Entities.BillingPeriodInfo billingPeriodInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            AddInsertOrUpdateBillingPeriodInfo(queryContext, billingPeriodInfo);

            queryContext.ExecuteNonQuery();
            return true;
        }

        public void AddInsertOrUpdateBillingPeriodInfo(RDBQueryContext queryContext, Entities.BillingPeriodInfo billingPeriodInfo)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(BillingPeriodInfoDataManager.TABLE_NAME);
            insertQuery.Column(COL_InvoiceTypeId).Value(billingPeriodInfo.InvoiceTypeId);
            insertQuery.Column(COL_PartnerId).Value(billingPeriodInfo.PartnerId);
            insertQuery.Column(COL_NextPeriodStart).Value(billingPeriodInfo.NextPeriodStart);

            var notExistsCondition = insertQuery.IfNotExists("billPer");
            notExistsCondition.EqualsCondition(COL_InvoiceTypeId).Value(billingPeriodInfo.InvoiceTypeId);
            notExistsCondition.EqualsCondition(COL_PartnerId).Value(billingPeriodInfo.PartnerId);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BillingPeriodInfoDataManager.TABLE_NAME);
            updateQuery.Column(COL_NextPeriodStart).Value(billingPeriodInfo.NextPeriodStart);
            var updateQueryAndCondition = updateQuery.Where();
            updateQueryAndCondition.EqualsCondition(COL_InvoiceTypeId).Value(billingPeriodInfo.InvoiceTypeId);
            updateQueryAndCondition.EqualsCondition(COL_PartnerId).Value(billingPeriodInfo.PartnerId);
        }
    }
}
