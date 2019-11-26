using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class EditPABXManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public

        public List<PabxContractDetail> GetPABXSecondaryContractsDetails(string pilotContractId)
        {
            var secondaryContracts = new List<PabxContractDetail>();

            using (SOMClient client = new SOMClient())
            {
                secondaryContracts = client.Get<List<PabxContractDetail>>(String.Format("api/SOM.ST/Billing/GetPABXSecondaryContractsDetails?contractId={0}", pilotContractId));
            }

            return secondaryContracts;

        }

        public void SubmitModifyPABXSubscription(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StManagePabx");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StPabxSecondaryContracts");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                List<PabxContractInput> selectedSecondaryContracts = JsonConvert.DeserializeObject<List<PabxContractInput>>(entities[0].GetColumnValue("StPabxSecondaryContracts").ToString());


                SOMRequestInput<EditPABXRequestInput> somRequestInput = new SOMRequestInput<EditPABXRequestInput>
                {

                    InputArguments = new EditPABXRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        SubmitPabxInput = new SubmitPabxInput()
                        {
                            //PilotContract = pilotContract,
                            Contracts = selectedSecondaryContracts
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<EditPABXRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitModifyPABXSubscription/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FinalizeModifyPABXSubscription(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StManagePabx");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<EditPABXRequestInput> somRequestInput = new SOMRequestInput<EditPABXRequestInput>
                {

                    InputArguments = new EditPABXRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<EditPABXRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeModifyPABXSubscription/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        #endregion
    }
}
