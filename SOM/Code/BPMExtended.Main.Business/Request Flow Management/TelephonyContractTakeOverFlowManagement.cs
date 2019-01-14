using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class TelephonyContractTakeOverFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "496681F9-8A5D-4CAD-89E3-2918B8A3EB5D";
        const string targetCustomerStep = "6491A4EF-29F6-4879-BAD8-30884F621065";
        const string printStep = "056AC63F-B743-4A7E-B007-371014BEF79F";
        const string billOnDemandStep = "6430317A-87B7-4703-AEAF-38E8B00A6479";
        const string paymentStep = "90ED9157-3D6E-41F8-BCA4-41845BF6BBDD";
        const string adslCredentialStep = "0ED21643-2A5C-4CB2-8320-64B67D30D516";
      //  const string pdnTeamStep = "CE8774B8-154B-485F-970B-8FAE05BB1902";
       // const string mdfTeamStep = "CD2A5018-23B0-4F36-942C-E450FDBBD4F0";
        const string technicalStep = "DF7AC0B3-F53D-4104-A260-2509A3E08769";
        const string completedStep = "A826F4E8-9352-46FB-814E-8B0B52655659";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = targetCustomerStep; break;
                case targetCustomerStep: nextStepId = printStep; break;
                case printStep: nextStepId = NBOfActiveContracts(id) == "1" ? billOnDemandStep : paymentStep; break;
                case billOnDemandStep: nextStepId = paymentStep; break;
                //  case paymentStep: nextStepId = IsTelephonyLineHasADSLContract(id)? adslCredentialStep : mdfTeamStep; break;
                //  case adslCredentialStep: nextStepId = pdnTeamStep; break;
                //   case pdnTeamStep: nextStepId = mdfTeamStep; break;
                //   case mdfTeamStep: nextStepId = completedStep; break;
                case paymentStep: nextStepId = adslCredentialStep; break;
                case adslCredentialStep: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completedStep; break;

            }
            return nextStepId;
        }

        public string NBOfActiveContracts(string id)
        {

            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6 ? "1" : "4";
        }

        public bool IsTelephonyLineHasADSLContract(string id)
        {
            //TODO: check if Telephony Line  mapped to an ADSL contract

          
                //get request from bpm
                Guid idd = new Guid(id.ToUpper());
                // Creation of query instance with "City" root schema. 
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyContractTakeOver");
                esq.AddColumn("Id");
                esq.AddColumn("StContractId");
                // Creation of the first filter instance.
                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                // Adding created filters to query collection. 
                esq.Filters.Add(esqFirstFilter);
                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esq.GetEntityCollection(BPM_UserConnection);
                
                var contractId = entities[0].GetColumnValue("StContractId");
                ContractManager contractManager = new ContractManager();
                TelephonyContractDetail contract = contractManager.GetTelephonyContract(contractId.ToString());

                SOM.Main.Entities.LinePath item = new SOM.Main.Entities.LinePath();
                using (SOMClient client = new SOMClient())
                {
                    item = client.Get<SOM.Main.Entities.LinePath>(String.Format("api/SOM_Main/Inventory/CheckADSL?phoneNumber={0}", contract.PhoneNumber));
                }
                string[] path = item.Path.Split(',');
                return path[6] == "1";
             
        }


    }
}
