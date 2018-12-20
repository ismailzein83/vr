using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace CP.WhS.Business
{
    public class BlockedAttemptsManager
    {
        public IDataRetrievalResult<BlockedAttemptDetail> GetFilteredBlockedAttempts(DataRetrievalInput<BlockedAttemptQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<BlockedAttemptQuery>>(query);
            clonedInput.IsAPICall = true;
            WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
            if (clonedInput.Query.Filter != null)
            {
                if (clonedInput.Query.Filter.CustomerIds != null && query.Query.Filter.CustomerIds.Count > 0)
                {
                    foreach (var customerId in query.Query.Filter.CustomerIds)
                    {
                        if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == customerId) == null)
                            return null;
                    }
                }
                else
                {
                    clonedInput.Query.Filter.CustomerIds = accessibleCarrierAccounts != null && accessibleCarrierAccounts.Count() > 0 ? accessibleCarrierAccounts.Select(x => x.AccountId).ToList() : null;
                }
            }
            else
            {
                clonedInput.Query.Filter = new BlockedAttemptFilter()
                {
                    CustomerIds = accessibleCarrierAccounts != null && accessibleCarrierAccounts.Count() > 0 ? accessibleCarrierAccounts.Select(x => x.AccountId).ToList() : null
                };
            }
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<BlockedAttemptQuery>, RemoteExcelResult<BlockedAttemptDetail>>("/api/WhS_Analytics/BlockedAttempts/GetBlockedAttemptsData", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<BlockedAttemptQuery>, BigResult<BlockedAttemptDetail>>("/api/WhS_Analytics/BlockedAttempts/GetBlockedAttemptsData", clonedInput);
            }
            return null;
        }
    }
}
