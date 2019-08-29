﻿using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ContractManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public TelephonyContractEntity GetTelephonyContract(string contractId)
        {

            var telephonyContractEntity = new TelephonyContractEntity();
            using (SOMClient client = new SOMClient())
            {
                var item = client.Get<CustomerContract>(String.Format("api/SOM.ST/Billing/GetTelephonyContract?ContractId={0}", contractId));
                telephonyContractEntity = CustomerContractToEntity(item);
            }
            return telephonyContractEntity;
        }

        public ContractInfo GetContractInfo(string contractId)
        {

            var contractInfo = new ContractInfo();
            using (SOMClient client = new SOMClient())
            {
                //TODO: ask rodi to return the minimal information in the below call
                var item = client.Get<CustomerContract>(String.Format("api/SOM.ST/Billing/GetTelephonyContract?ContractId={0}", contractId));
                contractInfo = CustomerContractToContractInfo(item);
            }
            return contractInfo;
        }

        public TelephonyContractInfo GetTelephonyContractInfo(string contractId)
        {

            var contractInfo = new TelephonyContractInfo();
            using (SOMClient client = new SOMClient())
            {
                //TODO: ask rodi to return the minimal information in the below call
                var item = client.Get<CustomerContract>(String.Format("api/SOM.ST/Billing/GetTelephonyContract?ContractId={0}", contractId));
                contractInfo = CustomerContractToTelephonyContractInfo(item);
            }
            return contractInfo;
        }

        public List<TelephonyContractDetail> GetTelephonyContracts(string customerId) // MYA: Uncomment and handle object client side
        {
            var telephonyContractDetails = new List<TelephonyContractDetail>();
            using (SOMClient client = new SOMClient())
            {
               // var items = client.Get<List<TelephonyContract>>(String.Format("api/SOM.ST/Billing/GetCustomerTelephonyContracts?CustomerId={0}", customerId));
               var items = client.Get<List<CustomerContract>>(String.Format("api/SOM.ST/Billing/GetCustomerTelephonyContracts?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                   // var telephonyContractDetailItem = TelephonyContractToDetail(item);
                    var telephonyContractDetailItem = CustomerContractToDetail(item);
                    telephonyContractDetails.Add(telephonyContractDetailItem);
                }
            }
            return telephonyContractDetails;

        }

        public List<ContractDetail> GetxDSLContracts(string customerId) // MYA: Uncomment and handle object client side
        {
            var xDSLontractDetails = new List<ContractDetail>();
            using (SOMClient client = new SOMClient())
            {
                var items = client.Get<List<CustomerContract>>(String.Format("api/SOM.ST/Billing/GetCustomerDSLContracts?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    // var telephonyContractDetailItem = TelephonyContractToDetail(item);
                    var xDSLContractDetailItem = xDSLCustomerContractToDetail(item);
                    xDSLontractDetails.Add(xDSLContractDetailItem);
                }
            }
            return xDSLontractDetails;

        }

       public void UpdateContractId(string requestId, string contractId)
        {
            string entityName = new CRMCustomerManager().GetEntityNameByRequestId(requestId);
            var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName(entityName);
            var recordEntity = recordSchema.CreateEntity(UserConnection);

            var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, entityName);
            eSQ.RowCount = 1;
            eSQ.AddAllSchemaColumns();
            eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
            var collection = eSQ.GetEntityCollection(UserConnection);
            if (collection.Count > 0)
            {
                recordEntity = collection[0];
                recordEntity.SetColumnValue("StContractID", contractId);
            }
            recordEntity.Save();
        }

        public List<BPInstance> GetFulfilmentLogs()
        {
            List<BPInstance> logs;

            using (SOMClient client = new SOMClient())
            {
                logs = client.Get<List<BPInstance>>(String.Format("api/SOM.ST/Billing/GetBPInstances"));
            }
            return logs;
        }

        public List<TelephonyContractDetail> GetTelephonyContractsByNumber(string phoneNumber)
        {
            return RatePlanMockDataGenerator.GetTelephonyContractsByNumber(phoneNumber);
        }
        //public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId, string pilotContractId)
        //{
        //    List<TelephonyContractDetail> contracts = RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
        //    List<PabxContractDetail> pabxContracts = RatePlanMockDataGenerator.GetPabxContracts(customerId);
        //    InventoryManager manager = new InventoryManager();

        //    Func<TelephonyContractDetail, bool> filterExpression = (item) =>
        //    {
        //        if (pabxContracts.Find(x => x.ContractId == item.ContractId) != null)
        //            return false;

        //        //TODO: AYman to implement if not on same switch
        //        if (!manager.IsNumbersOnSameSwitch(item.ContractId, pilotContractId)) return false;

        //        return true;
        //    };

        //    return contracts.MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        //}

        public GSHDSLContractDetail GetGSHDSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContract(contractId);
        }

        public List<GSHDSLContractDetail> GetGSHDSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContracts(customerId);
        }

        public ContractEntity GetChildADSLContractByTelephonyContract(string telephonyContractId)
        {
            ContractEntity adslContract;

            using (SOMClient client = new SOMClient())
            {
                adslContract = client.Get<ContractEntity>(String.Format("api/SOM.ST/Billing/GetChildADSLContractRatePlanId?ContractId={0}", telephonyContractId));
            }
            return adslContract;

        }

        public ADSLContractDetail GetADSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetADSLContract(contractId);
        }

        public List<ADSLContractDetail> GetADSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetADSLContracts(customerId);
        }

        public List<ADSLContractDetail> GetADSLContractsByUsername(string userName)
        {
            return RatePlanMockDataGenerator.GetADSLContractsByUsername(userName);
        }
        public bool IsTelephonyContractHasRelatedADSL(string contractId)
        {
            bool hasRelatedADSL;

            using (SOMClient client = new SOMClient())
            {
                hasRelatedADSL = client.Get<bool>(String.Format("api/SOM.ST/Billing/IsTelephonyContractHasRelatedADSL?ContractId={0}", contractId));
            }
            return hasRelatedADSL;
        }

        public GetContractAddressOutput GetContractAddressAndDirectoryInfo(string contractId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;    
            GetContractAddressOutput item = new GetContractAddressOutput();
            string serviceId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StPublicDIId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "E7CF42F9-7A83-4AD2-A73A-5203C94A4DA2");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                serviceId = entities[0].GetColumnValue("StPublicDIId").ToString();
            }


            using (var client = new SOMClient())
            {
                item = client.Get<GetContractAddressOutput>(String.Format("api/SOM.ST/Billing/GetContractAddressAndDirectoryInfo?ContractId={0}&ServiceId={1}", contractId, serviceId));

            }

            return item;

        }

        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId)
        {

            return RatePlanMockDataGenerator.GetTelephonyContractsInfo(customerId);
        }

        public List<TelephonyContractInfo> GetMoveADSLTelephonyContractsInfo(string customerId , string phoneNumber)
        {

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {

                if (item.PhoneNumber == phoneNumber)
                    return false;

                return true;
            };


            return RatePlanMockDataGenerator.GetTelephonyContracts(customerId).MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        }

        public List<TelephonyContractInfo> GetNewPabxTelephonyContractsInfo(string customerId, string pilotContractId)
        {
            InventoryManager manager = new InventoryManager();

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {

                if (!manager.IsNumbersOnSameSwitch(item.ContractId, pilotContractId)) return false;

                return true;
            };

            return RatePlanMockDataGenerator.GetTelephonyContracts(customerId).MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();


        }

        public LeasedLineContractDetail GetLeasedLineContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContract(contractId);
        }

        public List<LeasedLineContractDetail> GetLeasedLineContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContracts(customerId);
        }
        public List<LeasedLineContractDetail> GetLeasedLineContractsByNumber(string phoneNumber)
        {
            return RatePlanMockDataGenerator.GetLeasedLineContractsByNumber(phoneNumber);
        }
        public string CountOfActiveContracts(string customerId , string contractId)
        {
            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6? "1" : "4";
        }

        public bool ContractTakeOver(string customerId, string contractId , string targetCustomerId , bool isAdslTakenOver , string username, string pass)
        {
            //TODO: Activate the operation


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.ContractTakeOver;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }


            return true;
        }

        public bool AddADSLISPService(string contractId)
        {
            //
            return true;
        }

        public bool ActivateADSLISPService(string contractId, string customerId, string ispId, string port)
        {
            //TODO: Activate service
            return true;
        }


        public bool ActivateLeasedLineTermination(string contractId, string customerId)
        {
            //TODO: terminate the leased line contract of the customer


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.LeasedLineTermination;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }


            return true;
        }

        public List<FriendAndFamilyDetail> GetFriendAndFamilyNumbers(string contractId)
        {
            return RatePlanMockDataGenerator.GetFriendAndFamilyNumbers(contractId);
        }

        public bool ActivateADSLLineMoving(string telephonyContractId, string customerId, string port)
        {
            //TODO: Activate service


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.ADSLLineMoving;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }


        public bool ActivateADSLLineTermination(string contractId , string reason , string phoneNumber)
        {
            //TODO: Activate service


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.ADSLLineTermination;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }


        public bool ActivateADSLAlterSpeed(string contractId, string speed)
        {
            //TODO: Activate service

            //TODO:Get taxes
            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.ADSLAlterSpeed;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }



            return true;
        }

        public bool ActivateGSHDSL(string contractId)
        {
            //TODO: Activate service

            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.GSHDSLSubscription;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");
                                                                       
            }


            return true;
        }

        public bool ActivateGSHDSLTermination(string contractId)
        {
            //TODO: Terminate GSHDSL contract
            return true;
        }

        public bool ActivateFAF(string contractId , string Percentage , string data)
        {
            //TODO: Terminate GSHDSL contract
            return true;
        }

        public bool ActivateLineTermination(string contractId)
        {
            //TODO: Terminate GSHDSL contract


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.TelephonyLineTermination;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }


        public bool ActivateLineMovingSameSwitch(string contractId)
        {
            //TODO: Terminate GSHDSL contract


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.TelephonyLineMovingSameSwitch;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }

        public bool ActivateChangePhoneNumberRequest(string contractId)
        {
            //TODO: Terminate GSHDSL contract


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.TelephonyChangePhoneNumber;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }

        public bool ActivateLineMovingNewSwitch(string contractId)
        {
            //TODO: Terminate GSHDSL contract


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.TelephonyLineMovingNewSwitch;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }

        public bool ActivateDeportedNumber(string contractId)
        {
            //TODO: Terminate GSHDSL contract


            //TODO:Get taxes

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationOCC");

            esq.AddColumn("StServiceCode");
            esq.AddColumn("Id");

            int requestType = (int)OperationType.DeportedNumber;

            // Query data filtering.
            var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", requestType.ToString());
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var serviceCode = entities[0].GetColumnValue("StServiceCode");

            }

            return true;
        }

        public bool ChangeADSLPassword(string contractId, string newPassword)
        {
            //TODO: 
            return true;
        }


        public bool ActivateAdditionServices(string contractId, string services)
        {
            //TODO: Activate Addition Services
            return true;
        }


        public bool DeactivateLineMovingContract(string contractId , string requestId)
        {
            //TODO: Deactivate line moving contract on the network
            //TODO: if line deactivated => change step id to technical step
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StLineMovingNewSwitch").Set("StStepId", Column.Parameter("404A2E98-D3B8-4A7A-813F-CE4594CF580C"))
                .Where("Id").IsEqual(Column.Parameter(new Guid(requestId)));
            update.Execute();

            return true;
        }

       

        public bool DeactivateLineTerminationRequest(string contractId, string requestId)
        {
            //TODO: Deactivate line  contract on the network
            //TODO: if line deactivated => change step id to technical step
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StLineTerminationRequest").Set("StStepId", Column.Parameter("45363147-CC59-4632-B09E-EB850D2FD25F"))
                .Where("Id").IsEqual(Column.Parameter(new Guid(requestId)));
            update.Execute();

            return true;
        }

        public string GetContractStatusByEnumValue(string enumValue)
        {
            string contractStatusDescription = null;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StContractStatus");

            esq.AddColumn("StEnumValue");
            esq.AddColumn("Description");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StEnumValue", enumValue);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contractStatusDescription = entities[0].GetColumnValue("Description").ToString();
            }

            return contractStatusDescription;
        }


        #region mappers

        //TODO: to be remove later
        private TelephonyContractInfo TelephonyContractDetailToInfo(TelephonyContractDetail detail)
        {
            return null;
            //return new TelephonyContractInfo
            //{
            //    ContractId = detail.ContractId,
            //    CustomerId = detail.CustomerId,
            //    Address = detail.Address,
            //    PhoneNumber = detail.PhoneNumber
            //};
        }

        private TelephonyContractEntity CustomerContractToEntity(CustomerContract contract)
        {
            int stat = 0;
            int.TryParse(contract.Status.ToString(), out stat);

            var catalogManager = new CatalogManager();
            string ratePlanName = catalogManager.GetRatePlanNameFromCatalog(contract.RateplanId);

            return new TelephonyContractEntity
            {
                ContractId = contract.Id,
                ContractAddress = contract.ContractAddress,
                ContractBalance = contract.CurrentBalance,
                RatePlanId = contract.RateplanId,
                RatePlanName = ratePlanName,
                PathId = contract.LinePathId,
                PhoneNumber = contract.PhoneNumber,
                ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)contract.Status).LookupId,
                IsBlocked = contract.IsBlocked,
                StatusChangeDate = contract.LastStatusChangeDate == null || contract.LastStatusChangeDate == DateTime.MinValue ? "" : contract.LastStatusChangeDate.ToString("dd/MM/yyyy hh:mm")
            };
        }

        private TelephonyContractDetail CustomerContractToDetail(CustomerContract contract)
        {
            int stat = 0;
            int.TryParse(contract.Status.ToString(), out stat);

            return new TelephonyContractDetail
            {
                ContractId = contract.Id,
                Status = GetContractStatusByEnumValue(contract.Status.ToString()), //Check if API and Our conventions are the same
                PhoneNumber = contract.PhoneNumber,
                ActivationDate = contract.ActivationDate,
                //ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)contract.Status).LookupId
                //ContractStatusId = GetContractStatusByEnumValue(contract.Status.ToString())
            };
        }

        private ContractDetail xDSLCustomerContractToDetail(CustomerContract contract)
        {
            int stat = 0;
            int.TryParse(contract.Status.ToString(), out stat);

            return new ContractDetail
            {
                ContractId = contract.Id,
                Status = GetContractStatusByEnumValue(contract.Status.ToString()), //Check if API and Our conventions are the same
                ActivationDate = contract.ActivationDate,
                //ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)contract.Status).LookupId
                //ContractStatusId = GetContractStatusByEnumValue(contract.Status.ToString())
            };
        }

        private TelephonyContractDetail TelephonyContractToDetail(TelephonyContract telephonyContract)
        {
            return new TelephonyContractDetail()
            {
                ContractId = telephonyContract.Id,
                RatePlanName = telephonyContract.RateplanName,
                PhoneNumber = telephonyContract.PhoneNumber,
                ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)telephonyContract.Status).LookupId,
                ActivationDate = telephonyContract.ActivationDate,
                StatusDate = telephonyContract.LastStatusChangeDate
            };
        } 
        private ContractInfo CustomerContractToContractInfo(CustomerContract contract)
        {
            return new ContractInfo()
            {
                ContractId = contract.Id,
                RatePlanId = contract.RateplanId
            };
        }

        private TelephonyContractInfo CustomerContractToTelephonyContractInfo(CustomerContract contract)
        {
            return new TelephonyContractInfo()
            {
                ContractId = contract.Id,
                RatePlanId = contract.RateplanId,
                PhoneNumber = contract.PhoneNumber
            };
        }

        #endregion

    }
}
