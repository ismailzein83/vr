using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class StatusChargingManager
    {
        private StatusChargingSet GetChargingSet(int chargingSetId)
        {
            throw new NotImplementedException();
        }

        public bool HasInitialCharging(EntityType entityType, long entityId, Guid statusDefinitionId, out Decimal initialCharge)
        {
            StatusChargingSet chargingSet = GetChargingSet(entityType, entityId);
            if(chargingSet != null)
            {
                var statusCharge = chargingSet.Settings.StatusCharges.FirstOrDefault(itm => itm.StatusDefinitionId == statusDefinitionId);
                if(statusCharge != null)
                {
                    initialCharge = statusCharge.InitialPrice;
                    return true;
                }
            }
            initialCharge = 0;
            return false;
        }

        private StatusChargingSet GetChargingSet(EntityType entityType, long entityId)
        {
            switch(entityType)
            {
                case EntityType.Account: return GetAccountChargingSet(entityId);
                case EntityType.AccountService: return GetServiceChargingSet(entityId);
                default: throw new NotImplementedException(string.Format("entityType '{0}'", entityType));
            }
        }

        private StatusChargingSet GetAccountChargingSet(long accountId)
        {
            var account = (new AccountManager()).GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));
            if (account.Settings.StatusChargingSetId.HasValue)
                return GetChargingSet(account.Settings.StatusChargingSetId.Value);
            else
                return null;
        }
        private StatusChargingSet GetServiceChargingSet(long accountServiceId)
        {
            var accountService = (new AccountServiceManager()).GetAccountService(accountServiceId);
            if (accountService == null)
                throw new NullReferenceException(String.Format("accountService '{0}'", accountServiceId));
            var chargingPolicy = (new ChargingPolicyManager()).GetChargingPolicy(accountService.ServiceChargingPolicyId);
            if (chargingPolicy == null)
                throw new NullReferenceException(String.Format("chargingPolicy '{0}'", accountService.ServiceChargingPolicyId));
            if (chargingPolicy.Settings.StatusChargingSetId.HasValue)
                return GetChargingSet(chargingPolicy.Settings.StatusChargingSetId.Value);
            else
                return null;
        }
    }
}
