using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceViewerTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("3A02EEEA-6F38-4277-BAC4-9D8F88F71851"); }
        }
        public Guid VRConnectionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public InvoiceViewerTypeGridSettings GridSettings { get; set; }
        public InvoiceViewerTypeExtendedSettings ExtendedSettings { get; set; }
        public InvoiceQueryInterceptor InvoiceQueryInterceptor { get; set; }

    }
    public class InvoiceViewerTypeGridSettings
    {
        public List<InvoiceViewerTypeGridField> InvoiceGridFields { get; set; }
        public List<InvoiceViewerTypeGridAction> InvoiceGridActions { get; set; }

    }
    public class InvoiceViewerTypeGridAction
    {
        public string Title { get; set; }
        public Guid InvoiceViewerTypeGridActionId { get; set; }
        public InvoiceViewerTypeGridActionSettings Settings { get; set; }
    }
    public abstract class InvoiceViewerTypeGridActionSettings
    {
        public virtual string ActionTypeName { get; set; }
        public abstract Guid ConfigId { get; }
    }
    public abstract class InvoiceViewerTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract IEnumerable<PortalInvoiceAccount> GetInvoiceAccounts(IInvoiceViewerTypeExtendedSettingsContext context);
    }
    public interface IInvoiceViewerTypeExtendedSettingsContext
    {
         InvoiceViewerTypeSettings InvoiceViewerTypeSettings { get; set; }
         int UserId { get; set; }
    }
    public class InvoiceViewerTypeGridField
    {
        public string Header { get; set; }
        public InvoiceField Field { get; set; }
        public string CustomFieldName { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }

    }
}
