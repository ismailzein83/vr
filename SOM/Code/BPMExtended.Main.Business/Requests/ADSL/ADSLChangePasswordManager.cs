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
    public class ADSLChangePasswordManager
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
        public void PostADSLChangePasswordToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLChangePassword");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");              
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathdId = entities[0].GetColumnValue("StLinePathId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLChangePasswordRequestInput> somRequestInput = new SOMRequestInput<ADSLChangePasswordRequestInput>
                {

                    InputArguments = new ADSLChangePasswordRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = feesServices,
                            IsPaid = (bool)isPaid
                        },
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLChangePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitADSLResetPassword/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void FinalizeADSLResetPassword(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLChangePassword");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathdId = entities[0].GetColumnValue("StLinePathId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLChangePasswordRequestInput> somRequestInput = new SOMRequestInput<ADSLChangePasswordRequestInput>
                {

                    InputArguments = new ADSLChangePasswordRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = feesServices,
                            IsPaid = (bool)isPaid
                        },
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLChangePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeADSLResetPassword/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        #endregion
    }
}
