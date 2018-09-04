using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CPTManager
    {
        ContractManager _contractManager = new ContractManager();

        public List<CPTNumberDetail> GetFreeNumbers(string contractId)
        {
            TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            //TODO: pass the phone number to inventory
            return InventoryMockDataGenerator.GetAllCPTNumbers();
        }

        public bool RegisterCPTNumber(string contractId, string cptNumber)
        {
            TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            //TODO: get the phone number
            return true;
        }

        public bool UnRegisterCPTNumber(string contractId, string cptNumber)
        {
            return true;
        }

        public List<CPTNumberDetail> GetCountrFreeCptNumbers()
        {
            return InventoryMockDataGenerator.GetAllCPTNumbers();
        }

        public bool RegisterCountryCPTNumber(string cptNumber)
        {
            
            //TODO: send request to inventory system to register cptnumber in all provinces switches
            
            return true;
        }

    }
}
