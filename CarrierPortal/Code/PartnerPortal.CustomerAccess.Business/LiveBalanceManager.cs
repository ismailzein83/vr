using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
using Vanrise.Common;
namespace PartnerPortal.CustomerAccess.Business
{
    public class LiveBalanceManager
    {
        public CurrentAccountBalanceTile GetCurrentAccountBalance(Guid connectionId, Guid accountTypeId, Guid? viewId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            string accountId = accountInfo.AccountId.ToString();
          
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            var currentAccountBalance =  connectionSettings.Get<CurrentAccountBalance>(string.Format("/api/VR_AccountBalance/LiveBalance/GetCurrentAccountBalance?accountId={0}&accountTypeId={1}", accountId, accountTypeId));
            CurrentAccountBalanceTile currentAccountBalanceTile = new CurrentAccountBalanceTile
            {
                CurrentAccountBalance = currentAccountBalance
            };
            if (viewId.HasValue)
            {
                ViewManager viewManager = new ViewManager();
                var view = viewManager.GetView(viewId.Value);
                currentAccountBalanceTile.ViewURL = view.Url;
            }
            return currentAccountBalanceTile;
        }
    }
}
