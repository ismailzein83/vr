using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CompareInvoiceAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "CompareInvoiceAction"; } }
        public override Guid ConfigId { get { return new Guid("CA8088B9-2887-4B85-B263-49C895088E66"); } }

        public override bool DoesUserHaveAccess(IInvoiceActionSettingsCheckAccessContext context)
        {
            return SecurityContext.Current.IsAllowed(context.InvoiceAction.RequiredPermission, context.UserId);
        }
        public InvoiceCarrierType InvoiceCarrierType { get; set; }
        public string PartnerLabel { get; set; }
        public string PartnerAbbreviationLabel { get; set; }
        public CompareInvoiceSMSActionSettings SMSSettings { get; set; }
        public CompareInvoiceVoiceActionSettings VoiceSettings { get; set; }
    }

    public class CompareInvoiceActionSettings
    {
        public Guid ItemGroupingId { get; set; }
    }
    public class CompareInvoiceSMSActionSettings : CompareInvoiceActionSettings
    {
        public Guid MobileNetworkDimensionId { get; set; }
        public Guid CurrencyDimensionId { get; set; }
        public Guid RateDimensionId { get; set; }
        public Guid RateMeasureId { get; set; }
        public Guid NumberOfSMSsMeasureId { get; set; }
        public Guid AmountMeasureId { get; set; }
        public Guid FromDateMeasureId { get; set; }
        public Guid ToDateMeasureId { get; set; }
    }
    public class CompareInvoiceVoiceActionSettings : CompareInvoiceActionSettings
    {
        public Guid ZoneDimensionId { get; set; }
        public Guid CurrencyDimensionId { get; set; }
        public Guid RateDimensionId { get; set; }
        public Guid RateTypeDimensionId { get; set; }
        public Guid RateMeasureId { get; set; }
        public Guid NumberOfCallsMeasureId { get; set; }
        public Guid AmountMeasureId { get; set; }
        public Guid DurationMeasureId { get; set; }
        public Guid FromDateMeasureId { get; set; }
        public Guid ToDateMeasureId { get; set; }
    }
}