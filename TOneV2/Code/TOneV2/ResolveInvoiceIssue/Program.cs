using System;
using System.Data;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace ResolveInvoiceIssue
{
    #region Public Classes

    public class Invoice
    {
        public long InvoiceId { get; set; }
        public dynamic Details { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }

    public class InvoiceItemDetail
    {
        public long InvoiceItemId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public dynamic Details { get; set; }
    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {

            new TestMethod().Method();
        }

        public class TestMethod : BaseSQLDataManager
        {
            public TestMethod()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
            { }

            public void Method()
            {
                try
                {
                    BackUpInvoiceTables();
                    var customerInvoiceTypeId = new Guid("EADC10C8-FFD7-4EE3-9501-0B2CE09029AD");
                    var supplierInvoiceTypeId = new Guid("5FD8DF00-71E8-4EDB-BBEC-108EE3CD66B3");
                    var invoices = GetItemsText($"Select ID,Details,InvoiceTypeId From  VR_Invoice.Invoice where Details not like '%TotalInvoiceAmount\"%' and invoiceTypeId in ('{supplierInvoiceTypeId}','{customerInvoiceTypeId}')", InvoiceMapper, (cmd) => { });

                    if (invoices != null)
                    {
                        int updatedSupplierInvoicesCount = 0;
                        int updatedcustomerInvoicesCount = 0;

                        Console.WriteLine("{0} invoices to be updated.", invoices.Count);

                        foreach (var invoice in invoices)
                        {
                            if (invoice.InvoiceTypeId == customerInvoiceTypeId)
                            {
                                var customerInvoiceDetail = Serializer.Deserialize<CustomerInvoiceDetails>(invoice.Details);
                                customerInvoiceDetail.TotalInvoiceAmount = customerInvoiceDetail.TotalAmountAfterCommission;
                                if (ExecuteNonQueryText($"Update  VR_Invoice.Invoice set Details = '{Serializer.Serialize(customerInvoiceDetail)}' where ID = {invoice.InvoiceId}", null) > 0)
                                {
                                    updatedcustomerInvoicesCount++;
                                    Console.WriteLine("InvoiceId {0} updated.", invoice.InvoiceId);
                                }
                                else
                                {
                                    Console.WriteLine("InvoiceId {0} failed to updated.", invoice.InvoiceId);
                                }

                            }
                            else if (invoice.InvoiceTypeId == supplierInvoiceTypeId)
                            {
                                var supplierInvoiceDetail = Serializer.Deserialize<SupplierInvoiceDetails>(invoice.Details);
                                supplierInvoiceDetail.TotalInvoiceAmount = supplierInvoiceDetail.TotalAmountAfterCommission;
                                if (ExecuteNonQueryText($"Update  VR_Invoice.Invoice set Details = '{Serializer.Serialize(supplierInvoiceDetail)}' where ID = {invoice.InvoiceId}", null) > 0)
                                {
                                    updatedSupplierInvoicesCount++;
                                    Console.WriteLine("InvoiceId {0} updated.", invoice.InvoiceId);
                                }
                                else
                                {
                                    Console.WriteLine("InvoiceId {0} failed to updated.", invoice.InvoiceId);
                                }
                            }
                        }

                        if (updatedSupplierInvoicesCount > 0)
                            Console.WriteLine($"{updatedSupplierInvoicesCount} supplier invoices are updated.");

                        if (updatedcustomerInvoicesCount > 0)
                            Console.WriteLine($"{updatedcustomerInvoicesCount} customer invoices are updated.");

                    }
                    else
                    {
                        Console.WriteLine("All invoices are already updated.");
                    }

                    var invoiceItems = GetItemsText($"SELECT it.[ID] ID" +
                        $", InvoiceTypeID " +
                        $", it.[Details] Details " +
                        $"FROM VR_Invoice.InvoiceItem as it join VR_Invoice.Invoice as inv on it.InvoiceID = inv.ID where ItemSetName = 'GroupingByCurrency' AND it.Details not like '%TotalFullAmount\"%' and invoiceTypeId in ('{supplierInvoiceTypeId}','{customerInvoiceTypeId}')", InvoiceItemDetailMapper, (cmd) => { });

                    if (invoiceItems != null)
                    {
                        int supplierInvoiceItems = 0;
                        int customerInvoiceItems = 0;

                        Console.WriteLine("{0} invoice items to be updated.", invoiceItems.Count);

                        foreach (var invoiceItem in invoiceItems)
                        {
                            if (invoiceItem.InvoiceTypeId == customerInvoiceTypeId)
                            {
                                var customerInvoiceDetail = Serializer.Deserialize<CustomerInvoiceBySaleCurrencyItemDetails>(invoiceItem.Details);
                                customerInvoiceDetail.TotalFullAmount = customerInvoiceDetail.AmountAfterCommissionWithTaxes;
                                if (ExecuteNonQueryText($"Update  VR_Invoice.InvoiceItem set Details = '{Serializer.Serialize(customerInvoiceDetail)}' where ID = {invoiceItem.InvoiceItemId}", null) > 0)
                                {
                                    customerInvoiceItems++;
                                    Console.WriteLine("InvoiceItemId {0} updated.", invoiceItem.InvoiceItemId);
                                }
                                else
                                {
                                    Console.WriteLine("InvoiceItemId {0} failed to updated.", invoiceItem.InvoiceItemId);
                                }
                            }
                            else if (invoiceItem.InvoiceTypeId == supplierInvoiceTypeId)
                            {
                                var supplierInvoiceDetail = Serializer.Deserialize<SupplierInvoiceBySaleCurrencyItemDetails>(invoiceItem.Details);
                                supplierInvoiceDetail.TotalFullAmount = supplierInvoiceDetail.AmountAfterCommissionWithTaxes;
                                if (ExecuteNonQueryText($"Update  VR_Invoice.InvoiceItem set Details = '{Serializer.Serialize(supplierInvoiceDetail)}' where ID = {invoiceItem.InvoiceItemId}", null) > 0)
                                {
                                    supplierInvoiceItems++;
                                    Console.WriteLine("InvoiceItemId {0} updated.", invoiceItem.InvoiceItemId);
                                }
                                else
                                {
                                    Console.WriteLine("InvoiceItemId {0} failed to updated.", invoiceItem.InvoiceItemId);
                                }
                            }
                        }

                        if (supplierInvoiceItems > 0)
                            Console.WriteLine($"{supplierInvoiceItems} supplier invoices are updated.");

                        if (customerInvoiceItems > 0)
                            Console.WriteLine($"{customerInvoiceItems} customer invoices are updated.");
                    }
                    else
                    {
                        Console.WriteLine("All invoice items are already updated.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.ReadLine();
            }

            #region Private Methods

            private void BackUpInvoiceTables()
            {
                DateTime now = DateTime.Now;
                ExecuteNonQueryText($"Select * into VR_Invoice.Invoice_{now:yyyy_MM_dd_hh_mm_ss} from VR_Invoice.Invoice " +
                    $"Select * into VR_Invoice.InvoiceItem_{now:yyyy_MM_dd_hh_mm_ss} from VR_Invoice.InvoiceItem", null);
            }

            #endregion

            #region Mappers

            Invoice InvoiceMapper(IDataReader reader)
            {
                return new Invoice
                {
                    Details = GetReaderValue<dynamic>(reader, "Details"),
                    InvoiceId = GetReaderValue<long>(reader, "ID"),
                    InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeID")
                };
            }

            InvoiceItemDetail InvoiceItemDetailMapper(IDataReader reader)
            {
                return new InvoiceItemDetail
                {
                    Details = GetReaderValue<dynamic>(reader, "Details"),
                    InvoiceItemId = GetReaderValue<long>(reader, "ID"),
                    InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeID")
                };
            }

            #endregion
        }

    }
}
