using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;


namespace BPMExtended.Main.Business
{
    public class FriendAndFamilyManager
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


        public bool CheckIfNumberExist(string phoneNumber)
        {
            bool result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Inventory/CheckIfNumberExist?phoneNumber={0}", phoneNumber));
            }

            return result;
        }

        public List<FAFNumbersOutput> GetFAFNumbers(string contractId)
        {
            List<FAFNumbersOutput> result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<List<FAFNumbersOutput>>(String.Format("api/SOM.ST/Billing/GetFAFNumbers?ContractId={0}", contractId));
            }

            return result;
        }

        public void PostFAFManagementToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StFriendAndFamily");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<FAFManagementRequestInput> somRequestInput = new SOMRequestInput<FAFManagementRequestInput>
                {

                    InputArguments = new FAFManagementRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            // ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<FAFManagementRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ManageFAF/StartProcess", somRequestInput);
                }

            }

        }

        #endregion
    }
}
