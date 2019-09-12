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
    public class LineTerminationManager
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
        public void SubmitLineTerminationToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineTerminationRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StHasADSL");
            esq.AddColumn("StReason");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StLinePathId");
                bool hasADSL = (bool)entities[0].GetColumnValue("StHasADSL");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                ContractEntity entity = new ContractManager().GetChildADSLContractByTelephonyContract(contractId.ToString());

                SOMRequestInput<LineTerminationRequestInput> somRequestInput = new SOMRequestInput<LineTerminationRequestInput>
                {

                    InputArguments = new LineTerminationRequestInput
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
                        LinePathId = pathId.ToString(),
                        HasADSL = hasADSL,
                        Reason = reason.ToString(),
                        ADSLContract = entity == null?string.Empty: entity.ContractId
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/TelephonySubmitLineTermination/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void ActivateLineTerminationToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineTerminationRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StHasADSL");
            esq.AddColumn("StReason");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StLinePathId");
                bool hasADSL = (bool)entities[0].GetColumnValue("StHasADSL");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                ContractEntity entity = new ContractManager().GetChildADSLContractByTelephonyContract(contractId.ToString());

                SOMRequestInput<LineTerminationRequestInput> somRequestInput = new SOMRequestInput<LineTerminationRequestInput>
                {

                    InputArguments = new LineTerminationRequestInput
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
                        LinePathId = pathId.ToString(),
                        HasADSL = hasADSL,
                        Reason = reason.ToString(),
                        ADSLContract = entity == null ? string.Empty : entity.ContractId
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/TelephonyFinalizeLineTermination/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        public List<Reason> GetStatusChangeReasons()
        {
            List<Reason> items = new List<Reason>();

            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<Reason>>(String.Format("api/SOM.ST/Billing/GetStatusChangeReasons"));
            }

            return items;
        }

        #endregion
    }
}
