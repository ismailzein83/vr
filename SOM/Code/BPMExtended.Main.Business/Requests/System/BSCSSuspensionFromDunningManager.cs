using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Newtonsoft.Json;

namespace BPMExtended.Main.Business
{
    public class BSCSSuspensionFromDunningManager
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
        public void PostToOM(Guid requestId)
        {
            BPMExtended.Main.Business.CustomerRequestManager manager = new BPMExtended.Main.Business.CustomerRequestManager();
            manager.SetRequestCompleted(requestId.ToString());
            ////Get Data from StLineSubscriptionRequest table
            //EntitySchemaQuery esq;
            //IEntitySchemaQueryFilterItem esqFirstFilter;
            //SOMRequestOutput output;

            //esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "BSCSSuspensionFromDunning");
            //esq.AddColumn("StContractID");
            //esq.AddColumn("StCustomerId");
            //esq.AddColumn("StCptId");
            //esq.AddColumn("StPhoneNumber");
            //esq.AddColumn("StOperationAddedFees");
            //esq.AddColumn("StIsPaid");
            //esq.AddColumn("StCptNumber");


            //esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            //esq.Filters.Add(esqFirstFilter);

            //var entities = esq.GetEntityCollection(BPM_UserConnection);
            //if (entities.Count > 0)
            //{
            //    var contractId = entities[0].GetColumnValue("StContractID");
            //    var cptId = entities[0].GetColumnValue("StCptId");
            //    var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
            //    var customerId = entities[0].GetColumnValue("StCustomerId");
            //    string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
            //    var isPaid = entities[0].GetColumnValue("StIsPaid");
            //    var cptNumber = entities[0].GetColumnValue("StCptNumber");

            //    SOMRequestInput<ActivateCptRequestInput> somRequestInput = new SOMRequestInput<ActivateCptRequestInput>
            //    {

            //        InputArguments = new ActivateCptRequestInput
            //        {
            //            PaymentData = new PaymentData()
            //            {
            //                Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
            //                IsPaid = (bool)isPaid
            //            },
            //            CommonInputArgument = new CommonInputArgument()
            //            {
            //                ContractId = contractId.ToString(),
            //                RequestId = requestId.ToString(),
            //                CustomerId = customerId.ToString()
            //            },
            //            CPTServiceId = new CatalogManager().GetCPTServiceId(),
            //            DirectoryNumber = phoneNumber.ToString(),
            //            CPTId = cptId.ToString(),
            //            CPTNumber = cptNumber.ToString()
            //        }

            //    };

            //    //call api
            //    using (var client = new SOMClient())
            //    {
            //        output = client.Post<SOMRequestInput<ActivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_SubmitCPT/StartProcess", somRequestInput);
            //    }
            //    var manager = new BusinessEntityManager();
            //    manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            //}

        }
        public void ActivateRequest(Guid requestId)
        {
            ////Get Data from StLineSubscriptionRequest table
            //EntitySchemaQuery esq;
            //IEntitySchemaQueryFilterItem esqFirstFilter;
            //SOMRequestOutput output;

            //esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCpt");
            //esq.AddColumn("StContractID");
            //esq.AddColumn("StCustomerId");
            //esq.AddColumn("StCptId");
            //esq.AddColumn("StPhoneNumber");
            //esq.AddColumn("StCptNumber");
            //esq.AddColumn("StOperationAddedFees");
            //esq.AddColumn("StIsPaid");


            //esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            //esq.Filters.Add(esqFirstFilter);

            //var entities = esq.GetEntityCollection(BPM_UserConnection);
            //if (entities.Count > 0)
            //{
            //    var contractId = entities[0].GetColumnValue("StContractID");
            //    var cptId = entities[0].GetColumnValue("StCptId");
            //    var cptNumber = entities[0].GetColumnValue("StCptNumber");
            //    var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
            //    var customerId = entities[0].GetColumnValue("StCustomerId");
            //    string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
            //    var isPaid = entities[0].GetColumnValue("StIsPaid");

            //    SOMRequestInput<ActivateCptRequestInput> somRequestInput = new SOMRequestInput<ActivateCptRequestInput>
            //    {

            //        InputArguments = new ActivateCptRequestInput
            //        {
            //            PaymentData = new PaymentData()
            //            {
            //                Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
            //                IsPaid = (bool)isPaid
            //            },
            //            CommonInputArgument = new CommonInputArgument()
            //            {
            //                ContractId = contractId.ToString(),
            //                RequestId = requestId.ToString(),
            //                CustomerId = customerId.ToString()
            //            },
            //            CPTServiceId = new CatalogManager().GetCPTServiceId(),
            //            CPTId = cptId.ToString(),
            //            CPTNumber = cptNumber.ToString()
            //        }

            //    };

            //    //call api
            //    using (var client = new SOMClient())
            //    {
            //        output = client.Post<SOMRequestInput<ActivateCptRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivateCPT/StartProcess", somRequestInput);
            //    }
            //    var manager = new BusinessEntityManager();
            //    manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            //}
        }

        public void CreateRequest(string contactId)
        {
            EntitySchema schema = BPM_UserConnection.EntitySchemaManager.GetInstanceByName("StBSCSSuspensionFromDunning");
            Entity workorder = schema.CreateEntity(BPM_UserConnection);

            Guid workOrderId = Guid.NewGuid();

            workorder.SetColumnValue("Id", workOrderId);
            workorder.SetColumnValue("StName", "Suspension for: " + contactId);
            workorder.SetColumnValue("StTypeId", "B0F39A64-6B4A-4350-BFEA-D6ED5461C691"); 
            workorder.Save();
        }
        #endregion
    }
}
