using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
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
                    InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                    var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
                    var partnerDetails = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                    PartnerNameManagerContext partnerNameManagerContext = new PartnerNameManagerContext
                    {
                        PartnerId = invoice.PartnerId
                    };
                    return partnerDetails.GetPartnerName(partnerNameManagerContext);

                case InvoiceField.DueDate:
                    return invoice.DueDate;
                case InvoiceField.CreatedTime:
                    return invoice.CreatedTime;
                case InvoiceField.FromDate:
                    return invoice.FromDate;
                case InvoiceField.ToDate:
                    return invoice.ToDate;
                case InvoiceField.IssueDate:
                    return invoice.IssueDate;
                case InvoiceField.Note:
                    return invoice.Note;
                case InvoiceField.UserId:
                    UserManager userManager = new UserManager();
                    return userManager.GetUserName(invoice.UserId);
                case InvoiceField.SerialNumber:
                    return invoice.SerialNumber;
                case InvoiceField.CustomField:
                    return invoice.Details.GetType().GetProperty(this.FieldName).GetValue(invoice.Details, null);//Utilities.GetPropValue(this.FieldName, invoice.Details);
                //    //Vanrise.Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(invoice.Details);
                case InvoiceField.Paid:
                    return invoice.PaidDate;
                case InvoiceField.TimeZone:
                    if (invoice.TimeZoneId.HasValue)
                    {
                        VRTimeZoneManager timeZoneManager = new VRTimeZoneManager();
                        var timeZone = timeZoneManager.GetVRTimeZone(invoice.TimeZoneId.Value);
                        return timeZone.Name;
                    }
                    else
                        return null;
                case InvoiceField.TimeZoneOffset:
                    return invoice.TimeZoneOffset;
                default: return null;
            }
        }
    }
}
