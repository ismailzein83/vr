﻿using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
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
            esq.AddColumn("StContractIdOnHold");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractIdOnHold");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
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
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Billing_SubmitActivateContract/StartProcess", somRequestInput);
                }


            }


        }

        public void ActivateLineSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StContractIdOnHold");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractIdOnHold");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
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


            }


        }

        #endregion
    }
}
