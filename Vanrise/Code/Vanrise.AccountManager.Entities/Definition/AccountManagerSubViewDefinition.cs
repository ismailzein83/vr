using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerSubViewDefinition
    {
        public Guid AccountViewDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountManagerSubViewDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountManagerSubViewDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string RuntimeEditor { get; }

        public virtual bool DoesUserHaveAccess(IAccountManagerSubViewDefinitionCheckAccessContext context)
        {
            return true;
        }
    }

    public interface IAccountManagerSubViewDefinitionCheckAccessContext
    {
        Guid AccountManagerDefinitionId { get; }
        int UserId { get; }
    }

    //public class RetailAccountAccountManagerSubView : AccountManagerSubViewDefinitionSettings
    //{
    //    public override Guid ConfigId
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public Guid AccountDefinitionId { get; set; }

    //    public Object AccountCondition { get; set; }
    //}
}
