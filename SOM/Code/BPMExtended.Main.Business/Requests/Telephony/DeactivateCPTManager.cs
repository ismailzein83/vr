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
    public class DeactivateCPTManager
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

        public void SubmitDeactivateCpt(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeactivateCpt");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerID");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StCptNumber");



            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerID");
                var linePathId = entities[0].GetColumnValue("StLinePathId");
                var cptNumber = entities[0].GetColumnValue("StCptNumber");

                SOMRequestInput<DeactivateCptRequestInput> somRequestInput = new SOMRequestInput<DeactivateCptRequestInput>
                {

                    InputArguments = new DeactivateCptRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        CPTServiceId = new CatalogManager().GetCPTServiceId(),
                        LinePathId = linePathId.ToString(),
                        CPTNumber = cptNumber.ToString()
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<DeactivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitDeactivateCPT/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        public void FinalizeDeactivateCPT(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeactivateCpt");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerID");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCptId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var cptId = entities[0].GetColumnValue("StCptId");

                SOMRequestInput<FinalizeDeactivateCptRequestInput> somRequestInput = new SOMRequestInput<FinalizeDeactivateCptRequestInput>
                {

                    InputArguments = new FinalizeDeactivateCptRequestInput
                    {
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        CPTServiceId = new CatalogManager().GetCPTServiceId(),
                        CPTId = cptId.ToString(),
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<FinalizeDeactivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeDeactivateCPT/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        #endregion
    }
}
