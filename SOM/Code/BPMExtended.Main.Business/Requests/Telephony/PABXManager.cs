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
    public class PABXManager
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

        public List<TelephonyContractInfo> GetValidPabxPhoneNumbers(string contractId , string customerId)
        {
            EntitySchemaQuery esq;
            EntityCollection entities;
            IEntitySchemaQueryFilterItem esqFirstFilter, esqFirstFilter2;
            List<TelephonyContractInfo> items = new List<TelephonyContractInfo>();
            List<TelephonyContractInfo> filteredItems = new List<TelephonyContractInfo>();
            var telephonyOperationsIds = new List<string>();
            
            //Get pabx Service Id from general settings catalog
            string pabxServiceId = new CatalogManager().GetPABXServiceId();

            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<TelephonyContractInfo>>(String.Format("api/SOM.ST/Billing/GetValidPabxPhoneNumbers?ContractId={0}&PABXServiceId={1}&CustomerId={2}", contractId, pabxServiceId,customerId));
            }

            if(items !=null)
            if(items.Count > 0)
            {

                //Get telephony operations ids Id from general settings catalog
                telephonyOperationsIds = new CatalogManager().GetTelephonyOperations();

                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
                esq.AddColumn("Id");
                esq.AddColumn("StContractID");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", items.Select(p=>p.Id).ToArray());
                esqFirstFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.NotEqual, "StRequestType", telephonyOperationsIds.ToArray());
                //TODO: filter on cancelled requests

                esq.Filters.Add(esqFirstFilter);
                esq.Filters.Add(esqFirstFilter2);

                entities = esq.GetEntityCollection(BPM_UserConnection);

                  var secondaryContracts = from r in entities.AsEnumerable()
                                           group r by new
                                            {
                                               groupByContractID = r.GetColumnValue("StContractID").ToString()
                                            }
                                            into g
                                            select new
                                              {
                                                contractId = g.Key.groupByContractID
                                              };
                if (entities.Count == 0)
                    return items;
                else
                {
                    foreach (var secondaryContractId in secondaryContracts)
                    {

                        filteredItems.Add(new TelephonyContractInfo()
                        {
                            Id = secondaryContractId.contractId,
                            PhoneNumber = items.Where(e => e.Id == secondaryContractId.contractId).Select(e => e.PhoneNumber).First()
                        });

                    }
                }
                    
            }

            return filteredItems;
        }


        public PabxContractDetail GetPabxContract(string contractId, string phoneNumber)
        {

            var item = new PabxContractDetail();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<PabxContractDetail>(String.Format("api/SOM.ST/Billing/GetPabxContract?ContractId={0}&PhoneNumber={1}", contractId,phoneNumber));
            }

            return item;
        }


        public ServiceParameter GetPabxServiceParameterValues()
        {
            var item = new ServiceParameter();
            string pabxServiceId = new CatalogManager().GetPABXServiceId();

            using (SOMClient client = new SOMClient())
            {
                item = client.Get<ServiceParameter>(String.Format("api/SOM.ST/Billing/GetPabxServiceParameterValues?ServiceId={0}", pabxServiceId));
            }

            return item;

        }

        public void CreatePabxSwitchTeamWorkOrder(string requestId)
        {
            string workOrderId = new CustomerRequestManager().CreateWorkOrder(requestId, "F60012C5-A0C3-4593-B7C4-B50A21988D54");

            if (workOrderId != "")
            {
                //update technical step of the request
                var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StPabx");
                var recordEntity = recordSchema.CreateEntity(UserConnection);

                var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "StPabx");
                eSQ.RowCount = 1;
                eSQ.AddAllSchemaColumns();
                eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
                var collection = eSQ.GetEntityCollection(UserConnection);
                if (collection.Count > 0)
                {
                    recordEntity = collection[0];
                    recordEntity.SetColumnValue("StTechnicalStepId", "9BDB150C-919D-4147-9F20-62BF7F40FB5E");
                    recordEntity.SetColumnValue("StWorkOrderID", workOrderId);
                    recordEntity.SetColumnValue("StIsWorkOrderCompleted", false);
                }
                recordEntity.Save();
            }


        }

        public void PostCreatePABXToOM(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPabx");
            esq.AddColumn("StContractID");
            esq.AddColumn("StPilotContractParameter");
            esq.AddColumn("StParameterNumber");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StSelectedSecondaryContracts");
            esq.AddColumn("StCustomerId");
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
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var parameterNumber = entities[0].GetColumnValue("StParameterNumber");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                ServiceParameterValue pilotParameter = JsonConvert.DeserializeObject<ServiceParameterValue>(entities[0].GetColumnValue("StPilotContractParameter").ToString()); ;
                List<PabxContractInput> selectedSecondaryContracts = JsonConvert.DeserializeObject<List<PabxContractInput>>(entities[0].GetColumnValue("StSelectedSecondaryContracts").ToString());

                SOMRequestInput<CreatePABXRequestInput> somRequestInput = new SOMRequestInput<CreatePABXRequestInput>
                {

                    InputArguments = new CreatePABXRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            RequestId = requestId.ToString(),
                            ContractId = contractId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        SubmitPabxInput = new SubmitPabxInput()
                        {
                            PilotContract = new PabxContractInput()
                            {
                                ContractId = contractId.ToString(),
                                PhoneNumber = phoneNumber.ToString(),
                                PabxParameterValue = pilotParameter

                            },
                            Contracts = selectedSecondaryContracts,
                            PabxService = new PabxService()
                            {
                                Id = new CatalogManager().GetPABXServiceId(),
                                ParameterNumber = parameterNumber.ToString(),
                                PackageId = "",
                                ParameterId = ""
                            }
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<CreatePABXRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitPabx/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void ActivateCreatePABXService(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPabx");
            esq.AddColumn("StContractID");
            esq.AddColumn("StPilotContractParameter");
            esq.AddColumn("StParameterNumber");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StSelectedSecondaryContracts");
            esq.AddColumn("StCustomerId");
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
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var parameterNumber = entities[0].GetColumnValue("StParameterNumber");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                ServiceParameterValue pilotParameter = JsonConvert.DeserializeObject<ServiceParameterValue>(entities[0].GetColumnValue("StPilotContractParameter").ToString()); ;
                List<PabxContractInput> selectedSecondaryContracts = JsonConvert.DeserializeObject<List<PabxContractInput>>(entities[0].GetColumnValue("StSelectedSecondaryContracts").ToString());


                SOMRequestInput<CreatePABXRequestInput> somRequestInput = new SOMRequestInput<CreatePABXRequestInput>
                {

                    InputArguments = new CreatePABXRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            RequestId = requestId.ToString(),
                            ContractId = contractId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        SubmitPabxInput = new SubmitPabxInput()
                        {
                            PilotContract = new PabxContractInput()
                            {
                                ContractId = contractId.ToString(),
                                PhoneNumber = phoneNumber.ToString(),
                                PabxParameterValue = pilotParameter

                            },
                            Contracts = selectedSecondaryContracts,
                            PabxService = new PabxService()
                            {
                                Id = new CatalogManager().GetPABXServiceId(),
                                ParameterNumber = parameterNumber.ToString(),
                                PackageId = "",
                                ParameterId = ""
                            }
                        }
                    }
                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<CreatePABXRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivatePABX/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public bool IsSecondaryPABXContract(string contractId)
        {
            bool isSecondaryPABXContract = false;
            using (SOMClient client = new SOMClient())
            {
                isSecondaryPABXContract = client.Get<bool>(String.Format("api/SOM.ST/Billing/IsSecondaryPABXContract?ContractId={0}", contractId));
            }
            return isSecondaryPABXContract;
        }
         
        #endregion


    }
}
