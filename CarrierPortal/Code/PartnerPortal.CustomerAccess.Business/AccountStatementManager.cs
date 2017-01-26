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
            var vrConnection = connectionManager.GetVRConnection(settings.AccountStatementViewData.VRConnectionId);

            if (vrConnection == null)
                throw new NullReferenceException("vrConnection");

            if (vrConnection.Settings == null)
                throw new NullReferenceException("vrConnection.Settings");

            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            if (connectionSettings == null)
                throw new Exception(String.Format("vrConnection.Settings is not of type VRInterAppRestConnection. it is of type '{0}'.", vrConnection.Settings.GetType()));
            
            return connectionSettings.Post<DataRetrievalInput<AccountStatementAppQuery>, AccountStatementResult>("/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", input);

            //CredentialsInput credentialsInput = new CredentialsInput() { Email = "admin@vanrise.com", Password = "1" };
            //var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>("http://localhost:6655",
            //     "/api/VR_Sec/Security/Authenticate", credentialsInput, null);

            //Dictionary<string, string> headers = new Dictionary<string, string>();
            //headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);

            //input.Query.AccountId = 419231;
            //input.Query.AccountTypeId = new Guid("20b0c83e-6f53-49c7-b52f-828a19e6dc2a");

            //return Vanrise.Common.VRWebAPIClient.Post<DataRetrievalInput<AccountStatementQuery>, AccountStatementResult>("http://localhost:6655",
            //    "/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", input, headers);
        }

        public IEnumerable<AccountStatementContextHandlerTemplate> GetAccountStatementContextHandlerTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountStatementContextHandlerTemplate>(AccountStatementContextHandlerTemplate.EXTENSION_TYPE);
        }
    }
}