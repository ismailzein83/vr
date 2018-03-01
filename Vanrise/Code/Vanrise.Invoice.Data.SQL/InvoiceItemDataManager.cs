using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceItemDataManager:BaseSQLDataManager,IInvoiceItemDataManager
    {
       
        #region ctor
        const string LiveBalance_TABLENAME = "InvoiceItemTable";
        public InvoiceItemDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }

        string _storageConnectionStringKey;
        public string StorageConnectionStringKey
        {
            set { this._storageConnectionStringKey = value; }
        }
        #endregion
        public IEnumerable<InvoiceItem> GetFilteredInvoiceItems(long invoiceId, string itemSetName, CompareOperator compareOperator)
        {
            switch (compareOperator)
            {
                case CompareOperator.Contains:
                    itemSetName = string.Format("%{0}%", itemSetName);
                    break;
                case CompareOperator.EndWith:
                    itemSetName = string.Format("%{0}", itemSetName);
                    break;
                case CompareOperator.Equal:
                    break;
                case CompareOperator.StartWith:
                    itemSetName = string.Format("{0}%", itemSetName);
                    break;
            }
            return GetItemsSP("VR_Invoice.sp_InvoiceItem_GetFiltered", InvoiceMapper, invoiceId, itemSetName);
        }
        public void SaveInvoiceItems(long invoiceId, IEnumerable<GeneratedInvoiceItemSet> invoiceItemSets)
        {

            DataTable invoiceItemToSave = GetInvoiceItemTable();
            foreach(var invoiceItemSet in invoiceItemSets)
            {
                foreach (var invoiceItem in invoiceItemSet.Items)
                {
                    DataRow dr = invoiceItemToSave.NewRow();
                    FillInvoiceItemRow(dr, invoiceItem, invoiceItemSet.SetName, invoiceId);
                    invoiceItemToSave.Rows.Add(dr);
                }
            }
            invoiceItemToSave.EndLoadData();
            if (invoiceItemToSave.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_Invoice].[sp_InvoiceItem_Save]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@InvoiceItemTable", SqlDbType.Structured);
                           dtPrm.Value = invoiceItemToSave;
                           cmd.Parameters.Add(dtPrm);
                       });

        }

        public IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(List<long> invoiceIds, IEnumerable<string> itemSetNames, CompareOperator compareOperator)
        {
            switch (compareOperator)
            {
                case CompareOperator.Contains:
                    itemSetNames = itemSetNames.Select(x => string.Format("%{0}%", x));
                    break;
                case CompareOperator.EndWith:
                    itemSetNames = itemSetNames.Select(x => string.Format("%{0}", x));
                    break;
                case CompareOperator.Equal:
                    break;
                case CompareOperator.StartWith:
                    itemSetNames = itemSetNames.Select(x => string.Format("{0}%", x));
                    break;
            }
            string itemSetNamesString = null;
            if (itemSetNames != null && itemSetNames.Count() > 0)
            {
                itemSetNamesString = string.Join(",", itemSetNames);
            }

            string invoiceIdsString = null;
            if (invoiceIds != null && invoiceIds.Count() > 0)
            {
                invoiceIdsString = string.Join<long>(",", invoiceIds);
            }

            return GetItemsSP("VR_Invoice.sp_InvoiceItem_GetByItemSetNames", InvoiceMapper, invoiceIdsString, itemSetNamesString);
        }

        #region Private Methods
        private void FillInvoiceItemRow(DataRow dr, GeneratedInvoiceItem item, string setName, long invoiceId)
        {
            dr["InvoiceID"] = invoiceId;
            dr["ItemSetName"] = setName;
            dr["Name"] = item.Name;
            dr["Details"] = Vanrise.Common.Serializer.Serialize(item.Details);
        }
        private DataTable GetInvoiceItemTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("InvoiceID", typeof(long));
            dt.Columns.Add("ItemSetName", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Details", typeof(string));
            return dt;
        }
        #endregion
      
        #region Mappers
        public Entities.InvoiceItem InvoiceMapper(IDataReader reader)
        {
            Entities.InvoiceItem invoice = new Entities.InvoiceItem
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader["Details"] as string),
                InvoiceId = GetReaderValue<long>(reader, "InvoiceId"),
                ItemSetName = reader["ItemSetName"] as string,
                Name = reader["Name"] as string,
                InvoiceItemId = GetReaderValue<long>(reader, "ID"),
            };
            return invoice;
        }
        #endregion

        protected override string GetConnectionString()
        {
            if (!String.IsNullOrWhiteSpace(this._storageConnectionStringKey))
            {
                var connectionStringKey = ConfigurationManager.AppSettings[this._storageConnectionStringKey];
                if (!string.IsNullOrEmpty(connectionStringKey))
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                    if (connectionString == null)
                        throw new NullReferenceException(String.Format("connectionString '{0}'", connectionStringKey));
                    return connectionString.ConnectionString;
                }
            }
            return base.GetConnectionString();
        }
    }
}
