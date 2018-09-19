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

        public List<PabxContractDetail> GetDeactivatePabxContractsDetail(string customerId, string contractId)
        {
            List<PabxContractDetail> contracts = RatePlanMockDataGenerator.checkIsContactNotPilot(customerId, contractId);

            return contracts.Count == 0 ? RatePlanMockDataGenerator.GetFilteredPabxContracts(customerId, contractId) : contracts;
        }

        //public List<TelephonyContractDetail> GetFilteredTelephonyContractsInfo(string customerId)
        //{
        //    bool isExist = false;

        //    List<TelephonyContractDetail> filteredTelephonyContracts = new List<TelephonyContractDetail>();
        //    List<TelephonyContractDetail> contracts =  RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
        //    List<PabxContractDetail> pabxContracts = RatePlanMockDataGenerator.GetPabxContracts(customerId);

        //    foreach (TelephonyContractDetail con in contracts)
        //    {

        //        foreach (PabxContractDetail c in pabxContracts)
        //        {
        //            if (con.ContractId == c.ContractId)
        //            {
        //                isExist = true;
        //            }
        //        }

        //        if (!isExist) { filteredTelephonyContracts.Add(con); }
        //        isExist = false;
        //    }

        //    return filteredTelephonyContracts;
        //}

       


    }
}
