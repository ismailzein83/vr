using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAP;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class SuspensionManager
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
        public void SubmitContractSuspension(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSuspension");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StLinePathId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var linePathId = entities[0].GetColumnValue("StLinePathId");


                SOMRequestInput<SuspensionRequestInput> somRequestInput = new SOMRequestInput<SuspensionRequestInput>
                {

                    InputArguments = new SuspensionRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        PhoneNumber = phoneNumber.ToString(),
                        LinePathId = linePathId.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<SuspensionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitContractSuspensionFromCustomerCare/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        public void FinalizeContractSuspension(Guid requestId)//
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSuspension");
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

                SOMRequestInput<SuspensionRequestInput> somRequestInput = new SOMRequestInput<SuspensionRequestInput>
                {

                    InputArguments = new SuspensionRequestInput
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
                        RequestId = requestId.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<SuspensionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeContractSuspensionFromCustomerCare/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        public void CancelContractSuspensionRequest(Guid requestId)
        {
            SOMRequestOutput output;
            SOMRequestInput<CancelSuspensionRequestInput> somRequestInput = new SOMRequestInput<CancelSuspensionRequestInput>
            {
                InputArguments = { RequestId = requestId.ToString() }
            };
            using (var client = new SOMClient())
            {
                output = client.Post<SOMRequestInput<CancelSuspensionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CancellingContractSuspensionFromCustomerCare/StartProcess", somRequestInput);
            }
            var manager = new BusinessEntityManager();
            manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
        }


    }
}
#endregion