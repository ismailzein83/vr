using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceManager
    {
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoiceDetail(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);

            return new ComparisonInvoiceDetail()
            {
                To = invoice.PartnerName,
                ToDate = invoice.Entity.ToDate,
                DueDate = invoice.Entity.DueDate,
                Calls = invoice.Entity.Details.TotalNumberOfCalls,
                FromDate = invoice.Entity.FromDate,
                IssuedDate = invoice.Entity.IssueDate,
                SerialNumber = invoice.Entity.SerialNumber,
                Duration = invoice.Entity.Details.Duration,
                TotalAmount = invoice.Entity.Details.TotalInvoiceAmount,
                TotalNumberOfSMS = invoice.Entity.Details.TotalNumberOfSMS,
                IsLocked = invoice.Lock,
                IsPaid = invoice.Paid,
                IssuedBy = invoice.UserName,
                PartnerId = invoice.Entity.PartnerId,
                Currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.InterconnectCurrencyId)
            };
        }

        public bool UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(input.InvoiceId);
            invoice.ThrowIfNull("invoice", input.InvoiceId);

            if (input.AttachementFiles != null)
            {
                VRFileManager vrFileManager = new VRFileManager();
                foreach (var attachment in input.AttachementFiles)
                {
                    if (!vrFileManager.SetFileUsedAndUpdateSettings(attachment.FileId, new VRFileSettings { ExtendedSettings = new OriginalInvoiceDataFileSetting { InvoiceId = input.InvoiceId, InvoiceTypeId = invoice.InvoiceTypeId } }))
                    {
                        return false;
                    }
                }
            }

            var interconnectInvoiceDetails = invoice.Details as InterconnectInvoiceDetails;
            interconnectInvoiceDetails.ThrowIfNull("interconnectInvoiceDetails");
            interconnectInvoiceDetails.AttachementFiles = input.AttachementFiles;
            interconnectInvoiceDetails.Reference = input.Reference;
            interconnectInvoiceDetails.OriginalAmountByCurrency = input.OriginalDataCurrency;

            if (input.OriginalDataCurrency != null && input.OriginalDataCurrency.Count > 0 && input.OriginalDataCurrency.All(x => x.Value.OriginalAmount > 0))
                interconnectInvoiceDetails.IsOriginalAmountSetted = true;
            else
                interconnectInvoiceDetails.IsOriginalAmountSetted = false;

            invoice.Details = interconnectInvoiceDetails;

            if (invoiceManager.TryUpdateInvoice(invoice))
            {
                VRActionLogger.Current.LogObjectCustomAction(new Vanrise.Invoice.Business.InvoiceManager.InvoiceLoggableEntity(invoice.InvoiceTypeId), "Update", true, invoice, "Invoice original data updated");
                return true;
            }
            return false;
        }

        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);


            string reference = null;
            Dictionary<int, OriginalDataCurrrency> originalAmountByCurrency = null;
            List<AttachementFile> attachementFiles = null;

            var interconnnectInvoiceDetails = invoice.Details as InterconnectInvoiceDetails;
                    interconnnectInvoiceDetails.ThrowIfNull("customerInvoiceDetails");
                    reference = interconnnectInvoiceDetails.Reference;
                    originalAmountByCurrency = interconnnectInvoiceDetails.OriginalAmountByCurrency;
                    attachementFiles = interconnnectInvoiceDetails.AttachementFiles;

            var originalInvoiceDataRuntime = new OriginalInvoiceDataRuntime
            {
                Reference = reference,
            };
            CurrencyManager currencyManager = new CurrencyManager();

            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var currencyItemSetName = invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceId, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
            if (currencyItemSetName != null)
            {
                originalInvoiceDataRuntime.OriginalDataCurrency = new Dictionary<int, OriginalDataCurrrency>();
                foreach (var item in currencyItemSetName)
                {
                    var itemDetails = item.Details as InterconnectInvoiceByCurrencyItemDetails;
                    if (itemDetails != null)
                    {
                        var currencySymbol = currencyManager.GetCurrencySymbol(itemDetails.CurrencyId);
                        currencySymbol.ThrowIfNull("currencySymbol", itemDetails.CurrencyId);
                        var originalDataCurrrency = originalInvoiceDataRuntime.OriginalDataCurrency.GetOrCreateItem(itemDetails.CurrencyId);
                        originalDataCurrrency.CurrencySymbol = currencySymbol;
                    }
                }
            }


            if (originalAmountByCurrency != null)
            {
                foreach (var originalAmount in originalAmountByCurrency)
                {
                    var record = originalInvoiceDataRuntime.OriginalDataCurrency.GetRecord(originalAmount.Key);
                    if (record != null)
                    {
                        record.OriginalAmount = originalAmount.Value.OriginalAmount;
                        record.IncludeOriginalAmountInSettlement = originalAmount.Value.IncludeOriginalAmountInSettlement;
                    }
                }
            }

            if (attachementFiles != null && attachementFiles.Count > 0)
            {
                originalInvoiceDataRuntime.AttachementFilesRuntime = new List<AttachementFileRuntime>();
                var fileIds = attachementFiles.Select(x => x.FileId);
                VRFileManager vrFileManager = new VRFileManager();
                var files = vrFileManager.GetFilesInfo(fileIds);
                foreach (var attachementFile in attachementFiles)
                {
                    if (files != null)
                    {
                        var file = files.FindRecord(x => x.FileId == attachementFile.FileId);
                        if (file != null)
                        {
                            originalInvoiceDataRuntime.AttachementFilesRuntime.Add(new AttachementFileRuntime
                            {
                                FileId = file.FileId,
                                FileName = file.Name
                            });
                        }
                    }
                }
            }
            return originalInvoiceDataRuntime;
        }


        public bool DoesUserHaveUpdateOriginalInvoiceDataAccess(long invoiceId)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().DoesUserHaveGenerateAccess(invoiceId);

        }

        public bool DoesInvoiceReportExist(bool isCustomer)
        {
            string physicalPath = "";
            if (isCustomer)
                physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/Retail_Interconnect/Reports/CustomerCompareInvoiceReport.rdlc");
            else
                physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/Retail_Interconnect/Reports/SupplierCompareInvoiceReport.rdlc");
            return Utilities.PhysicalPathExists(physicalPath);
        }
    }
}
