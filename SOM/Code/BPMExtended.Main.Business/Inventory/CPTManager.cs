using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.Entities;
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

        public ReserveCPTRequestOutput RegisterCPTNumber(string contractId, string cptId)
        {
            //TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            //ReserveCPTRequestInput input = new ReserveCPTRequestInput()
            //{
            //    PhoneNumber = contract.PhoneNumber,
            //    CPTID = cptId
            //};
            //ReserveCPTRequestOutput output = null;

            //using (var client = new SOMClient())
            //{
            //    output = client.Post<ReserveCPTRequestInput, ReserveCPTRequestOutput>("api/SOM_Main/Inventory/ReserveCPT", input);
            //}
            //return output;

            string result = null;
            TelephonyContractDetail contract = this._contractManager.GetTelephonyContract(contractId);
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<string>(String.Format("api/SOM_Main/Inventory/ReserveCPT?PhoneNumber={0}&CPTID={1}", contract.PhoneNumber, cptId));
            }
            ReserveCPTRequestOutput output = null;
            output.Message = result.ToString();
            return output;
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
