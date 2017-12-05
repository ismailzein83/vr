using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerAssignmentDefinition
    {
        public Guid AccountManagerAssignementDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountManagerAssignmentDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountManagerAssignmentDefinitionSettings
    {
        public abstract Guid ConfigId { get;  }
        public abstract string RuntimeEditor { get; }
        public abstract string GetAccountName(string accountId);
        public virtual void TrackAndLogObject(IAssignmentDefinitionTrackAndLogObject context)
        {

        }
    }
    public interface IAssignmentDefinitionTrackAndLogObject
    {
        AccountManagerAssignment AccountManagerAssignment{get;}
    }
}
