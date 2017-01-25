using Vanrise.AccountBalance.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountStatementContextHandlerContext : IAccountStatementContextHandlerContext
    {
        public AccountStatementQuery Query { get; set; }
    }
}
