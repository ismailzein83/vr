using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceFieldPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("259F1D17-09A0-4BDA-A83A-BFC5624AD73B"); }
        }
        public InvoiceField InvoiceField { get; set; }
        public string FieldName { get; set; }
        public bool UseDescription { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            Vanrise.Invoice.Entities.Invoice invoice = context.Object as Vanrise.Invoice.Entities.Invoice;
            if (invoice == null)
                throw new NullReferenceException("invoice");
            switch (this.InvoiceField)
            {
                case Entities.InvoiceField.InvoiceId:
                    return invoice.InvoiceId;
                case InvoiceField.Partner:
                    if (this.UseDescription)
                    {
                        PartnerManager partnerManager = new PartnerManager();
                        return partnerManager.GetPartnerName(invoice.InvoiceTypeId, invoice.PartnerId);
                    }
                    return invoice.PartnerId;
                case InvoiceField.DueDate:
                    if (this.UseDescription)
                    {
                        return FormatDateTime(invoice.DueDate, Utilities.GetDateTimeFormat(DateTimeType.Date));
                    }
                    return invoice.DueDate;
                case InvoiceField.CreatedTime:
                    if (this.UseDescription)
                    {
                        return FormatDateTime(invoice.CreatedTime, Utilities.GetDateTimeFormat(DateTimeType.DateTime));
                    }
                    return invoice.CreatedTime;
                case InvoiceField.FromDate:
                    if (this.UseDescription)
                    {
                        return FormatDateTime(invoice.FromDate, Utilities.GetDateTimeFormat(DateTimeType.Date));
                    }
                    return invoice.FromDate;
                case InvoiceField.ToDate:
                    if (this.UseDescription)
                    {
                        return FormatDateTime(invoice.ToDate, Utilities.GetDateTimeFormat(DateTimeType.Date));
                    }
                    return invoice.ToDate;
                case InvoiceField.IssueDate:
                    if (this.UseDescription)
                    {
                        return FormatDateTime(invoice.IssueDate, Utilities.GetDateTimeFormat(DateTimeType.Date));
                    }
                    return invoice.IssueDate;
                case InvoiceField.Note:
                    return invoice.Note;
                case InvoiceField.UserId:
                    if (this.UseDescription)
                    {
                        UserManager userManager = new UserManager();
                        return userManager.GetUserName(invoice.UserId);
                    }
                    return invoice.UserId;
                case InvoiceField.SerialNumber:
                    return invoice.SerialNumber;
                case InvoiceField.IsAutomatic:
                    return invoice.IsAutomatic;
                case InvoiceField.IsSent:
                    return invoice.SentDate.HasValue;
                case InvoiceField.CustomField:
                    DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                    InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                    var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
                    var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(invoiceType.Settings.InvoiceDetailsRecordTypeId);
                    InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(invoice);
                    invoiceRecordObject.ThrowIfNull("invoiceRecordObject");
                    invoiceRecordObject.InvoiceDataRecordObject.ThrowIfNull("invoiceRecordObject.InvoiceDataRecordObject");
                    var fieldValue = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(this.FieldName);
                    if (this.UseDescription)
                    {
                        DataRecordField dataRecordField;
                        if (dataRecordTypeFields.TryGetValue(this.FieldName, out dataRecordField))
                        {
                            return dataRecordField.Type.GetDescription(fieldValue);
                        }
                        throw new NullReferenceException(string.Format("Field: '{0}' not available in dataRecordType '{1}'.", this.FieldName, invoiceType.Settings.InvoiceDetailsRecordTypeId));
                    }
                    return fieldValue;
                case InvoiceField.Paid:
                    if (this.UseDescription && invoice.PaidDate.HasValue)
                    {
                        return FormatDateTime(invoice.PaidDate.Value, Utilities.GetDateTimeFormat(DateTimeType.Date));
                    }
                    return invoice.PaidDate;
                default: return null;
            }
        }
        private string FormatDateTime(DateTime dateTobeFormated, string format)
        {
            return dateTobeFormated.ToString(format);
        }
    }
}
