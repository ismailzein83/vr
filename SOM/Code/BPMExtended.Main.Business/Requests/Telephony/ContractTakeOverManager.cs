﻿using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ContractTakeOverManager
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
        public void PostContractTakeOverToOM(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyContractTakeOver");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");
            // esq.AddColumn("StContactId");
            //esq.AddColumn("StContact.Id");
            // esq.AddColumn("StAccount");
            // esq.AddColumn("StAccount.Id");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                //var contractId = entities[0].GetColumnValue("StContractId");
                // var contactId = entities[0].GetColumnValue("StContactId");
                // var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<ContractTakeOverRequestInput> somRequestInput = new SOMRequestInput<ContractTakeOverRequestInput>
                {

                    InputArguments = new ContractTakeOverRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            // ContractId = contractId.ToString(),
                            // ContactId = contactId.ToString(),
                            // AccountId = null,
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ContractTakeOverRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ContractTakeOver/StartProcess", somRequestInput);
                }

            }

        }

        #endregion
    }
}
