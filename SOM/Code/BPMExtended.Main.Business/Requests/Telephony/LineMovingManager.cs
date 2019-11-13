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
    public class LineMovingManager
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

        public List<ServiceData> GetNotApplicableServicesOnSwitch(string switchId, string contractId)
        {
            List<ServiceData> items = new List<ServiceData>();
            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<ServiceData>>(String.Format("api/SOM.ST/Billing/GetNotApplicableServicesOnSwitch?switchId={0}&contractId={1}", switchId, contractId));
            }
            return items;
        }

        public void SwitchDeleteSubscriptionLineMoving(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StIsNewSwitch");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");
            esq.AddColumn("StAddressNotes");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");
                var newSwitch = entities[0].GetColumnValue("StIsNewSwitch");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var street = entities[0].GetColumnValue("StStreet");
                var notes = entities[0].GetColumnValue("StAddressNotes");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloor");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var location = entities[0].GetColumnValue("StLocationName");
                var town = entities[0].GetColumnValue("StTownName");
                var province = entities[0].GetColumnValue("StProvinceName");



                SOMRequestInput<LineMovingInput> somRequestInput = new SOMRequestInput<LineMovingInput>
                {

                    InputArguments = new LineMovingInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        OldLinePathId = oldLinePathId.ToString(),
                        NewLinePathId = newLinePathID.ToString(),
                        SameSwitch = !((bool)newSwitch),
                        Address = new Address
                        {
                            Sequence = new ContractManager().GetContractAddressAndDirectoryInfo(contractId.ToString()).Address.Sequence.ToString(),//new CRMCustomerManager().GetCustomerAddress(customerId.ToString()).Sequence.ToString(),
                            StateProvince = province.ToString(),
                            City = city.ToString(),
                            Town = town.ToString(),
                            Region = area.ToString(),
                            Street = street.ToString(),
                            Building = building.ToString(),
                            Floor = floor.ToString(),
                            Notes = notes.ToString(),
                            LocationType = location.ToString()

                        },
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
                    output = client.Post<SOMRequestInput<LineMovingInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitTelephonyLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void HandelADSLLineMovingNewSwitch(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StIsNewSwitch");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");

                SOMRequestInput<LineMovingInput> somRequestInput = new SOMRequestInput<LineMovingInput>
                {

                    InputArguments = new LineMovingInput
                    {
                        
                        OldLinePathId = oldLinePathId.ToString(),
                        NewLinePathId = newLinePathID.ToString(),
                        RequestId = requestId.ToString(),
                        ContractId = contractId.ToString(),
                    },
                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeTelephonyDeleteSubscription/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void SwitchCreateSubscriptionLineMoving(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StIsNewSwitch");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");

                SOMRequestInput<LineMovingInput> somRequestInput = new SOMRequestInput<LineMovingInput>
                {

                    InputArguments = new LineMovingInput
                    {
                        OldLinePathId = oldLinePathId.ToString(),
                        NewLinePathId = newLinePathID.ToString(),
                        RequestId = requestId.ToString(),
                        ContractId = contractId.ToString()
                    },
                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitTelephonyCreateSubscription/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void CreateSubscriptionHandleADSL(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StIsNewSwitch");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");

                SOMRequestInput<LineMovingInput> somRequestInput = new SOMRequestInput<LineMovingInput>
                {

                    InputArguments = new LineMovingInput
                    {
                        OldLinePathId = oldLinePathId.ToString(),
                        NewLinePathId = newLinePathID.ToString(),
                        RequestId = requestId.ToString(),
                        ContractId = contractId.ToString()
                    },
                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeTelephonyCreateSubscription/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FullfilmentLineMove(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StNewPhoneNumber");
            esq.AddColumn("StIsNewSwitch");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");
            esq.AddColumn("StAddressNotes");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StNewADSLLinePathId");
            esq.AddColumn("StFreeReservationSwitchID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldPhoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var newPhoneNumber = entities[0].GetColumnValue("StNewPhoneNumber");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                bool isNewSwitch = (bool)entities[0].GetColumnValue("StIsNewSwitch");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                var street = entities[0].GetColumnValue("StStreet");
                var notes = entities[0].GetColumnValue("StAddressNotes");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloor");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var location = entities[0].GetColumnValue("StLocationName");
                var town = entities[0].GetColumnValue("StTownName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");
                var newADSLLinePathId = entities[0].GetColumnValue("StNewADSLLinePathId");
                var freeReservationSwitchID = entities[0].GetColumnValue("StFreeReservationSwitchID");


                SOMRequestInput<LineMovingSubmitNewSwitchInput> somRequestInput = new SOMRequestInput<LineMovingSubmitNewSwitchInput>
                {

                    InputArguments = new LineMovingSubmitNewSwitchInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        OldPhoneNumber = oldPhoneNumber.ToString(),
                        NewPhoneNumber = newPhoneNumber.ToString(),
                       // OldTelLinePathId = oldLinePathId.ToString(),
                        NewTelLinePathId = newLinePathID.ToString(),
                        NewADSLLinePathId = newADSLLinePathId.ToString(),
                        NotApplicableServices = GetNotApplicableServicesOnSwitch(freeReservationSwitchID.ToString(),contractId.ToString()),
                        Address = new Address
                        {
                            Sequence = new ContractManager().GetContractAddressAndDirectoryInfo(contractId.ToString()).Address.Sequence.ToString(),
                            //new CRMCustomerManager().GetCustomerAddress(customerId.ToString()).Sequence.ToString(),
                            StateProvince = province.ToString(),
                            City = city.ToString(),
                            Town = town.ToString(),
                            Region = area.ToString(),
                            Street = street.ToString(),
                            Building = building.ToString(),
                            Floor = floor.ToString(),
                            Notes = notes.ToString(),
                            LocationType = location.ToString()

                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingSubmitNewSwitchInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/BeginFinalizeDiffSwitchTelephonyLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void ProceedFinalizeDiffSwitchTelephonyLineMove(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StNewPhoneNumber");
            esq.AddColumn("StIsNewSwitch");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");
            esq.AddColumn("StAddressNotes");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StNewADSLLinePathId");
            esq.AddColumn("StFreeReservationSwitchID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldPhoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var newPhoneNumber = entities[0].GetColumnValue("StNewPhoneNumber");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                bool isNewSwitch = (bool)entities[0].GetColumnValue("StIsNewSwitch");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                var street = entities[0].GetColumnValue("StStreet");
                var notes = entities[0].GetColumnValue("StAddressNotes");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloor");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var location = entities[0].GetColumnValue("StLocationName");
                var town = entities[0].GetColumnValue("StTownName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");
                var newADSLLinePathId = entities[0].GetColumnValue("StNewADSLLinePathId");
                var freeReservationSwitchID = entities[0].GetColumnValue("StFreeReservationSwitchID");

                SOMRequestInput<LineMovingSubmitNewSwitchInput> somRequestInput = new SOMRequestInput<LineMovingSubmitNewSwitchInput>
                {

                    InputArguments = new LineMovingSubmitNewSwitchInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        OldPhoneNumber = oldPhoneNumber.ToString(),
                        NewPhoneNumber = newPhoneNumber.ToString(),
                        //OldTelLinePathId = oldLinePathId.ToString(),
                        NewTelLinePathId = newLinePathID.ToString(),
                        NewADSLLinePathId = newADSLLinePathId.ToString(),
                        NotApplicableServices = GetNotApplicableServicesOnSwitch(freeReservationSwitchID.ToString(), contractId.ToString()),
                        Address = new Address
                        {
                            Sequence = new ContractManager().GetContractAddressAndDirectoryInfo(contractId.ToString()).Address.Sequence.ToString(),
                            //new CRMCustomerManager().GetCustomerAddress(customerId.ToString()).Sequence.ToString(),
                            StateProvince = province.ToString(),
                            City = city.ToString(),
                            Town = town.ToString(),
                            Region = area.ToString(),
                            Street = street.ToString(),
                            Building = building.ToString(),
                            Floor = floor.ToString(),
                            Notes = notes.ToString(),
                            LocationType = location.ToString()

                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingSubmitNewSwitchInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ProceedFinalizeDiffSwitchTelephonyLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void FinishFinalizeDiffSwitchTelephonyLineMove(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPhoneNumber");
            esq.AddColumn("StNewPhoneNumber");
            esq.AddColumn("StIsNewSwitch");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");
            esq.AddColumn("StAddressNotes");
            esq.AddColumn("StNewLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StNewADSLLinePathId");
            esq.AddColumn("StFreeReservationSwitchID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldPhoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var newPhoneNumber = entities[0].GetColumnValue("StNewPhoneNumber");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                var street = entities[0].GetColumnValue("StStreet");
                var notes = entities[0].GetColumnValue("StAddressNotes");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloor");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var location = entities[0].GetColumnValue("StLocationName");
                var town = entities[0].GetColumnValue("StTownName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");
                var newADSLLinePathId = entities[0].GetColumnValue("StNewADSLLinePathId");
                var freeReservationSwitchID = entities[0].GetColumnValue("StFreeReservationSwitchID");

                SOMRequestInput<LineMovingSubmitNewSwitchInput> somRequestInput = new SOMRequestInput<LineMovingSubmitNewSwitchInput>
                {

                    InputArguments = new LineMovingSubmitNewSwitchInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        OldPhoneNumber = oldPhoneNumber.ToString(),
                        NewPhoneNumber = newPhoneNumber.ToString(),
                        //OldTelLinePathId = oldLinePathId.ToString(),
                        NewTelLinePathId = newLinePathID.ToString(),
                        NewADSLLinePathId = newADSLLinePathId.ToString(),
                        NotApplicableServices = GetNotApplicableServicesOnSwitch(freeReservationSwitchID.ToString(), contractId.ToString()),
                        Address = new Address
                        {
                            Sequence = new ContractManager().GetContractAddressAndDirectoryInfo(contractId.ToString()).Address.Sequence.ToString(),
                            //new CRMCustomerManager().GetCustomerAddress(customerId.ToString()).Sequence.ToString(),
                            StateProvince = province.ToString(),
                            City = city.ToString(),
                            Town = town.ToString(),
                            Region = area.ToString(),
                            Street = street.ToString(),
                            Building = building.ToString(),
                            Floor = floor.ToString(),
                            Notes = notes.ToString(),
                            LocationType = location.ToString()

                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LineMovingSubmitNewSwitchInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinishFinalizeDiffSwitchTelephonyLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }
        #endregion
    }
}
