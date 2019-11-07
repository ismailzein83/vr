 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
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
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            var newCustomerContact =  esq.AddColumn("StTargetContact.StCustomerId");
            var newCustomerAccount = esq.AddColumn("StTargetAccount.StCustomerId");
            esq.AddColumn("StNewUserName");
            esq.AddColumn("StPassword");
            esq.AddColumn("StOldUserName");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StADSLContractId");
            esq.AddColumn("StDivisionName");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var newCustomerContactId = entities[0].GetTypedColumnValue<string>(newCustomerContact.Name);
                var newCustomerIdAccount = entities[0].GetTypedColumnValue<string>(newCustomerAccount.Name);
                var newUserName = entities[0].GetColumnValue("StNewUserName");
                var divisionName = entities[0].GetColumnValue("StDivisionName");
                var password = entities[0].GetColumnValue("StPassword");
                var adslContractId = entities[0].GetColumnValue("StADSLContractId");
                var linePathId = entities[0].GetColumnValue("StLinePathId");
                var oldUserName = entities[0].GetColumnValue("StOldUserName");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var contactId = entities[0].GetColumnValue("StContactId");

                CRMCustomerInfo info = contactId != null ? new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null) : new CRMCustomerManager().GetCRMCustomerInfo(null, accountId.ToString());

                SOMRequestInput<ContractTakeOverRequestInput> somRequestInput = new SOMRequestInput<ContractTakeOverRequestInput>
                {

                    InputArguments = new ContractTakeOverRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            RequestId = requestId.ToString(),
                            ContractId = contractId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        NewUserName = newUserName.ToString(),
                        NewPassword = password.ToString(),
                        OldUserName= oldUserName.ToString(),
                        NewCustomerId = newCustomerContactId !=null && newCustomerContactId != "" ? newCustomerContactId.ToString() : newCustomerIdAccount.ToString(),
                        LinePathId = linePathId.ToString(),
                        ADSLContractId = adslContractId.ToString(),
                        CSO = info.csoId
                        //SubType = divisionName.ToString()
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ContractTakeOverRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/TelephonySubmitContractTakeOver/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public bool hasADSLContract(string id)
        {
            bool hasAnADSL = false;
            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyContractTakeOver");
            esq.AddColumn("Id");
            esq.AddColumn("StHasAnADSL");

            var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", id);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                hasAnADSL = (bool)entities[0].GetColumnValue("StHasAnADSL");
            }

            return hasAnADSL;
        }

        public bool checkBillOnDemand(string requestId)//to be removed
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyContractTakeOver");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var contractId = entities[0].GetColumnValue("StContractID");

                //get number of contracts
                List<TelephonyContractDetail> telephonyContracts = new ContractManager().GetTelephonyContracts(customerId.ToString());
                //telephonyContracts.RemoveAll(item => item.ContractId == contractId.ToString());
                //return telephonyContracts.Count <= 1;
                if (telephonyContracts != null)
                {
                    if (telephonyContracts.Count == 0)
                        return true;

                    IEnumerable<TelephonyContractDetail> v = telephonyContracts.FindAllRecords<TelephonyContractDetail>(x => x.Status == "Active");
                    return v.Count() <= 1;
                }

                //check if account type is official or operation is advantageous then skip BOD

            }


            return false;
        }

        public string GetADSLDivisionName(string contractId, string requestId)
        {
            string divsionName =null;

            ContractEntity adslContract = new ContractManager().GetChildADSLContractByTelephonyContract(contractId);

            if (adslContract != null)
            {
                divsionName = new CatalogManager().GetDivisionByRatePlanId(adslContract.RatePlanId);

                //update adsl contract Id
                UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];

                var update = new Update(connection, "StTelephonyContractTakeOver").Set("StADSLContractId", Column.Parameter(adslContract.ContractId))
                           .Where("Id").IsEqual(Column.Parameter(requestId));

                update.Execute();

            }

            return divsionName;

            //Array values = Enum.GetValues(typeof(ADSLSubType));
            //Random random = new Random();
            //return values.GetValue(random.Next(values.Length)).ToString();
        }

        #endregion
    }
}
