using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountActionDefinitionCheckAccessContext : IAccountActionDefinitionCheckAccessContext
    {
        public Guid AccountBEDefinitionId { get; set; }

        public int UserId { get; set; }
    }
}
