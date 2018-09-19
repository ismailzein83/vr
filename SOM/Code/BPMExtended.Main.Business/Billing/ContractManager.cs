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

        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId, string pilotContractId)
        {
            List<TelephonyContractDetail> contracts = RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
            List<PabxContractDetail> pabxContracts = RatePlanMockDataGenerator.GetPabxContracts(customerId);

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {
                if (pabxContracts.Find(x => x.ContractId == item.ContractId) != null)
                    return false;

                //TODO: AYman to implement if not on same switch

                return true;
            };

            return contracts.MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        }

       
        public ADSLContractDetail GetADSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetADSLContract(contractId);
        }

        public List<ADSLContractDetail> GetADSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetADSLContracts(customerId);
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


        #region mappers

        private TelephonyContractInfo TelephonyContractDetailToInfo(TelephonyContractDetail detail)
        {
            return new TelephonyContractInfo
            {
                ContractId = detail.ContractId,
                CustomerId = detail.CustomerId,
                Address = detail.Address,
                PhoneNumber = detail.PhoneNumber
            };
        }

        #endregion


    }
}
