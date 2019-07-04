using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ProvisioningManager
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

        public string ResetNetworkServicePassword(string contractId , string requestId, string serviceId, string linePathId)
        {
            string processInstanceId = string.Empty;

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StNetworkResetPassword");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            string fees = string.Empty;
            bool isPaid = false;

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                isPaid = (bool)entities[0].GetColumnValue("StIsPaid");
            }

            var somRequestInput = new SOMRequestInput<ResetNetworkServicePasswordRequestInput>
            {
                InputArguments = new ResetNetworkServicePasswordRequestInput
                {
                    PaymentData = new PaymentData()
                    {
                        Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                        IsPaid = (bool)isPaid
                    },
                    CommonInputArgument = new CommonInputArgument()
                    {
                        ContractId = contractId,
                        RequestId = requestId
                    },
                    LinePathId = linePathId
                }
            };

            using (var client = new SOMClient())
            {
                client.Post<SOMRequestInput<ResetNetworkServicePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitResetKeyword/StartProcess", somRequestInput);
            }

            return processInstanceId;

        }
    }
}
