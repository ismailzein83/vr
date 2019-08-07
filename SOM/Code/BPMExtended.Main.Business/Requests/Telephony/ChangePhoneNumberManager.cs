using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ChangePhoneNumberManager
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

        #region Public
        public void CreateChangePhoneNumberSwitchTeamWorkOrder(string requestId)
        {
            string workOrderId = new CustomerRequestManager().CreateWorkOrder(requestId, "B6ECD46A-556A-404C-8C4E-E7B1645EB186");

            if (workOrderId != "")
            {
                //update technical step of the request
                var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StChangePhoneNumberRequest");
                var recordEntity = recordSchema.CreateEntity(UserConnection);

                var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "StChangePhoneNumberRequest");
                eSQ.RowCount = 1;
                eSQ.AddAllSchemaColumns();
                eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
                var collection = eSQ.GetEntityCollection(UserConnection);
                if (collection.Count > 0)
                {
                    recordEntity = collection[0];
                    recordEntity.SetColumnValue("StWorkOrderStageId", "4EE8DB9E-684E-4FB6-AE69-C04C41C4635B");
                    recordEntity.SetColumnValue("StWorkOrderID", workOrderId);
                    recordEntity.SetColumnValue("StIsWorkOrderCompleted", false);
                }
                recordEntity.Save();
            }


        }

        public void PostChangePhoneNumberToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            List<SaleService> feesToRemove = new List<SaleService>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StChangePhoneNumberRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StRatePlanId");
            esq.AddColumn("StNewRatePlanId");
            esq.AddColumn("StNewDirectoryNumber");
            esq.AddColumn("StOldSubTypeClass");
            esq.AddColumn("StIsDeclassify");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldDirectoryNumber = entities[0].GetColumnValue("StPhoneNumber");
                var oldRatePlanId = entities[0].GetColumnValue("StRatePlanId");
                var newRatePlanId = entities[0].GetColumnValue("StNewRatePlanId");
                var newDirectoryNumber = entities[0].GetColumnValue("StNewDirectoryNumber");
                string oldSubTypeClass = (string)entities[0].GetColumnValue("StOldSubTypeClass");
                bool isDeclassify = (bool)entities[0].GetColumnValue("StIsDeclassify");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //Get fees to remove if exist
                if (isDeclassify && oldSubTypeClass!= "Normal")
                {
                    feesToRemove = new CatalogManager().GetTelephonyPhoneCategoryFees(oldSubTypeClass);
                }


                SOMRequestInput<ChangePhoneNumberRequestInput> somRequestInput = new SOMRequestInput<ChangePhoneNumberRequestInput>
                {
                    InputArguments = new ChangePhoneNumberRequestInput
                    {
                        OldDirectoryNumber = oldDirectoryNumber.ToString(),
                        NewDirectoryNumber = newDirectoryNumber.ToString(),
                        OldRatePlanId = oldRatePlanId.ToString(),
                        NewRatePlanId = (newRatePlanId == null || newRatePlanId.ToString() == "") ? oldRatePlanId.ToString() : newRatePlanId.ToString(),
                        FeesToRemove = feesToRemove,
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
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ChangePhoneNumberRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitChangePhoneNumberOnSameSwitch/StartProcess", somRequestInput);
                }

            }

        }

        public void ActivateChangePhoneRequest(Guid requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            List<SaleService> feesToRemove = new List<SaleService>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StChangePhoneNumberRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StRatePlanId");
            esq.AddColumn("StNewRatePlanId");
            esq.AddColumn("StNewDirectoryNumber");
            esq.AddColumn("StOldSubTypeClass");
            esq.AddColumn("StIsDeclassify");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldDirectoryNumber = entities[0].GetColumnValue("StPhoneNumber");
                var oldRatePlanId = entities[0].GetColumnValue("StRatePlanId");
                var newRatePlanId = entities[0].GetColumnValue("StNewRatePlanId");
                var newDirectoryNumber = entities[0].GetColumnValue("StNewDirectoryNumber");
                string oldSubTypeClass = (string)entities[0].GetColumnValue("StOldSubTypeClass");
                bool isDeclassify = (bool)entities[0].GetColumnValue("StIsDeclassify");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //Get fees to remove if exist
                if (isDeclassify && oldSubTypeClass != "Normal")
                {
                    feesToRemove = new CatalogManager().GetTelephonyPhoneCategoryFees("AADF5555-146E-4FB3-BDEC-4477446BE12F");
                }


                SOMRequestInput<ChangePhoneNumberRequestInput> somRequestInput = new SOMRequestInput<ChangePhoneNumberRequestInput>
                {
                    InputArguments = new ChangePhoneNumberRequestInput
                    {
                        OldDirectoryNumber = oldDirectoryNumber.ToString(),
                        NewDirectoryNumber = newDirectoryNumber.ToString(),
                        OldRatePlanId = oldRatePlanId.ToString(),
                        NewRatePlanId = (newRatePlanId == null || newRatePlanId.ToString() == "") ? oldRatePlanId.ToString() : newRatePlanId.ToString(),
                        FeesToRemove = feesToRemove,
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
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ChangePhoneNumberRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivateChangePhoneNumberOnSameSwitch/StartProcess", somRequestInput);
                }

            }
        }
        #endregion
    }
}
