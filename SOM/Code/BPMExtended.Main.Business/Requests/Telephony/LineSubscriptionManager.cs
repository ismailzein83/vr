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
    public class LineSubscriptionManager
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
        public void PostLineSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
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

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }

        public void ActivateLineSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
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

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }


        }

        public void CreateSwitchAccount(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StDeviceType");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var deviceType = entities[0].GetColumnValue("StDeviceType");

                SOMRequestInput<CreateSwitchAccountInput> somRequestInput = new SOMRequestInput<CreateSwitchAccountInput>
                {

                    InputArguments = new CreateSwitchAccountInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            RequestId = requestId.ToString()
                        },
                        LinePathId = pathId.ToString(),
                        DeviceType = deviceType.ToString()
                    },
                    

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<CreateSwitchAccountInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateSwitchAccount/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }

        public void CancelLineSubscriptionRequest(Guid requestId)
        {
            //SOMRequestOutput output;
            //SOMRequestInput<CancelSuspensionRequestInput> somRequestInput = new SOMRequestInput<CancelSuspensionRequestInput>
            //{
            //    InputArguments = new CancelSuspensionRequestInput { RequestId = requestId.ToString() }
            //};
            //using (var client = new SOMClient())
            //{
            //    output = client.Post<SOMRequestInput<CancelSuspensionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CancellingContractSuspensionFromCustomerCare/StartProcess", somRequestInput);
            //}
            //var manager = new BusinessEntityManager();
            //manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
        }
        #endregion
    }
}
