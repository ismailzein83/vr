using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace PartnerPortal.CustomerAccess.MainExtensions
{
    public class DefaultAccountStatementExtendedSettings : AccountStatementExtendedSettings
    {
        public override IEnumerable<PortalBalanceAccount> GetBalanceAccounts(IAccountStatementExtendedSettingsContext context)
        {
            context.AccountStatementViewData.ThrowIfNull("context.AccountStatementViewData");
        
            var accountId = new RetailAccountUserManager().GetRetailAccountId(context.UserId);

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(context.AccountStatementViewData.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            List<ClientBalanceAccountInfo> result = connectionSettings.Get<List<ClientBalanceAccountInfo>>(string.Format("/api/Retail_BE/FinancialAccount/GetClientBalanceAccounts?accountTypeId={0}&accountId={1}", context.AccountStatementViewData.AccountTypeId, accountId));
            List<PortalBalanceAccount> returnedResult = null;
            if (result != null)
            {
                returnedResult = new List<PortalBalanceAccount>();
                foreach (var item in result)
                {
                    returnedResult.Add(new PortalBalanceAccount
                    {
                        Name = item.Name,
                        PortalBalanceAccountId = item.BalanceAccountId
                    });
                }
            }
            return returnedResult;
        }
        public override Guid ConfigId
        {
            get { return new Guid("BB59E6BE-A590-4142-A5B4-F65DAAF02FF6"); }
        }
    }
}
