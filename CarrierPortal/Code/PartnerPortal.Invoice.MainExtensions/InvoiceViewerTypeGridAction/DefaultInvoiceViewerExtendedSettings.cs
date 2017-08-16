using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.Invoice.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace PartnerPortal.Invoice.MainExtensions
{
    public class DefaultInvoiceViewerExtendedSettings : InvoiceViewerTypeExtendedSettings
    {
        public override IEnumerable<PortalInvoiceAccount> GetInvoiceAccounts(IInvoiceViewerTypeExtendedSettingsContext context)
        {
            context.InvoiceViewerTypeSettings.ThrowIfNull("context.InvoiceViewerTypeSettings");
            var accountId = new RetailAccountUserManager().GetRetailAccountId(context.UserId);


            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(context.InvoiceViewerTypeSettings.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            List<ClientInvoiceAccountInfo> result = connectionSettings.Get<List<ClientInvoiceAccountInfo>>(string.Format("/api/Retail_BE/FinancialAccount/GetClientInvoiceAccounts?invoiceTypeId={0}&accountId={1}", context.InvoiceViewerTypeSettings.InvoiceTypeId, accountId));
            List<PortalInvoiceAccount> returnedResult = null; 
            if(result != null)
            {
                returnedResult = new List<PortalInvoiceAccount>(); 
                foreach(var item in result)
                {
                    returnedResult.Add(new PortalInvoiceAccount
                    {
                        Name = item.Name,
                        PortalInvoiceAccountId = item.InvoiceAccountId
                    });
                }
            }
            return returnedResult;
        }
        public override Guid ConfigId
        {
            get { return new Guid("06377156-8265-4424-BD62-46FA5AB2CE41"); }
        }
    }
}
