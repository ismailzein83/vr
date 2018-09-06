using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ContractManager
    {
        public TelephonyContractDetail GetTelephonyContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetTelephonyContract(contractId);
        }

        public List<TelephonyContractDetail> GetTelephonyContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
        }

        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId)
        {
            return RatePlanMockDataGenerator.GetTelephonyContractsInfo(customerId);
        }

        public LeasedLineContractDetail GetLeasedLineContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContract(contractId);
        }

        public List<LeasedLineContractDetail> GetLeasedLineContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContracts(customerId);
        }
    }
}
