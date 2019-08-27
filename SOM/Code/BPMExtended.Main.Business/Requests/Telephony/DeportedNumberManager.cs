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
    public class DeportedNumberManager
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
        public void PostDeportedNumberToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeportedNumber");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StOldLinePathId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var oldPathId = entities[0].GetColumnValue("StOldLinePathId");
                var newPathId = entities[0].GetColumnValue("StLinePathID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<DeportedNumberRequestInput> somRequestInput = new SOMRequestInput<DeportedNumberRequestInput>
                {

                    InputArguments = new DeportedNumberRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        OldLinePath =oldPathId.ToString(),
                        NewLinePath = newPathId.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<DeportedNumberRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitDeportNumber/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }
        #endregion
    }
}
