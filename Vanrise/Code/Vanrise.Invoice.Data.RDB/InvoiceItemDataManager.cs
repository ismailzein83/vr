using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceItemDataManager 
    {
        public static string TABLE_NAME = "VR_Invoice_InvoiceItem";

        static InvoiceItemDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("InvoiceID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("ItemSetName", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("Name", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceItem",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        string _storageConnectionStringKey;
        public string StorageConnectionStringKey
        {
            set { this._storageConnectionStringKey = value; }
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            
            if (!String.IsNullOrWhiteSpace(this._storageConnectionStringKey))
            {
                var connectionStringKey = ConfigurationManager.AppSettings[this._storageConnectionStringKey];
                if (!string.IsNullOrEmpty(connectionStringKey))
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                    if (connectionString == null)
                        throw new NullReferenceException(String.Format("connectionString '{0}'", connectionStringKey));
                    return RDBDataProviderFactory.CreateProvider("VR_Invoice", connectionString.ConnectionString);
                }
            }
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        #endregion

        public void SaveInvoiceItems(long invoiceId, IEnumerable<GeneratedInvoiceItemSet> invoiceItemSets)
        {
            if (invoiceItemSets != null && invoiceItemSets.Count() > 0)
            {
                var queryContext = new RDBQueryContext(GetDataProvider());

                foreach (var itemSet in invoiceItemSets)
                {
                    if (itemSet.Items != null && itemSet.Items.Count > 0)
                    {
                        foreach (var item in itemSet.Items)
                        {
                            var insertQuery = queryContext.AddInsertQuery();
                            insertQuery.IntoTable(TABLE_NAME);
                            insertQuery.ColumnValue("InvoiceID", invoiceId);
                            insertQuery.ColumnValue("ItemSetName", itemSet.SetName);
                            insertQuery.ColumnValue("Name", item.Name);
                            string serializedDetails = Vanrise.Common.Serializer.Serialize(item.Details);
                            insertQuery.ColumnValue("Details", serializedDetails);
                        }
                    }
                }

                queryContext.ExecuteNonQuery();
            }
        }
    }
}
