using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceItemDataManager:BaseSQLDataManager
    {
       
        #region ctor
        const string LiveBalance_TABLENAME = "InvoiceItemTable";
        public InvoiceItemDataManager()
            : base(GetConnectionStringName("InvoiceTypeDBConnStringKey", "InvoiceTypeDBConnString"))
        {

        }
        #endregion
        public void SaveInvoiceItems(long invoiceId, List<GeneratedInvoiceItemSet> invoiceItemSets)
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

        #region Private Methods
        private void FillInvoiceItemRow(DataRow dr, GeneratedInvoiceItem item, string setName, long invoiceId)
        {
            dr["InvoiceID"] = invoiceId;
            dr["ItemSetName"] = setName;
            dr["Details"] = Vanrise.Common.Serializer.Serialize(item);
        }
        private DataTable GetInvoiceItemTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("InvoiceID", typeof(long));
            dt.Columns.Add("ItemSetName", typeof(string));
            dt.Columns.Add("Details", typeof(string));
            return dt;
        }
        #endregion
    }
}
