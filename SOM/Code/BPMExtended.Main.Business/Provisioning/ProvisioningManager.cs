using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;

namespace BPMExtended.Main.Business
{
    public class ProvisioningManager
    {

        public string ResetNetworkServicePassword(string contractId , string requestId, string serviceId, string linePathId)
        {
            string processInstanceId = string.Empty;
            var somRequestInput = new SOMRequestInput<ResetNetworkServicePasswordRequestInput>
            {
                InputArguments = new ResetNetworkServicePasswordRequestInput
                {
                    CommonInputArgument = new CommonInputArgument()
                    {
                        ContractId = contractId,
                        RequestId = requestId
                    },
                    ServiceId = serviceId,
                    LinePathId = linePathId
                }
            };

            using (var client = new SOMClient())
            {
                client.Post<SOMRequestInput<ResetNetworkServicePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitResetKeyword/StartProcess", somRequestInput);
            }

            return processInstanceId;

        }
    }
}
