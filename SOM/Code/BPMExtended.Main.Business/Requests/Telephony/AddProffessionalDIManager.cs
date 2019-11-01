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
    public class AddProffessionalDIManager
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
        public void PostToOM(Guid requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StAddProfessionalDI");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StGivenName");
            esq.AddColumn("StBusinessTypeDisplay");
            esq.AddColumn("StBusinessTypeValue");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                string givenName = entities[0].GetColumnValue("StGivenName").ToString();
                string businessType = entities[0].GetColumnValue("StBusinessTypeDisplay").ToString();
                string serviceId = entities[0].GetColumnValue("StBusinessTypeValue").ToString();

                List<ServiceData> servicesList = new List<ServiceData>();
                servicesList.Add(new ServiceData() { Id = serviceId });

                CRMCustomerManager crmmanager = new CRMCustomerManager();
                CustomerAddress customerAddress = crmmanager.GetCustomerAddress(customerId.ToString());
                


                SOMRequestInput<AddProffessionalDIInput> somRequestInput = new SOMRequestInput<AddProffessionalDIInput>
                {
                    InputArguments = new AddProffessionalDIInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId.ToString(),
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        ServicesList = servicesList,
                        AddressSequence = customerAddress.Sequence,
                        BusinessType=businessType,
                        GivenName=givenName
                    }
                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<AddProffessionalDIInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/AddContractToPrivateDirectoryInquiry/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
