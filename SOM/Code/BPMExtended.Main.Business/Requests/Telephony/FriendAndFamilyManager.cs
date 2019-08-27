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

        public FAFNumbersOutput GetFAFNumbers(string contractId)
        {
            FAFNumbersOutput result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<FAFNumbersOutput>(String.Format("api/SOM.ST/Billing/ReadFAFNumbers?ContractId={0}", contractId));
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
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StFAFGridData");
            esq.AddColumn("StFAFGroupId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var fafGroupId = entities[0].GetColumnValue("StFAFGroupId");
                string fafNumbers = entities[0].GetColumnValue("StFAFGridData").ToString();
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<FAFManagementRequestInput> somRequestInput = new SOMRequestInput<FAFManagementRequestInput>
                {

                    InputArguments = new FAFManagementRequestInput
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
                        PhoneNumber = phoneNumber.ToString(),
                        FAFGroupId= fafGroupId.ToString(),
                        FAFNumbers = JsonConvert.DeserializeObject<List<FAFNumber>>(fafNumbers)
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<FAFManagementRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ManageFAF/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        #endregion
    }
}
