using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LastMileChangeManager
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

        public string GetConnectionType(Guid requestId)
        {
            Random random = new Random();
            return random.Next(10) > 5 ? "Fiber" : "Copper";
        }

        public void SubmitLastMileChangeRequestToOM(Guid requestId)
        {
            bool isVPNServiceFound = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLastMileChange");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                //check if one of the services is a vpn sevice
                List <CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
                List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

                foreach(var item in vpnServices)
                {
                    foreach(var Service in contractServices)
                    {
                        if (Service.Id == item.Id)
                        {
                            isVPNServiceFound = true;
                            break;
                        }
                    }
                    if (isVPNServiceFound) break;
                }

                List<SaleService> feesServices= JsonConvert.DeserializeObject<List<SaleService>>(fees);

                if (isVPNServiceFound)
                {
                    SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                    if (vpnServiceFee != null) feesServices.Add(vpnServiceFee);
                }

                SOMRequestInput<LastMileChangeRequestInput> somRequestInput = new SOMRequestInput<LastMileChangeRequestInput>
                {

                    InputArguments = new LastMileChangeRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = feesServices,
                            IsPaid = (bool)isPaid
                        },
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LastMileChangeRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_LL_CreateContract/StartProcess", somRequestInput);
                }

            }

        }

    }
}
