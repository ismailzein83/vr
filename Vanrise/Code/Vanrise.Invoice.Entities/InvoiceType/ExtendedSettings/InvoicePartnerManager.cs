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
        public virtual string PartnerInvoiceSettingFilterFQTN { get; set; }
        public abstract string GetPartnerName(IPartnerNameManagerContext context);
        public virtual string GetFullPartnerName(IPartnerNameManagerContext context)
        {
            return GetPartnerName(context);
        }
        public abstract dynamic GetPartnerInfo(IPartnerManagerInfoContext context);
        public abstract dynamic GetActualPartnerId(IActualPartnerContext context);
        public virtual EffectivePartnerInvoiceSetting GetEffectivePartnerInvoiceSetting(IInvoicePartnerSettingsContext context)
        {
            IPartnerInvoiceSettingManager partnerInvoiceSettingManager = BusinessManagerFactory.GetManager<IPartnerInvoiceSettingManager>();
            IInvoiceSettingManager manager = BusinessManagerFactory.GetManager<IInvoiceSettingManager>();
            var partnerInvoiceSetting = partnerInvoiceSettingManager.GetPartnerInvoiceSettingByPartnerId(context.PartnerId,context.InvoiceTypeId);
            InvoiceSetting invoiceSetting = null;
            if (partnerInvoiceSetting != null)
            {
                invoiceSetting = manager.GetInvoiceSetting(partnerInvoiceSetting.InvoiceSettingID);
            }
            else
            {
                invoiceSetting = manager.GetDefaultInvoiceSetting(context.InvoiceTypeId);
            }
            return new EffectivePartnerInvoiceSetting { InvoiceSetting = invoiceSetting, PartnerInvoiceSetting = partnerInvoiceSetting };
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
            IPartnerInvoiceSettingManager partnerInvoiceSettingManager = BusinessManagerFactory.GetManager<IPartnerInvoiceSettingManager>();
            if(invoiceSetting.PartnerInvoiceSetting !=null)
            {
                var invoicePartnerSettingPart = partnerInvoiceSettingManager.GetPartnerInvoiceSettingDetailByType<T>(invoiceSetting.PartnerInvoiceSetting.PartnerInvoiceSettingId);
                if (invoicePartnerSettingPart != null)
                    return invoicePartnerSettingPart;
            }
         
            return manager.GetInvoiceSettingDetailByType<T>(invoiceSetting.InvoiceSetting.InvoiceSettingId);
        }
        public abstract VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context);

    }
    public interface IBasePartnerManagerContext
    {
        string PartnerId { get; }
    }
    public interface IInvoiceAccountDataContext : IBasePartnerManagerContext
    {
        Guid InvoiceTypeId { get; }
    }
    public interface IInvoicePartnerSettingsContext : IBasePartnerManagerContext
    {
        Guid InvoiceTypeId { get;}
    }
    public interface IPartnerTimeZoneContext:IBasePartnerManagerContext
    {

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
