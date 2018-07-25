using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceDataManager
    {
        public static string TABLE_NAME = "VR_Invoice_Invoice";

        static InvoiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("UserId", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("SettlementInvoiceId", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("SplitInvoiceGroupId", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceSettingID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier});
            columns.Add("SerialNumber", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add("FromDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ToDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IssueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("DueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add("PaidDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("LockDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("SentDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "Invoice",
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
    }
}
