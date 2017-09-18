using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountInfoManager
    {
        public ClientRetailProfileAccountInfo GetClientProfileAccountInfo(Guid vrConnectionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<ClientRetailProfileAccountInfo>(string.Format("/api/Retail_BE/RetailClientAccount/GetClientProfileAccountInfo?accountBEDefinitionId={0}&accountId={1}", accountInfo.AccountBEDefinitionId, accountInfo.AccountId));
        }
    }
}
