using System.IO;
using System.Net;
using System.Text;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;

namespace PartnerPortal.CustomerAccess.Business
{

    public class AccountStatementManager
    {
        public IDataRetrievalResult<AccountStatementItem> GetFilteredAccountStatments(DataRetrievalInput<AccountStatementQuery> input)
        {
            CredentialsInput credentialsInput = new CredentialsInput() { Email = "admin@vanrise.com", Password = "1" };
            var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>("http://localhost:6655",
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);

            input.Query.AccountId = 419231;
            input.Query.AccountTypeId = new Guid("20b0c83e-6f53-49c7-b52f-828a19e6dc2a");

            return Vanrise.Common.VRWebAPIClient.Post<DataRetrievalInput<AccountStatementQuery>, AccountStatementResult>("http://localhost:6655",
                "/api/VR_AccountBalance/AccountStatement/GetFilteredAccountStatments", input, headers);
        }
    }
}
