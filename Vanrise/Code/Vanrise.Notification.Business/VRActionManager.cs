using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRActionManager
    {
        public void CreateAction(CreateVRActionInput createActionInput,int userId)
        {
            CreateActionProcessInput createActionProcessInput = new CreateActionProcessInput
            {
                CreateVRActionInput = createActionInput
            };
           
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput { InputArguments = createActionProcessInput };

            createProcessInput.InputArguments.UserId = userId;

            Vanrise.BusinessProcess.Business.BPInstanceManager bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
            var output = bpInstanceManager.CreateNewProcess(createProcessInput);                 
        }

        public IEnumerable<VRActionConfig> GetVRActionConfigs(string extensionType)
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<VRActionConfig>(extensionType);
        }
    }
}
