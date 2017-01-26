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
            AccountStatementContextHandlerContext retailAccountUserHandlerContext = context as AccountStatementContextHandlerContext;
            if (retailAccountUserHandlerContext == null)
                throw new Exception("retailAccountUserHandlerContext is not of type RetailAccountUserHandlerContext.");

            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            retailAccountUserHandlerContext.Query.AccountId = manager.GetRetailAccountId(userId);
        }
    }
}