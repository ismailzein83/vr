using PartnerPortal.CustomerAccess.Business;
using Retail.MultiNet.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
using Vanrise.Common;
namespace CP.MultiNet.Business
{
    public class AccountAdditionalInfoManager
    {
        public ClientAccountAdditionalInfo GetClientAccountAdditionalInfo(Guid vrConnectionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<ClientAccountAdditionalInfo>(string.Format("/api/Retail_MultiNet/MultiNetAccountInfo/GetClientAccountAdditionalInfo?accountBEDefinitionId={0}&accountId={1}", accountInfo.AccountBEDefinitionId, accountInfo.AccountId));
        }
    }
}
