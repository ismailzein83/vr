using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public abstract class AccountStatementContextHandler
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IAccountStatementContextHandlerContext context);
    }

    public interface IAccountStatementContextHandlerContext
    {
        AccountStatementQuery Query { get; set; }
    }
}
