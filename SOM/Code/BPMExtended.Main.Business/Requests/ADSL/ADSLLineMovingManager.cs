﻿using System;
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
    public class ADSLLineMovingManager
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
        public void PostADSLLineMovingToOM(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineMoving");
            esq.AddColumn("StTelephonyContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StADSLContractId");
            esq.AddColumn("StSelectedTelephonyContract");
            esq.AddColumn("StSelectedDSLAMPort");
            esq.AddColumn("StFloor");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StStreet");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StIsSameSwitch");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var oldContractId = entities[0].GetColumnValue("StTelephonyContractId");
                var newContractId = entities[0].GetColumnValue("StSelectedTelephonyContract");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var adslContractId = entities[0].GetColumnValue("StADSLContractId");
                var newDSLAMPort = entities[0].GetColumnValue("StSelectedDSLAMPort");
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                bool isSameSwitch = (bool)entities[0].GetColumnValue("StIsSameSwitch");

                SOMRequestInput<ADSLLineMovingRequestInput> somRequestInput = new SOMRequestInput<ADSLLineMovingRequestInput>
                {

                    InputArguments = new ADSLLineMovingRequestInput
                    {
                        NewDSLAM = isSameSwitch? "425": newDSLAMPort.ToString(),
                        OldDSLAM = "425",
                        AddressSequence = new ContractManager().GetTelephonyContract(newContractId.ToString()).ContractAddress.Sequence,
                        Address = new Address
                        {
                            CountryId = "206",
                            StateProvince= province.ToString(),
                            City=city.ToString(),
                            Town = town.ToString(),
                            Region=area.ToString(),
                            Building=buildingNumber.ToString(),
                            Floor= floor.ToString(),
                            Street=street.ToString(),
                            
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        OldTelephonyContractId = oldContractId.ToString(),
                        NewTelephonyContractId= newContractId.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = adslContractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLLineMovingRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitADSLLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FinalizeADSLLineMovingToOM(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineMoving");
            esq.AddColumn("StADSLContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StAddressSequence");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var adslContractId = entities[0].GetColumnValue("StADSLContractId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ADSLLineMovingRequestInput> somRequestInput = new SOMRequestInput<ADSLLineMovingRequestInput>
                {

                    InputArguments = new ADSLLineMovingRequestInput
                    {
                        ContractId = adslContractId.ToString(),
                        RequestId = requestId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = adslContractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLLineMovingRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeADSLLineMove/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
