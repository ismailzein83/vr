using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountSynchronizerInsertHandler
    {
        public Guid AccountSynchronizerInsertHandlerId { get; set; }

        public string Name { get; set; }

        public AccountCondition AccountCondition { get; set; }

        public AccountSynchronizerInsertHandlerSettings Settings { get; set; }
    }

    public abstract class AccountSynchronizerInsertHandlerSettings
    {
        public virtual void OnPreInsert(IAccountSynchronizerInsertHandlerPreInsertContext context)
        {

        }

        public virtual void OnPostInsert(IAccountSynchronizerInsertHandlerPostInsertContext context)
        {

        }
    }

    public interface IAccountSynchronizerInsertHandlerPreInsertContext
    {
        Guid AccountBEDefinitionId { get; }

        Account Account { get; }
    }

    public interface IAccountSynchronizerInsertHandlerPostInsertContext
    {
        Guid AccountBEDefinitionId { get; }

        Account Account { get; }
    }
}
