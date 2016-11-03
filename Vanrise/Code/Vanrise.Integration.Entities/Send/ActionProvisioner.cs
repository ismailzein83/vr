using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public enum ActionProvisioningResult { Succeeded = 0, Failed = 1 }

    public abstract class ActionProvisioner
    {
        public abstract void Execute(IActionProvisioningContext context);
    }

    public interface IActionProvisioningContext
    {
        ActionProvisionerDefinitionSettings DefinitionSettings { get; }

        dynamic Entity { get; }
        List<Object> ExecutedActionsData { get; set; }
        ActionProvisioningOutput ExecutionOutput { set; }

        bool IsWaitingResponse { set; }
    }

    public class ActionProvisioningOutput
    {
        public ActionProvisioningResult Result { get; set; }

        public string FailureMessage { get; set; }

        public Object ProvisioningData { get; set; }
    }
}
