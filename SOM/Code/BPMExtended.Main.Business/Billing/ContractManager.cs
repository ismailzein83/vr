using BPMExtended.Main.Common;
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
        public List<TelephonyContractDetail> GetTelephonyContractsByNumber(string phoneNumber)
        {
            return RatePlanMockDataGenerator.GetTelephonyContractsByNumber(phoneNumber);
        }
        public List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId, string pilotContractId)
        {
            List<TelephonyContractDetail> contracts = RatePlanMockDataGenerator.GetTelephonyContracts(customerId);
            List<PabxContractDetail> pabxContracts = RatePlanMockDataGenerator.GetPabxContracts(customerId);
            InventoryManager manager = new InventoryManager();

            Func<TelephonyContractDetail, bool> filterExpression = (item) =>
            {
                if (pabxContracts.Find(x => x.ContractId == item.ContractId) != null)
                    return false;

                //TODO: AYman to implement if not on same switch
                if (!manager.IsNumbersOnSameSwitch(item.ContractId, pilotContractId)) return false;

                return true;
            };

            return contracts.MapRecords(TelephonyContractDetailToInfo, filterExpression).ToList();
        }

        public GSHDSLContractDetail GetGSHDSLContract(string contractId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContract(contractId);
        }

        public List<GSHDSLContractDetail> GetGSHDSLContracts(string customerId)
        {
            return RatePlanMockDataGenerator.GetGSHDSLContracts(customerId);
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
        public bool IsTelephonyLineHasADSLContract(string contractId)
        {
            //TODO: check if Telephony Line  mapped to an ADSL contract

            //return random boolean value
            ContractManager contractManager = new ContractManager();
            //TelephonyContractDetail contract = contractManager.GetTelephonyContract(contractId);

            ADSLLinePath item = new ADSLLinePath();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<ADSLLinePath>(String.Format("api/SOM/Inventory/CheckADSL?phoneNumber={0}", "1111"));
            }
            string[] path = item.Path.Split(',');
            return path[6] == "1";
            //Random gen = new Random();
            //int prob = gen.Next(100);
            //return prob <= 50;
        }

        public GetContractAddressOutput GetContractAddressAndDirectoryInfo(string contractId)
        {
            GetContractAddressOutput item = new GetContractAddressOutput();
            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            string serviceId = packages.SNPCode;

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

            var contractAddress = new Address()
            {
                Street = contract.Street,
                City = contract.City
            };
            return new TelephonyContractEntity
            {
                ContractId = contract.Id,
                ContractAddress = contractAddress,
                RatePlanId = contract.RateplanId,
                PathId = contract.LinePathId,
                PhoneNumber = contract.PhoneNumber,
                ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)contract.Status).LookupId
            };
        }

        private TelephonyContractDetail CustomerContractToDetail(CustomerContract contract)
        {
            int stat = 0;
            int.TryParse(contract.Status.ToString(), out stat);

            var contractAddress = new Address()
            {
                Street = contract.Street,
                City = contract.City
            };
            return new TelephonyContractDetail
            {
                ContractId = contract.Id,
                Status = stat.ToString(), //Check if API and Our conventions are the same
                PhoneNumber = contract.PhoneNumber,
                ActivationDate = contract.ActivationDate,
                ContractStatusId = Utilities.GetEnumAttribute<ContractStatus, LookupIdAttribute>((ContractStatus)contract.Status).LookupId
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
