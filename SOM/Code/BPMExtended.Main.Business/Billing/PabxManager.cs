using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class PabxManager
    {

        public bool RegisterPabxContracts(string customerId, List<PabxContractDetail> contracts)
        {
            //send contracts to bscc

            return true;
        }

        public bool UpdatePabxContracts(string customerId , List<PabxContractDetail> contracts)
        {
            //send contracts to bscc

            return true;
        }

        public List<PabxContractDetail> GetPabxContractsDetail(string customerId)
        {
            return RatePlanMockDataGenerator.GetPabxContracts(customerId);
        }

        public bool DeactivatePabxContract(string contractId)
        {
            //Deactivate pabx contract pilot/secondary
            return true;
        }

        public bool CheckIfPilot(string customerId, string contractId)
        {
            return RatePlanMockDataGenerator.checkIfContactPilot(customerId, contractId).Count > 0 ? true : false ;

        }



       


    }
}
