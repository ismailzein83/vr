using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountProvisioner
    {
        public abstract void Execute(IAccountProvisioningContext context);
    }
    public interface IAccountProvisioningContext
    {
        AccountProvisionerDefinitionSettings DefinitionSettings { get; }
         Guid ActionDefinitionId { get;  }
         string ActionDefinitionName { get;  }
        long AccountId { get; }
        Guid AccountBEDefinitionId { get; }
        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);
        
        void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args);
        void TrackActionExecuted(string actionDescription, Object technicalInformation);
        void TrackActionExecuted(long accountId, string actionDescription, Object technicalInformation);
    }

   
}
