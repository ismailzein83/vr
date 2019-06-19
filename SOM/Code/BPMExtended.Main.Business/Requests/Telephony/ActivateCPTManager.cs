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
    public class ActivateCPTManager
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
        public void PostCptToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCpt");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerID");
            esq.AddColumn("StCptId");
            esq.AddColumn("StPhoneNumber");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var cptId = entities[0].GetColumnValue("StCptId");
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var customerId = entities[0].GetColumnValue("StCustomerID");

                SOMRequestInput<ActivateCptRequestInput> somRequestInput = new SOMRequestInput<ActivateCptRequestInput>
                {

                    InputArguments = new ActivateCptRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        DirectoryNumber= phoneNumber.ToString(),
                        CPTId= cptId.ToString()
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitCPT/StartProcess", somRequestInput);
                }

            }

        }

        public void ActivateCptRequest(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCpt");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerID");
            esq.AddColumn("StCptId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StCptNumber");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var cptId = entities[0].GetColumnValue("StCptId");
                var cptNumber = entities[0].GetColumnValue("StCptNumber");
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var customerId = entities[0].GetColumnValue("StCustomerID");

                SOMRequestInput<ActivateCptRequestInput> somRequestInput = new SOMRequestInput<ActivateCptRequestInput>
                {

                    InputArguments = new ActivateCptRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        CPTService ="",
                        CPTId = cptId.ToString(),
                        CPTNumber = cptNumber.ToString()
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivateCPT/StartProcess", somRequestInput);
                }

            }
        }

        public List<CPTItem> SearchCPT(string phoneNumber)
        {
            List<CPTItem> result = new List<CPTItem>();

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<List<CPTItem>>(String.Format("api/SOM.ST/Inventory/SearchCPT?Phonenumber={0}", phoneNumber));
            }
            return result;
        }

        public bool RegisterCPTNumber(string phoneNumber, string cptId)
        {
            bool output = false;

            using (SOMClient client = new SOMClient())
            {
                output = client.Get<bool>(String.Format("api/SOM.ST/Inventory/ReserveCPT?phoneNumber={0}&cptId={1}", phoneNumber, cptId));
            }
            return output;
        }

        #endregion
    }
}
