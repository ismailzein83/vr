using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;
using PartnerPortal.CustomerAccess.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
using Vanrise.Common;
namespace PartnerPortal.CustomerAccess.Business
{
    public class AccountStatementManager
    {
        public IDataRetrievalResult<AccountStatementItem> GetFilteredAccountStatments(DataRetrievalInput<AccountStatementAppQuery> input)
        {
            ViewManager viewManager = new ViewManager();
            View view = viewManager.GetView(input.Query.ViewId);
            if (view == null)
                throw new NullReferenceException("view");

            if (view.Settings == null)
                throw new NullReferenceException("view.Settings");

            PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings settings = view.Settings as PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings;
            if (settings == null)
                throw new Exception(String.Format("view.Settings is not of type AccountStatementViewSettings. it is of type '{0}'.", view.Settings.GetType()));

            input.Query.AccountTypeId = settings.AccountStatementViewData.AccountTypeId;

            if (settings.AccountStatementViewData.AccountStatementHandler != null)
            {
                AccountStatementContextHandlerContext context = new AccountStatementContextHandlerContext() { Query = input.Query };
                settings.AccountStatementViewData.AccountStatementHandler.PrepareQuery(context);
            }
            if (!ValidateBalanceAccount(settings.AccountStatementViewData, input.Query.AccountId))
            {
                throw new Exception("Account not valid.");
            }

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(settings.AccountStatementViewData.VRConnectionId);

            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            var clonedInput = Vanrise.Common.Utilities.CloneObject<DataRetrievalInput<AccountStatementAppQuery>>(input);
            clonedInput.IsAPICall = true;

            if (input.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<AccountStatementAppQuery>, RemoteExcelResult<AccountStatementItem>>("/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", clonedInput);
            }
            else
                return connectionSettings.Post<DataRetrievalInput<AccountStatementAppQuery>, AccountStatementResult>("/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", clonedInput);
        }

        public IEnumerable<AccountStatementContextHandlerTemplate> GetAccountStatementContextHandlerTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountStatementContextHandlerTemplate>(AccountStatementContextHandlerTemplate.EXTENSION_TYPE);
        }
        public IEnumerable<PortalBalanceAccount> GetBalanceAccounts(Guid viewId)
        {
            ViewManager viewManager = new ViewManager();
            var view = viewManager.GetView(viewId);
            view.ThrowIfNull("view", viewId);
            view.Settings.ThrowIfNull("view.Settings");
            var accountStatementViewSettings = view.Settings as PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings;
            accountStatementViewSettings.ThrowIfNull("accountStatementViewSettings");
            accountStatementViewSettings.AccountStatementViewData.ThrowIfNull("accountStatementViewSettings.AccountStatementViewData");
            accountStatementViewSettings.AccountStatementViewData.ExtendedSettings.ThrowIfNull("accountStatementViewSettings.AccountStatementViewData.ExtendedSettings");
            AccountStatementExtendedSettingsContext context = new AccountStatementExtendedSettingsContext
            {
                AccountStatementViewData = accountStatementViewSettings.AccountStatementViewData,
                UserId = SecurityContext.Current.GetLoggedInUserId()
            };
            return accountStatementViewSettings.AccountStatementViewData.ExtendedSettings.GetBalanceAccounts(context);
        }
        public IEnumerable<AccountStatementExtendedSettingsConfigs> GetAccountStatementExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountStatementExtendedSettingsConfigs>(AccountStatementExtendedSettingsConfigs.EXTENSION_TYPE);
        }

        public bool ValidateBalanceAccount(AccountStatementViewData accountStatementViewData, string accountId)
        {
            accountStatementViewData.ThrowIfNull("accountStatementViewData");
            accountStatementViewData.ExtendedSettings.ThrowIfNull("settings.AccountStatementViewData.ExtendedSettings");
            AccountStatementExtendedSettingsContext accountStatementExtendedSettingsContext = new Business.AccountStatementExtendedSettingsContext
            {
                AccountStatementViewData = accountStatementViewData,
                UserId = SecurityContext.Current.GetLoggedInUserId()
            };
            var balanceAccounts = accountStatementViewData.ExtendedSettings.GetBalanceAccounts(accountStatementExtendedSettingsContext);
            balanceAccounts.ThrowIfNull("balanceAccounts");

            if (balanceAccounts.FindRecord(x => x.PortalBalanceAccountId == accountId) == null)
            {
                return false;
            }
            return true;
        }
    }
}