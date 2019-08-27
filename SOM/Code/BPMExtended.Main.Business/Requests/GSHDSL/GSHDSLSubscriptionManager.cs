using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class GSHDSLSubscriptionManager
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

        public string HasPendingRequestInWaitingList(string switchId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("Id");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StSwitchId", switchId);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStep.Id", "ED5E126A-3336-47E6-9138-2788FF96B78A");

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var id = entities[0].GetColumnValue("Id");
                return id.ToString();
            }
            return null;
        }

        public void PostGSHDSLRequestToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<GSHDSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<GSHDSLSubscriptionRequestInput>
                {

                    InputArguments = new GSHDSLSubscriptionRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            //ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<GSHDSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_GSHDSL_CreateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
