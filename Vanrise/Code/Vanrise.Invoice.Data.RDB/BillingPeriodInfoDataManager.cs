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
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "billPer")
                        .Where().And()
                                    .EqualsCondition("InvoiceTypeID", invoiceTypeId)
                                    .EqualsCondition("PartnerID", partnerId)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("billPer").EndColumns()
                        .EndSelect()
                        .GetItem(BillingPeriodInfoMapper);
        }

        public bool InsertOrUpdateBillingPeriodInfo(Entities.BillingPeriodInfo billingPeriodInfo)
        {
            new RDBQueryContext(GetDataProvider()).InsertOrUpdateBillingPeriodInfo(billingPeriodInfo)
                .ExecuteNonQuery();
            return true;
        }
    }
}
