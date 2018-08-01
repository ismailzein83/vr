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
        public static string TABLE_NAME = "VR_Invoice_BillingPeriodInfo";

        static BillingPeriodInfoDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("NextPeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "BillingPeriodInfo",
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

        public Entities.BillingPeriodInfo BillingPeriodInfoMapper(IRDBDataReader reader)
        {
            Entities.BillingPeriodInfo invoice = new Entities.BillingPeriodInfo
            {
                NextPeriodStart = reader.GetDateTimeWithNullHandling("NextPeriodStart"),
                InvoiceTypeId = reader.GetGuidWithNullHandling("InvoiceTypeId"),
                PartnerId = reader.GetString("PartnerId")
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
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);

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
            var ifQuery = queryContext.AddIfQuery();
            var ifExistsSelectQuery = ifQuery.IfCondition().ExistsCondition();
            ifExistsSelectQuery.From(BillingPeriodInfoDataManager.TABLE_NAME, "billPer");
            ifExistsSelectQuery.SelectColumns().Expression("dum").Value(1);

            var ifExistsSelectQuerywhere = ifExistsSelectQuery.Where();
            ifExistsSelectQuerywhere.EqualsCondition("InvoiceTypeID").Value(billingPeriodInfo.InvoiceTypeId);
            ifExistsSelectQuerywhere.EqualsCondition("PartnerID").Value(billingPeriodInfo.PartnerId);

            var updateQuery = ifQuery.ThenQuery().AddUpdateQuery();
            updateQuery.FromTable(BillingPeriodInfoDataManager.TABLE_NAME);
            updateQuery.Column("NextPeriodStart").Value(billingPeriodInfo.NextPeriodStart);
            var updateQueryAndCondition = updateQuery.Where();
            updateQueryAndCondition.EqualsCondition("InvoiceTypeID").Value(billingPeriodInfo.InvoiceTypeId);
            updateQueryAndCondition.EqualsCondition("PartnerID").Value(billingPeriodInfo.PartnerId);

            var insertQuery = ifQuery.ElseQuery().AddInsertQuery();
            insertQuery.IntoTable(BillingPeriodInfoDataManager.TABLE_NAME);
            insertQuery.Column("InvoiceTypeID").Value(billingPeriodInfo.InvoiceTypeId);
            insertQuery.Column("PartnerID").Value(billingPeriodInfo.PartnerId);
            insertQuery.Column("NextPeriodStart").Value(billingPeriodInfo.NextPeriodStart);
        }
    }
}
