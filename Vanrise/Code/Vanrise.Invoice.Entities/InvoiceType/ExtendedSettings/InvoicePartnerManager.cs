using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoicePartnerManager
    {
        public virtual string PartnerSelector { get; set; }
        public virtual string PartnerFilterSelector { get; set; }
        public abstract string GetPartnerName(IPartnerNameManagerContext context);
        public abstract dynamic GetPartnerInfo(IPartnerManagerInfoContext context);
        public abstract dynamic GetActualPartnerId(IActualPartnerContext context);
        public abstract int GetPartnerDuePeriod(IPartnerDuePeriodContext context);
        public abstract string GetPartnerSerialNumberPattern(IPartnerSerialNumberPatternContext context);
        public abstract bool CheckInvoiceFollowBillingPeriod(ICheckInvoiceFollowBillingPeriodContext context);
        public virtual InvoicePartnerSettings GetInvoicePartnerSettings(IInvoicePartnerSettingsContext context)
        {
            IInvoiceSettingManager manager = BusinessManagerFactory.GetManager<IInvoiceSettingManager>();
            var invoiceSetting = manager.GetDefaultInvoiceSetting(context.InvoiceTypeId);
            return new InvoicePartnerSettings { InvoiceSetting = invoiceSetting };
        }
    }
    public interface IBasePartnerManagerContext
    {
        string PartnerId { get; }
    }
    public interface IInvoicePartnerSettingsContext : IBasePartnerManagerContext
    {
        Guid InvoiceTypeId { get;}
    }
    public interface IPartnerManagerInfoContext : IBasePartnerManagerContext
    {
        string InfoType { get; }
        InvoicePartnerManager InvoicePartnerManager { get; }
    }
    public interface IPartnerNameManagerContext : IBasePartnerManagerContext
    {
    }
    public interface IPartnerDuePeriodContext : IBasePartnerManagerContext
    {
    }
    public interface IActualPartnerContext : IBasePartnerManagerContext
    {
    }
    public interface IPartnerSerialNumberPatternContext : IBasePartnerManagerContext
    {
    }
    public interface ICheckInvoiceFollowBillingPeriodContext : IBasePartnerManagerContext
    {
    }
}
