using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;
using PartnerPortal.CustomerAccess.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

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

            AccountStatementContextHandlerContext context = new AccountStatementContextHandlerContext() { Query = input.Query };
            settings.AccountStatementViewData.AccountStatementHandler.PrepareQuery(context);

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(settings.AccountStatementViewData.VRConnectionId);

            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Post<DataRetrievalInput<AccountStatementAppQuery>, AccountStatementResult>("/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", input);
        }

        public IEnumerable<AccountStatementContextHandlerTemplate> GetAccountStatementContextHandlerTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountStatementContextHandlerTemplate>(AccountStatementContextHandlerTemplate.EXTENSION_TYPE);
        }
    }
}