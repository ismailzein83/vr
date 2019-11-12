using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using System.Linq;

namespace BPMExtended.Main.Business
{ 
    public class LineSubscriptionManager
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
        public void PostLineSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }

        public void ActivateLineSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }


        }

        public void CreateSwitchAccount(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StContractID");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var contractId = entities[0].GetColumnValue("StContractID");

                SOMRequestInput<CreateSwitchAccountInput> somRequestInput = new SOMRequestInput<CreateSwitchAccountInput>
                {

                    InputArguments = new CreateSwitchAccountInput
                    {
                        RequestId = requestId.ToString(),
                        LinePathId = pathId.ToString(),
                        ContractId = contractId.ToString()
                    },
                    

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<CreateSwitchAccountInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateSwitchAccount/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void RecreateSwitchAccount(Guid requestId, string oldDevice, string newDevice)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StDeviceType");
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var deviceType = entities[0].GetColumnValue("StDeviceType");
                var contractId = entities[0].GetColumnValue("StContractID");

                SOMRequestInput<RecreateSwitchAccountInput> somRequestInput = new SOMRequestInput<RecreateSwitchAccountInput>
                {
                    InputArguments = new RecreateSwitchAccountInput
                    {
                        LinePathId = pathId.ToString(),
                        ContractId = contractId.ToString(),
                        NewDeviceId = newDevice,
                        OldDeviceId = oldDevice,
                        RequestId = requestId.ToString()
                    },
                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<RecreateSwitchAccountInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/RecreateSwitchAccount/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void SubmitActivateTelephonyContract(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StDeviceType");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var deviceType = entities[0].GetColumnValue("StDeviceType");
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var coreServices = entities[0].GetColumnValue("StCoreServices").ToString();
                var optionalServices = entities[0].GetColumnValue("StServices").ToString();

                List<ServiceItem> contractServices = new List<ServiceItem>();
                List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
                List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
                string serviceResourceId = "";

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);

                }


                SOMRequestInput<SubmitActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<SubmitActivateTelephonyContractInput>
                {
                    InputArguments = new SubmitActivateTelephonyContractInput
                    {
                        LinePathId = pathId.ToString(),
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        Services = contractServices
                    },
                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<SubmitActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateTelephonyContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void ProceedActivateTelephonyContract(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StDeviceType");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var deviceType = entities[0].GetColumnValue("StDeviceType");
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var coreServices = entities[0].GetColumnValue("StCoreServices").ToString();
                var optionalServices = entities[0].GetColumnValue("StServices").ToString();

                List<ServiceItem> contractServices = new List<ServiceItem>();
                List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
                List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
                string serviceResourceId = "";

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);

                }


                SOMRequestInput<SubmitActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<SubmitActivateTelephonyContractInput>
                {
                    InputArguments = new SubmitActivateTelephonyContractInput
                    {
                        LinePathId = pathId.ToString(),
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        Services = contractServices
                    },
                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<SubmitActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ProceedActivateTelephonyContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void FinalizeActivateTelephonyContract(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StDeviceType");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var deviceType = entities[0].GetColumnValue("StDeviceType");
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                
                SOMRequestInput<FinalizeActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<FinalizeActivateTelephonyContractInput>
                {
                    InputArguments = new FinalizeActivateTelephonyContractInput
                    {
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        }
                    },
                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<FinalizeActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeActivateTelephonyContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void CancelLineSubscriptionRequest(Guid requestId)
        {
            new CustomerRequestManager().SetRequestAsCancelled(requestId.ToString());
            //SOMRequestOutput output;
            //SOMRequestInput<CancelSuspensionRequestInput> somRequestInput = new SOMRequestInput<CancelSuspensionRequestInput>
            //{
            //    InputArguments = new CancelSuspensionRequestInput { RequestId = requestId.ToString() }
            //};
            //using (var client = new SOMClient())
            //{
            //    output = client.Post<SOMRequestInput<CancelSuspensionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CancellingContractSuspensionFromCustomerCare/StartProcess", somRequestInput);
            //}
            //var manager = new BusinessEntityManager();
            //manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
        }
        #endregion
        public ServiceItem ServiceDetailToContractServiceMapper(ServiceDetail item)
        {
            return new ServiceItem
            {
                Id = item.Id,
                PackageId = item.PackageId
            };
        }
    }
}
