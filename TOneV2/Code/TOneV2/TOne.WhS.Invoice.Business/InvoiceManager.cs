using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Business.Extensions;
using TOne.WhS.Invoice.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Common.Business;
using System.Globalization;
namespace TOne.WhS.Invoice.Business
{
    public class InvoiceManager
    {

        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoiceDetail(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);
            string timezone = "";
            string currency = "";
            switch (invoiceCarrierType)
            {
                case InvoiceCarrierType.Customer:
                    var customerInvoiceDetails = (CustomerInvoiceDetails)invoice.Entity.Details;
                    customerInvoiceDetails.ThrowIfNull("customerInvoiceDetails");
                    currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.SaleCurrencyId);
                    if (customerInvoiceDetails.TimeZoneId != null)
                    {
                        Vanrise.Entities.VRTimeZone timeZone = new Vanrise.Common.Business.VRTimeZoneManager().GetVRTimeZone((int)customerInvoiceDetails.TimeZoneId);
                        timezone = timeZone.Name;
                    }
                    break;
                case InvoiceCarrierType.Supplier:
                    var supplierInvoiceDetails = (SupplierInvoiceDetails)invoice.Entity.Details;
                    supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                    currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.SupplierCurrencyId);
                    if (supplierInvoiceDetails.TimeZoneId != null)
                    {
                        Vanrise.Entities.VRTimeZone timeZone = new Vanrise.Common.Business.VRTimeZoneManager().GetVRTimeZone((int)supplierInvoiceDetails.TimeZoneId);
                        timezone = timeZone.Name;
                    }
                    break;

            }


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
                Currency = currency,
                TimeZone = timezone
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
            InvoiceCarrierType invoiceCarrierType = input.invoiceCarrierType;
            switch (invoiceCarrierType)
            {
                case InvoiceCarrierType.Customer:
                    var customerInvoiceDetails = invoice.Details as CustomerInvoiceDetails;
                    customerInvoiceDetails.ThrowIfNull("customerInvoiceDetails");
                    customerInvoiceDetails.AttachementFiles = input.AttachementFiles;
                    customerInvoiceDetails.Reference = input.Reference;
                    customerInvoiceDetails.OriginalAmountByCurrency = input.OriginalDataCurrency;

                    invoice.Details = customerInvoiceDetails;
                    break;
                case InvoiceCarrierType.Supplier:
                    var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
                    supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                    supplierInvoiceDetails.AttachementFiles = input.AttachementFiles;
                    supplierInvoiceDetails.Reference = input.Reference;
                    supplierInvoiceDetails.OriginalAmountByCurrency = input.OriginalDataCurrency;

                    invoice.Details = supplierInvoiceDetails;
                    break;

            }



            if (invoiceManager.TryUpdateInvoice(invoice))
            {
                VRActionLogger.Current.LogObjectCustomAction(new Vanrise.Invoice.Business.InvoiceManager.InvoiceLoggableEntity(invoice.InvoiceTypeId), "Update", true, invoice, "Invoice original data updated");
                return true;
            }
            return false;

        }


        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);


            string reference = null;
            Dictionary<int, OriginalDataCurrrency> originalAmountByCurrency = null;
            List<AttachementFile> attachementFiles = null;
            switch (invoiceCarrierType)
            {
                case InvoiceCarrierType.Customer:
                    var customerInvoiceDetails = invoice.Details as CustomerInvoiceDetails;
                    customerInvoiceDetails.ThrowIfNull("customerInvoiceDetails");
                    reference = customerInvoiceDetails.Reference;
                    originalAmountByCurrency = customerInvoiceDetails.OriginalAmountByCurrency;
                    attachementFiles = customerInvoiceDetails.AttachementFiles;

                    break;

                case InvoiceCarrierType.Supplier:
                    var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
                    supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                    reference = supplierInvoiceDetails.Reference;
                    originalAmountByCurrency = supplierInvoiceDetails.OriginalAmountByCurrency;
                    attachementFiles = supplierInvoiceDetails.AttachementFiles;
                    break;
            }


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
                    var itemDetails = item.Details as InvoiceBySaleCurrencyItemDetails;
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

        public string GetRDLCReportPath(int? financialAccountCarrierProfileId, Guid invoiceTypeId, int? financialAccountCarrierAccountId)
        {
            Dictionary<Guid, InvoiceReportFile> invoiceReportFiles;
            InvoiceReportFileManager invoiceReportFileManager = new InvoiceReportFileManager();
            if (financialAccountCarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                invoiceReportFiles = carrierProfileManager.GetCompanySettingInvoiceReportFiles(financialAccountCarrierProfileId.Value);
                if (invoiceReportFiles != null && invoiceReportFiles.Count > 0)
                {
                    var invoiceReportFile = invoiceReportFiles.GetRecord(invoiceTypeId);
                    if (invoiceReportFile != null)
                    {
                        var reportName = invoiceReportFileManager.GetInvoiceReportFileName(invoiceReportFile.InvoiceReportFileId);
                        return string.Format("Module/WhS_Invoice/Reports/{0}.rdlc", reportName);
                    }
                }
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                invoiceReportFiles = carrierAccountManager.GetCompanySettingInvoiceReportFiles(financialAccountCarrierAccountId.Value);
                if (invoiceReportFiles != null && invoiceReportFiles.Count > 0)
                {
                    var invoiceReportFile = invoiceReportFiles.GetRecord(invoiceTypeId);
                    if (invoiceReportFile != null)
                    {
                        var reportName = invoiceReportFileManager.GetInvoiceReportFileName(invoiceReportFile.InvoiceReportFileId);
                        return string.Format("Module/WhS_Invoice/Reports/{0}.rdlc", reportName);
                    }
                }
            }
            return null;
        }

        public bool DoesUserHaveUpdateOriginalInvoiceDataAccess(long invoiceId)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().DoesUserHaveGenerateAccess(invoiceId);

        }

        //public bool DoesInvoiceReportExist(bool isCustomer) {
        //	string physicalPath = "";
        //  if(isCustomer)
        //		physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/WhS_Invoice/Reports/CustomerCompareInvoiceReport.rdlc");
        //  else
        //		physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/WhS_Invoice/Reports/SupplierCompareInvoiceReport.rdlc");
        //	return Utilities.PhysicalPathExists(physicalPath);
        //}


    }
}
