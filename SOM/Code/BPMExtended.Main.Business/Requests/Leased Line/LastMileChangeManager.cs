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
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esq.AddColumn("StCountry");
            esq.AddColumn("StCountry.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");




            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var country = entities[0].GetColumnValue("StCountryName");

                ContractManager mngr = new ContractManager();
                GetContractAddressOutput contractAddressOutput = mngr.GetContractAddressAndDirectoryInfo(contractId.ToString());
                var sequence = contractAddressOutput.Address.Sequence.ToString();

                //check if one of the services is a vpn sevice
                List <CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
                List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

                isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));

              /*  foreach (var item in vpnServices)
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
                }*/

                List<SaleService> feesServices= JsonConvert.DeserializeObject<List<SaleService>>(fees);

                if (isVPNServiceFound)
                {
                    SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                    vpnServiceFee.UpFront = false;
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
                        Address = new Address()
                        {
                            CountryId = "206",
                            Building = buildingNumber.ToString(),
                            City=city.ToString(),
                            Floor = floor.ToString(),
                            Region = area.ToString(),
                            StateProvince = province.ToString(),
                            Street =street.ToString(),
                            Town = town.ToString(),
                            Sequence= sequence
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LastMileChangeRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitLastMileChange/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void SubmitLastMileChange(Guid requestId)
        {
            bool isVPNServiceFound = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLastMileChange");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCountry");
            esq.AddColumn("StCountry.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");

            esq.AddColumn("StLinePathId");
            esq.AddColumn("StOldLinePathId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var country = entities[0].GetColumnValue("StCountryName");

                var linePathId = entities[0].GetColumnValue("StLinePathId");
                var OldlinePathId = entities[0].GetColumnValue("StOldLinePathId");

                ContractManager mngr = new ContractManager();
                GetContractAddressOutput contractAddressOutput = mngr.GetContractAddressAndDirectoryInfo(contractId.ToString());
                var sequence = contractAddressOutput.Address.Sequence.ToString();

                //check if one of the services is a vpn sevice
                List<CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
                //List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

                //isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));
                
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);

                //if (isVPNServiceFound)
                //{
                //    SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                //    vpnServiceFee.UpFront = false;
                //    if (vpnServiceFee != null) feesServices.Add(vpnServiceFee);
                //}
                SOMRequestInput<LastMileChangeRequestInput> somRequestInput = new SOMRequestInput<LastMileChangeRequestInput>
                {
                    InputArguments = new LastMileChangeRequestInput
                    {
                        NewLinePathId = linePathId.ToString(),
                        OldLinePathId = OldlinePathId.ToString(),
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
                        Address = new Address()
                        {
                            CountryId = "206",
                            Building = buildingNumber.ToString(),
                            City = city.ToString(),
                            Floor = floor.ToString(),
                            Region = area.ToString(),
                            StateProvince = province.ToString(),
                            Street = street.ToString(),
                            Town = town.ToString(),
                            Sequence = sequence
                        }
                    }

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LastMileChangeRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitLastMileChange/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void ProceedLastMileChange(Guid requestId)
        {
            bool isVPNServiceFound = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLastMileChange");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCountry");
            esq.AddColumn("StCountry.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");

            esq.AddColumn("StLinePathId");
            esq.AddColumn("StOldLinePathId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var country = entities[0].GetColumnValue("StCountryName");

                var linePathId = entities[0].GetColumnValue("StLinePathId");
                var OldlinePathId = entities[0].GetColumnValue("StOldLinePathId");

                ContractManager mngr = new ContractManager();
                GetContractAddressOutput contractAddressOutput = mngr.GetContractAddressAndDirectoryInfo(contractId.ToString());
                var sequence = contractAddressOutput.Address.Sequence.ToString();

                //check if one of the services is a vpn sevice
                List<CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
                //List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

                //isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));

                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);

                //if (isVPNServiceFound)
                //{
                //    SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                //    vpnServiceFee.UpFront = false;
                //    if (vpnServiceFee != null) feesServices.Add(vpnServiceFee);
                //}
                SOMRequestInput<LastMileChangeRequestInput> somRequestInput = new SOMRequestInput<LastMileChangeRequestInput>
                {
                    InputArguments = new LastMileChangeRequestInput
                    {
                        NewLinePathId = linePathId.ToString(),
                        OldLinePathId = OldlinePathId.ToString(),
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
                        Address = new Address()
                        {
                            CountryId = "206",
                            Building = buildingNumber.ToString(),
                            City = city.ToString(),
                            Floor = floor.ToString(),
                            Region = area.ToString(),
                            StateProvince = province.ToString(),
                            Street = street.ToString(),
                            Town = town.ToString(),
                            Sequence = sequence
                        }
                    }

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LastMileChangeRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ProceedLastMileChange/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }
        public void FinalizeLastMileChange(Guid requestId)
        {

            bool isVPNServiceFound = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLastMileChange");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCountry");
            esq.AddColumn("StCountry.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");

            esq.AddColumn("StLinePathId");
            esq.AddColumn("StOldLinePathId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");


                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var country = entities[0].GetColumnValue("StCountryName");

                var linePathId = entities[0].GetColumnValue("StLinePathId");
                var OldlinePathId = entities[0].GetColumnValue("StOldLinePathId");

                ContractManager mngr = new ContractManager();
                GetContractAddressOutput contractAddressOutput = mngr.GetContractAddressAndDirectoryInfo(contractId.ToString());
                var sequence = contractAddressOutput.Address.Sequence.ToString();

                //check if one of the services is a vpn sevice
                List<CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
                List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

                isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));

                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);

                if (isVPNServiceFound)
                {
                    SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                    vpnServiceFee.UpFront = false;
                    if (vpnServiceFee != null) feesServices.Add(vpnServiceFee);
                }
                SOMRequestInput<LastMileChangeRequestInput> somRequestInput = new SOMRequestInput<LastMileChangeRequestInput>
                {
                    InputArguments = new LastMileChangeRequestInput
                    {
                        NewLinePathId = linePathId.ToString(),
                        OldLinePathId = OldlinePathId.ToString(),
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
                        Address = new Address()
                        {
                            CountryId = "206",
                            Building = buildingNumber.ToString(),
                            City = city.ToString(),
                            Floor = floor.ToString(),
                            Region = area.ToString(),
                            StateProvince = province.ToString(),
                            Street = street.ToString(),
                            Town = town.ToString(),
                            Sequence = sequence
                        }
                    }

                };
                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LastMileChangeRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeLastMileChange/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }
        }

    }
}
