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
        public virtual EffectivePartnerInvoiceSetting GetEffectivePartnerInvoiceSetting(IInvoicePartnerSettingsContext context)
        {
            IPartnerInvoiceSettingManager partnerInvoiceSettingManager = BusinessManagerFactory.GetManager<IPartnerInvoiceSettingManager>();
            IInvoiceSettingManager manager = BusinessManagerFactory.GetManager<IInvoiceSettingManager>();
            var partnerInvoiceManager = partnerInvoiceSettingManager.GetPartnerInvoiceSettingByPartnerId(context.PartnerId);
            InvoiceSetting invoiceSetting = null;
            if (partnerInvoiceManager != null)
            {
                invoiceSetting = manager.GetInvoiceSetting(partnerInvoiceManager.InvoiceSettingID);
            }
            else
            {
                invoiceSetting = manager.GetDefaultInvoiceSetting(context.InvoiceTypeId);
            }
            return new EffectivePartnerInvoiceSetting { InvoiceSetting = invoiceSetting };
        }
        public virtual T GetInvoicePartnerSettingPart<T>(IInvoicePartnerSettingPartContext context) where T : InvoiceSettingPart
        {
            InvoicePartnerSettingsContext invoicePartnerSettingsContext = new InvoicePartnerSettingsContext
            {
                InvoiceTypeId = context.InvoiceTypeId,
                PartnerId = context.PartnerId
            };
            var invoiceSetting = GetEffectivePartnerInvoiceSetting(invoicePartnerSettingsContext);
            IInvoiceSettingManager manager = BusinessManagerFactory.GetManager<IInvoiceSettingManager>();
            return manager.GetInvoiceSettingDetailByType<T>(invoiceSetting.InvoiceSetting.InvoiceSettingId);
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
    public class InvoicePartnerSettingsContext:IInvoicePartnerSettingsContext
    {
        public string PartnerId { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }
    public interface IInvoicePartnerSettingPartContext : IBasePartnerManagerContext
    {
        Guid InvoiceTypeId { get; }
        Guid InvoiceSettingId { get; set; }
    }

    public interface IPartnerManagerInfoContext : IBasePartnerManagerContext
    {
        string InfoType { get; }
        InvoicePartnerManager InvoicePartnerManager { get; }
    }
    public interface IPartnerNameManagerContext : IBasePartnerManagerContext
    {
    }
    public interface IActualPartnerContext : IBasePartnerManagerContext
    {
    }

}
