using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace PartnerPortal.CustomerAccess.Business
{
    public class DIDManager
    {

        public IDataRetrievalResult<DIDClientDetail> GetFilteredDIDs(DataRetrievalInput<DIDAppQuery> input)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            DataRetrievalInput<DIDQuery> query = new DataRetrievalInput<DIDQuery>
            {
                DataRetrievalResultType = input.DataRetrievalResultType,
                FromRow = input.FromRow,
                SortByColumnName = input.SortByColumnName,
                GetSummary = input.GetSummary,
                IsSortDescending = input.IsSortDescending,
                ResultKey = input.ResultKey,
                ToRow = input.ToRow,
                Query = new DIDQuery
                {
                    WithSubAccounts = input.Query.WithSubAccounts,
                    AccountIds = new List<long> { accountInfo.AccountId },
                },
                IsAPICall= true
            };

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(input.Query.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            if (input.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<DIDQuery>, RemoteExcelResult<DIDClientDetail>>("/api/Retail_BE/DID/GetFilteredClientDIDs", query);
            }
            else
                return connectionSettings.Post<DataRetrievalInput<DIDQuery>, BigResult<DIDClientDetail>>("/api/Retail_BE/DID/GetFilteredClientDIDs", query);
        }
    }
}
