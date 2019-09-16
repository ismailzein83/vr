using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLTakeOverManager
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

        public List<Contact> GetFilteredActiveContacts(string customerId)
        {
            //List<Contact> activeContacts = new CommonManager().GetActiveContacts().Where(item =>item.CustomerId != customerId).ToList();
            List<Contact> validContacts = new CommonManager().GetValidContacts();
            //List<Contact> filteredContacts = activeContacts.Where(b => validContacts.Any(a => a.CustomerId == b.CustomerId)).ToList();
            return validContacts;//return filteredContacts;
        }

        public void PostADSLTakeOverToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLContractTakeOver");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StUserName");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StPassword");
            esq.AddColumn("StSelectedCustomerId");
            esq.AddColumn("StNewLinePathId");
            esq.AddColumn("StNewTelephonyContractId");
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
                var userName = entities[0].GetColumnValue("StUserName");
                var password = entities[0].GetColumnValue("StPassword");
                var newCustomerId = entities[0].GetColumnValue("StSelectedCustomerId");
                var pathId = entities[0].GetColumnValue("StNewLinePathId");
                var newTelephonyContractId = entities[0].GetColumnValue("StNewTelephonyContractId");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                SOMRequestInput<ADSLTakeOverInput> somRequestInput = new SOMRequestInput<ADSLTakeOverInput>
                {

                    InputArguments = new ADSLTakeOverInput
                    {
                        NewUserName = userName.ToString(),
                        NewCustomerId = newCustomerId.ToString(),
                        LinePathId = pathId.ToString(),
                        NewPassword = password.ToString(),
                        CSO = new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null).csoBSCSId,
                        NewTelephonyContractId = newTelephonyContractId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            //AccountId = null,
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLTakeOverInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/XDSLSubmitContractTakeOver/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FinalizeADSLContractTakeOver(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLContractTakeOver");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StUserName");
            esq.AddColumn("StPassword");
            esq.AddColumn("StSelectedCustomerId");
            esq.AddColumn("StNewLinePathId");
            esq.AddColumn("StNewTelephonyContractId");
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
                var userName = entities[0].GetColumnValue("StUserName");
                var password = entities[0].GetColumnValue("StPassword");
                var newCustomerId = entities[0].GetColumnValue("StSelectedCustomerId");
                var pathId = entities[0].GetColumnValue("StNewLinePathId");
                var newTelephonyContractId = entities[0].GetColumnValue("StNewTelephonyContractId");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                SOMRequestInput<ADSLTakeOverInput> somRequestInput = new SOMRequestInput<ADSLTakeOverInput>
                {

                    InputArguments = new ADSLTakeOverInput
                    {
                        NewCustomerId = newCustomerId.ToString(),
                        LinePathId = pathId.ToString(),
                        CSO = new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null).csoBSCSId,
                        NewTelephonyContractId = newTelephonyContractId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            //AccountId = null,
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLTakeOverInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/XDSLFinalizeContractTakeOver/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
    }
}
