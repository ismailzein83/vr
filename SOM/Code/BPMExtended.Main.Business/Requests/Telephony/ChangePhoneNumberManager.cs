using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ChangePhoneNumberManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public void PostChangePhoneNumberToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            List<ServiceData> feesToRemove = new List<ServiceData>();

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
                        NewRatePlanId = newRatePlanId.ToString(),
                        FeesToRemove = feesToRemove,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ChangePhoneNumberRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ChangePhoneNumberOnSameSwitch/StartProcess", somRequestInput);
                }

            }

        }

        public void ActivateChangePhoneRequest(Guid requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            List<ServiceData> feesToRemove = new List<ServiceData>();

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

                //Get fees to remove if exist
                if (isDeclassify && oldSubTypeClass != "Normal")
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
                        NewRatePlanId = newRatePlanId.ToString(),
                        FeesToRemove = feesToRemove,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ChangePhoneNumberRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitChangePhoneNumberOnSameSwitch/StartProcess", somRequestInput);
                }

            }
        }

    }
}
