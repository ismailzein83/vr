using System;
using Vanrise.Security.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountStatementViewSettings : ViewSettings
    {
        public AccountStatementViewData AccountStatementViewData { get; set; }

        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/PartnerPortal_CustomerAccess/Elements/AccountStatement/Views/AccountStatementManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }

    public class AccountStatementViewData
    {
        public Guid VRConnectionId { get; set; }

        public Guid AccountTypeId { get; set; }

        public AccountStatementContextHandler AccountStatementHandler { get; set; }
    }
}
