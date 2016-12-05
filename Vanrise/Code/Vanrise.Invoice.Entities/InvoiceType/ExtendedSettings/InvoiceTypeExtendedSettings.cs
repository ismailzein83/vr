using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string GenerationCustomSectionDirective { get; set; }
        public abstract InvoiceGenerator GetInvoiceGenerator();
        public abstract InvoicePartnerSettings GetPartnerSettings();
        public abstract dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context);
        public abstract void GetInitialPeriodInfo(IInitialPeriodInfoContext context);
        public abstract BillingPeriod GetBillingPeriod(IExtendedSettingsBillingPeriodContext context);
    }
   
    public abstract class InvoiceGenerator
    {
        public abstract void GenerateInvoice(IInvoiceGenerationContext context);
    }
    public interface IInvoiceGenerationContext
    {
        Guid InvoiceTypeId { get; }
        string PartnerId { get; }

        DateTime FromDate { get; }

        DateTime ToDate { get; }

        dynamic CustomSectionPayload { get; }

        GeneratedInvoice Invoice { set; }
    }
    public interface IInvoiceTypeExtendedSettingsInfoContext
    {
        string InfoType { get; set; }
        Entities.Invoice Invoice { get; set; }
    }

    public interface IInitialPeriodInfoContext
    {
        string PartnerId { get; }
        DateTime InitialStartDate { set; }
        DateTime PartnerCreationDate { set; }
    }
    public interface IExtendedSettingsBillingPeriodContext
    {
        string PartnerId { get; }
    }
}
