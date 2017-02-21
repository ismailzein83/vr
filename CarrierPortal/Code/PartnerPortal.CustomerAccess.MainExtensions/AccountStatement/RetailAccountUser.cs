using System;
using Vanrise.Security.Business;
using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;

namespace PartnerPortal.CustomerAccess.MainExtensions.AccountStatement
{
    public class RetailAccountUser : AccountStatementContextHandler
    {
        public override Guid ConfigId { get { return new Guid("A8085279-37BF-40C5-941B-A1E46F83DFAB"); } }

        public override void PrepareQuery(IAccountStatementContextHandlerContext context)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            context.Query.AccountId = manager.GetRetailAccountId(userId).ToString();
        }
    }
}