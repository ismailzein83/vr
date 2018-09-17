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

        public bool RegisterCPTNumber(string contractId, string cptId)
        {
            string result = null;
            TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<string>(String.Format("api/SOM_Main/Inventory/ReserveCPT?phoneNumber={0}&cPTId={1}", contract.PhoneNumber, cptId));
            }

            return result == null || result == "" ? false : true;
        }

        public bool UnRegisterCPTNumber(string contractId)
        {
            string result = null;
            TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<string>(String.Format("api/SOM_Main/Inventory/DeleteCPTReservation?phoneNumber={0}", contract.PhoneNumber));
            }
            
            return result==null || result== ""  ? false:true;
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
