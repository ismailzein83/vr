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
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLLineTerminationManager
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

        public SaleService GetVPNServiceFee(string contractId)
        {
            bool isVPNServiceFound = false;
            List<CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
            List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

            isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));

            /*foreach (var item in vpnServices)
            {
                foreach (var Service in contractServices)
                {
                    if (Service.Id == item.Id)
                    {
                        isVPNServiceFound = true;
                        break;
                    }
                }
                if (isVPNServiceFound) break;
            }*/

            if (isVPNServiceFound)
            {
                SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                vpnServiceFee.UpFront = true;
                return vpnServiceFee;
            }
            return null;
        }

        public void PostADSLLineTerminationToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineTermination");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPathId");
            esq.AddColumn("StTelephonyContractId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StReason");
            esq.AddColumn("StUserName");
            esq.AddColumn("StRatePlanId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var userName = entities[0].GetColumnValue("StUserName");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var telephonyContractId = entities[0].GetColumnValue("StTelephonyContractId");
                var pathdId = entities[0].GetColumnValue("StPathId");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLLineTerminationRequestInput> somRequestInput = new SOMRequestInput<ADSLLineTerminationRequestInput>
                {

                    InputArguments = new ADSLLineTerminationRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        Reason = reason.ToString(),
                        Username = userName.ToString(),
                        IsVPN = new CatalogManager().GetDivisionByRatePlanId(ratePlanId.ToString()) == "VPN" ? true : false,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
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
                    output = client.Post<SOMRequestInput<ADSLLineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/XDSLSubmitLineTermination/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void SubmitCabilingDisconnect(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineTermination");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPathId");
            esq.AddColumn("StTelephonyContractId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StReason");
            esq.AddColumn("StUserName");
            esq.AddColumn("StRatePlanId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var userName = entities[0].GetColumnValue("StUserName");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var pathdId = entities[0].GetColumnValue("StPathId");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLLineTerminationRequestInput> somRequestInput = new SOMRequestInput<ADSLLineTerminationRequestInput>
                {

                    InputArguments = new ADSLLineTerminationRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        Reason = reason.ToString(),
                        Username = userName.ToString(),
                        IsVPN = new CatalogManager().GetDivisionByRatePlanId(ratePlanId.ToString()) == "VPN" ? true : false,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
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
                    output = client.Post<SOMRequestInput<ADSLLineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitCabilingDisconnect/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void FinalizeCabilingDisconnect(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineTermination");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPathId");
            esq.AddColumn("StTelephonyContractId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StReason");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var telephonyContractId = entities[0].GetColumnValue("StTelephonyContractId");
                var pathdId = entities[0].GetColumnValue("StPathId");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLLineTerminationRequestInput> somRequestInput = new SOMRequestInput<ADSLLineTerminationRequestInput>
                {

                    InputArguments = new ADSLLineTerminationRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        Reason = reason.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
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
                    output = client.Post<SOMRequestInput<ADSLLineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeCabilingDisconnect/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void FinalizeADSLLineTermination(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineTermination");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPathId");
            esq.AddColumn("StTelephonyContractId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StReason");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var telephonyContractId = entities[0].GetColumnValue("StTelephonyContractId");
                var pathdId = entities[0].GetColumnValue("StPathId");
                var reason = entities[0].GetColumnValue("StReason");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);


                SOMRequestInput<ADSLLineTerminationRequestInput> somRequestInput = new SOMRequestInput<ADSLLineTerminationRequestInput>
                {

                    InputArguments = new ADSLLineTerminationRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
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
                    output = client.Post<SOMRequestInput<ADSLLineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/XDSLFinalizeLineTermination/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }
        #endregion
    }
}
