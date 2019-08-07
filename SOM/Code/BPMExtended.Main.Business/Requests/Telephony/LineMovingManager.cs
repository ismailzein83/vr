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

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldLinePathId = entities[0].GetColumnValue("StOldLinePathId");
                var newLinePathID = entities[0].GetColumnValue("StNewLinePathID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var newSwitch = entities[0].GetColumnValue("StIsNewSwitch");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

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
                        OldLinePathId= oldLinePathId.ToString(),
                        NewLinePathId= newLinePathID.ToString(),
                        SameSwitch = !((bool)newSwitch),
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

            }
        }

        #endregion
    }
}
