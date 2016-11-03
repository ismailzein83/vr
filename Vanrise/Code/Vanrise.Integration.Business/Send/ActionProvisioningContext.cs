using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class ActionProvisioningContext : IActionProvisioningContext
    {
        public dynamic Entity { get; set; }

        public ActionProvisioningOutput ExecutionOutput
        {
            get;
            set;
        }


        public bool IsWaitingResponse
        {
            get;
            set;
        }

        public ActionProvisionerDefinitionSettings DefinitionSettings
        {
            get;
            set;
        }


        public List<object> ExecutedActionsData
        {
            get;
            set;
        }
    }
}
