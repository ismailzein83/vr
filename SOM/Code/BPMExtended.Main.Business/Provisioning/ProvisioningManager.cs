using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class ProvisioningManager
    {

        public bool ResetNetworkServicePassword(string contractId , string serviceId)
        {
            //TODO: call Provisioning to reset the network service password
            SOMRequestOutput output;

            SOMRequestInput<ResetNetworkServicePasswordRequestInput> somRequestInput = new SOMRequestInput<ResetNetworkServicePasswordRequestInput>
            {
                InputArguments = new ResetNetworkServicePasswordRequestInput
                {
                    CommonInputArgument = new CommonInputArgument()
                    {
                        // ContractId = contractId.ToString(),
                       // RequestId = requestId.ToString(),
                        // CustomerId = customerId.ToString()
                    }
                }

            };


            using (var client = new SOMClient())
            {
                output = client.Post<SOMRequestInput<ResetNetworkServicePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_NetworkServiceResetPassword/StartProcess", somRequestInput);
            }


            return true;

        }
    }
}
