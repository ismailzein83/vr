using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace ResolveInvoiceIssue
{
    public class Invoice
    {
        public long InvoiceId { get; set; }
        public dynamic Details { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {

              new TestMethod().Method();
        }
        public class Class2
        {
            public int ID { get; set; }
            public int? OrderBy { get; set; }
            public string Name { get; set; }
        }



        public class TestMethod: BaseSQLDataManager
        {
            public TestMethod()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
            { }

            public void Method()
            {
                BackUpInvoiceTable();

                var customerInvoiceTypeId = new Guid("EADC10C8-FFD7-4EE3-9501-0B2CE09029AD");
                var supplierInvoiceTypeId = new Guid("5FD8DF00-71E8-4EDB-BBEC-108EE3CD66B3");
                var invoices = GetItemsText($"Select ID,Details,InvoiceTypeId From  VR_Invoice.Invoice where Details not like '%TotalInvoiceAmount%' and invoiceTypeId in ('{supplierInvoiceTypeId}','{customerInvoiceTypeId}')", InvoiceMapper, (cmd) => { });
                if (invoices != null)
                {
                    foreach (var invoice in invoices)
                    {
                        if (invoice.InvoiceTypeId == customerInvoiceTypeId)
                        {
                            var customerInvoiceDetail = Serializer.Deserialize<CustomerInvoiceDetails>(invoice.Details);
                            customerInvoiceDetail.TotalInvoiceAmount = customerInvoiceDetail.TotalAmountAfterCommission;
                            ExecuteNonQueryText($"Update  VR_Invoice.Invoice set Details = '{Serializer.Serialize(customerInvoiceDetail)}' where ID = {invoice.InvoiceId}", null);
                        }
                        else if (invoice.InvoiceTypeId == supplierInvoiceTypeId)
                        {
                            var supplierInvoiceDetail = Serializer.Deserialize<SupplierInvoiceDetails>(invoice.Details);
                            supplierInvoiceDetail.TotalInvoiceAmount = supplierInvoiceDetail.TotalAmountAfterCommission;
                            ExecuteNonQueryText($"Update  VR_Invoice.Invoice set Details = '{Serializer.Serialize(supplierInvoiceDetail)}' where ID = {invoice.InvoiceId}", null);
                        }
                    }
                }
            }

            private void BackUpInvoiceTable()
            {
                ExecuteNonQueryText($"Select * into VR_Invoice.Invoice_{DateTime.Now:yyyy_MM_dd_hh_mm_ss} from VR_Invoice.Invoice",null);
            }

            Invoice InvoiceMapper(IDataReader reader)
            {
                return new Invoice
                {
                    Details = GetReaderValue<dynamic>(reader, "Details"),
                    InvoiceId = GetReaderValue<long>(reader, "ID"),
                    InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeID")
                };
            }
        }
     
    }
}
