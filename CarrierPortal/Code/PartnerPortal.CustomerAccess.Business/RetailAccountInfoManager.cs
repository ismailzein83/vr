using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
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
        public IEnumerable<GenericFieldDefinitionInfo> GetRemoteGenericFieldDefinitionsInfo(Guid vrConnectionId, Guid accountBEDefinitionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<IEnumerable<GenericFieldDefinitionInfo>>(string.Format("/api/Retail_BE/AccountType/GetClientGenericFieldDefinitionsInfo?accountBEDefinitionId={0}", accountBEDefinitionId));
        }
        public IDataRetrievalResult<AccountClientDetail> GetFilteredSubAccounts(DataRetrievalInput<AccountAppQuery> input)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            long accountId = accountInfo.AccountId;
            if (input.Query.ParentAccountId.HasValue)
                accountId = input.Query.ParentAccountId.Value;

            DataRetrievalInput<AccountQuery> query = new DataRetrievalInput<AccountQuery>
            {
                DataRetrievalResultType = input.DataRetrievalResultType,
                FromRow = input.FromRow,
                SortByColumnName = input.SortByColumnName,
                GetSummary = input.GetSummary,
                IsSortDescending = input.IsSortDescending,
                ResultKey = input.ResultKey,
                ToRow = input.ToRow,
                Query = new AccountQuery
                {
                    AccountBEDefinitionId = accountInfo.AccountBEDefinitionId,
                    ParentAccountId = accountId,
                    Columns = input.Query.Columns
                },
                IsAPICall = true
            };

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(input.Query.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            if (input.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<AccountQuery>, RemoteExcelResult<AccountClientDetail>>("/api/Retail_BE/AccountBE/GetFilteredClientAccounts", query);
            }
            else
                return connectionSettings.Post<DataRetrievalInput<AccountQuery>, BigResult<AccountClientDetail>>("/api/Retail_BE/AccountBE/GetFilteredClientAccounts", query);
        }
        public List<DataRecordGridColumnAttribute> GetSubAccountsGridColumnAttributes(AccountGridFieldInput input)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);

            long accountId = accountInfo.AccountId;
            if (input.ParentAccountId.HasValue)
                accountId = input.ParentAccountId.Value;
           
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(input.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            var gridColumnsAttributes = connectionSettings.Get<IEnumerable<DataRecordGridColumnAttribute>>(string.Format("/api/Retail_BE/AccountType/GetGenericFieldGridColumnAttribute?accountBEDefinitionId={0}", accountInfo.AccountBEDefinitionId));
            List<DataRecordGridColumnAttribute> gridColumnAttributes = null;
            if (gridColumnsAttributes != null && input.AccountGridFields != null)
            {
                 gridColumnAttributes = new List<DataRecordGridColumnAttribute>();
                foreach (var item in input.AccountGridFields)
                {
                    var gridColumnField = gridColumnsAttributes.FirstOrDefault(x => x.Name == item.FieldName);
                    if (gridColumnField != null && gridColumnField.Attribute != null)
                    {
                        int? widthFactor = null;
                        int? fixedWidth = null;
                        if (item.ColumnSettings != null)
                        {
                            widthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(item.ColumnSettings);
                            if (!widthFactor.HasValue)
                                fixedWidth = item.ColumnSettings.FixedWidth;
                        }
                        gridColumnField.Attribute.HeaderText = item.FieldTitle;
                        gridColumnField.Attribute.FixedWidth = fixedWidth;
                        gridColumnField.Attribute.WidthFactor =widthFactor;
                        gridColumnAttributes.Add(gridColumnField);
                    }
                }
                
            }
            return gridColumnAttributes;
        }
    }
}
