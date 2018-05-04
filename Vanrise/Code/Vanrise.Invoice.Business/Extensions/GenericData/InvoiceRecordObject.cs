using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Invoice.Business
{
    public class InvoiceRecordObject
    {
        static InvoiceTypeManager s_invoiceTypeManager = new InvoiceTypeManager();
        static Vanrise.GenericData.Business.DataRecordTypeManager s_dataRecordTypeManager = new GenericData.Business.DataRecordTypeManager();

        public InvoiceRecordObject(Entities.Invoice invoice)
        {
            BuildDataRecordObject(invoice);
        }

        private void BuildDataRecordObject(Entities.Invoice invoice)
        {
            this.Invoice = invoice;
            invoice.ThrowIfNull("invoice");
            ExtensionMethods.ThrowIfNull(invoice.Details, "invoice.Details", invoice.InvoiceId);
            var invoiceType = s_invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceType.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", invoiceType.InvoiceTypeId);
            var invoiceDataRecordType = s_dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            invoiceDataRecordType.ThrowIfNull("invoiceDataRecordType", invoiceType.Settings.InvoiceDetailsRecordTypeId);
            Dictionary<string, Object> invoiceFieldValues = null;           
            if (invoiceDataRecordType.ExtraFieldsEvaluator != null && invoiceDataRecordType.ExtraFieldsEvaluator is InvoiceRecordTypeMainFields)
            {
                invoiceFieldValues = new Dictionary<string, object>();
                invoiceFieldValues.Add("ID", invoice.InvoiceId);
                invoiceFieldValues.Add("Partner", invoice.PartnerId);
                invoiceFieldValues.Add("SerialNumber", invoice.SerialNumber);
                invoiceFieldValues.Add("IssueDate", invoice.IssueDate);
                invoiceFieldValues.Add("DueDate", invoice.DueDate);
                invoiceFieldValues.Add("FromDate", invoice.FromDate);
                invoiceFieldValues.Add("ToDate", invoice.ToDate);
                invoiceFieldValues.Add("PaidDate", invoice.PaidDate);
                invoiceFieldValues.Add("CreatedTime", invoice.CreatedTime);
                invoiceFieldValues.Add("ApprovedBy", invoice.ApprovedBy);
                invoiceFieldValues.Add("ApprovedTime", invoice.ApprovedTime);
                invoiceFieldValues.Add("IsAutomatic", invoice.IsAutomatic);
                invoiceFieldValues.Add("NeedApproval", invoice.NeedApproval);
                invoiceFieldValues.Add("Notes", invoice.Note);
                invoiceFieldValues.Add("SentDate", invoice.SentDate);
                invoiceFieldValues.Add("User", invoice.UserId);
            }

            var _recordRuntimeType = s_dataRecordTypeManager.GetDataRecordRuntimeType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            _recordRuntimeType.ThrowIfNull("recordRuntimeType", invoiceType.Settings.InvoiceDetailsRecordTypeId);
            dynamic invoiceDataRecord = Activator.CreateInstance(_recordRuntimeType, invoiceFieldValues, invoice.Details);

            InvoiceDataRecordObject = new GenericData.Business.DataRecordObject(invoiceType.Settings.InvoiceDetailsRecordTypeId, invoiceDataRecord);
        }

        public Entities.Invoice Invoice { get; private set; }

        public Vanrise.GenericData.Business.DataRecordObject InvoiceDataRecordObject
        {
            get;
            private set;
        }
    }
}
