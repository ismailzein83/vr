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
        public void PostLineMovingToOM(Guid requestId)
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
                            Notes= notes.ToString(),
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
                    output = client.Post<SOMRequestInput<LineMovingInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_LineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }


        public void PostLineMovingSubmitNewSwitch(Guid requestId)
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
                        OldDirectoryNumber = oldPhoneNumber.ToString(),
                        NewDirectoryNumber = newPhoneNumber.ToString(),
                        SameSwitch = !isNewSwitch,
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
                    output = client.Post<SOMRequestInput<LineMovingSubmitNewSwitchInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_LineMoveSubmitNewSwitch/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void ActivateLineMoving(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingRequest");
            esq.AddColumn("StContractId");
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


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
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
                        OldDirectoryNumber = oldPhoneNumber.ToString(),
                        NewDirectoryNumber = newPhoneNumber.ToString(),
                        SameSwitch = !isNewSwitch,
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
                    output = client.Post<SOMRequestInput<LineMovingSubmitNewSwitchInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivateLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        #endregion
    }
}
