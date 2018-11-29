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
        public List<TelephonyContractDetail> GetTelephonyContractsByNumber(string phoneNumber)
        {
            return RatePlanMockDataGenerator.GetTelephonyContractsByNumber(phoneNumber);
        }
        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId, string pilotContractId)
        {
            List<TelephonyContractDetail> contracts = RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
            List<PabxContractDetail> pabxContracts = RatePlanMockDataGenerator.GetPabxContracts(customerId);
            InventoryManager manager = new InventoryManager();

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {
                if (pabxContracts.Find(x => x.ContractId == item.ContractId) != null)
                    return false;

                //TODO: AYman to implement if not on same switch
                if (!manager.IsNumbersOnSameSwitch(item.ContractId, pilotContractId)) return false;

                return true;
            };

            return contracts.MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        }

        public GSHDSLContractDetail GetGSHDSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContract(contractId);
        }

        public List<GSHDSLContractDetail> GetGSHDSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContracts(customerId);
        }

        public ADSLContractDetail GetADSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetADSLContract(contractId);
        }

        public List<ADSLContractDetail> GetADSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetADSLContracts(customerId);
        }
        public List<ADSLContractDetail> GetADSLContractsByUsername(string userName)
        {
            return RatePlanMockDataGenerator.GetADSLContractsByUsername(userName);
        }
        public bool IsTelephonyLineHasADSLContract(string contractId, string phoneNumber)
        {
            //TODO: check if Telephony Line  mapped to an ADSL contract

            //return random boolean value
            ContractManager contractManager = new ContractManager();
            TelephonyContractDetail contract = contractManager.GetTelephonyContract(contractId);

            SOM.Main.Entities.LinePath item = new SOM.Main.Entities.LinePath();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<SOM.Main.Entities.LinePath>(String.Format("api/SOM_Main/Inventory/CheckADSL?phoneNumber={0}", contract.PhoneNumber));
            }
            string[] path = item.Path.Split(',');
            return path[6] == "1";
            //Random gen = new Random();
            //int prob = gen.Next(100);
            //return prob <= 50;
        }

        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId)
        {

            return RatePlanMockDataGenerator.GetTelephonyContractsInfo(customerId);
        }

        public List<TelephonyContractInfo> GetMoveADSLTelephonyContractsInfo(string customerId , string phoneNumber)
        {

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {

                if (item.PhoneNumber == phoneNumber)
                    return false;

                return true;
            };


            return RatePlanMockDataGenerator.GetTelephonyContracts(customerId).MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        }

        public List<TelephonyContractInfo> GetNewPabxTelephonyContractsInfo(string customerId, string pilotContractId)
        {
            InventoryManager manager = new InventoryManager();

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {

                if (!manager.IsNumbersOnSameSwitch(item.ContractId, pilotContractId)) return false;

                return true;
            };

            return RatePlanMockDataGenerator.GetTelephonyContracts(customerId).MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();


        }

        public LeasedLineContractDetail GetLeasedLineContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContract(contractId);
        }

        public List<LeasedLineContractDetail> GetLeasedLineContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContracts(customerId);
        }
        public List<LeasedLineContractDetail> GetLeasedLineContractsByNumber(string phoneNumber)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContractsByNumber(phoneNumber);
        }
        public string CountOfActiveContracts(string customerId , string contractId)
        {
            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6? "1" : "4";
        }

        public bool ContractTakeOver(string customerId, string contractId , string targetCustomerId , bool isAdslTakenOver , string username, string pass)
        {
            //TODO: 

            return true;
        }

        public bool AddADSLISPService(string contractId)
        {
            //
            return true;
        }

        public bool ActivateADSLISPService(string contractId, string customerId, string ispId, string port)
        {
            //TODO: Activate service
            return true;
        }


        public bool ActivateLeasedLineTermination(string contractId, string customerId)
        {
            //TODO: terminate the leased line contract of the customer
            return true;
        }


        public bool ActivateADSLLineMoving(string telephonyContractId, string customerId, string port)
        {
            //TODO: Activate service
            return true;
        }


        public bool ActivateADSLLineTermination(string contractId , string reason , string phoneNumber)
        {
            //TODO: Activate service
            return true;
        }


        public bool ActivateADSLAlterSpeed(string contractId, string speed)
        {
            //TODO: Activate service
            return true;
        }

        public bool ActivateGSHDSL(string contractId)
        {
            //TODO: Activate service
            return true;
        }

        public bool ActivateGSHDSLTermination(string contractId)
        {
            //TODO: Terminate GSHDSL contract
            return true;
        }

        public bool ChangeADSLPassword(string contractId, string newPassword)
        {
            //TODO: 
            return true;
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
