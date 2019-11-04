using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using SOM.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CatalogManager
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
        public List<ServiceData> GetAddPublicDIServices()
        {
            List<ServiceData> services = new List<ServiceData>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StPublicDIId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "e7cf42f9-7a83-4ad2-a73a-5203c94a4da2");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var DIId = (string)entities[0].GetColumnValue("StPublicDIId");
                services.Add(new ServiceData() { Id = DIId});

            }
            return services;
        }
        public List<ServiceData> GetRemovePublicDIServices()
        {
            List<ServiceData> services = new List<ServiceData>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StRemovePublicDIId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "e7cf42f9-7a83-4ad2-a73a-5203c94a4da2");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var DIId = (string)entities[0].GetColumnValue("StRemovePublicDIId");
                services.Add(new ServiceData() { Id = DIId });

            }
            return services;
        }
        public List<SaleService> GetTelephonyPhoneCategoryFees(string phoneCategoryId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<SaleService> fees = new List<SaleService>();
            string phoneCategoryCatalogId = null, serviceID, serviceName;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyPhoneCategoryServicesCatalog");
            var IdCol = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StPhoneCategory.Id", phoneCategoryId);

            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                phoneCategoryCatalogId = entities[0].GetTypedColumnValue<Guid>(IdCol.Name).ToString();

                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPOSServiceInPhoneCategoryCatalog");
                esq.AddColumn("StPOSServiceID");
                esq.AddColumn("StPOSServiceName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StTelephonyPhoneCategoryServicesCatalog.Id", phoneCategoryCatalogId);

                esq.Filters.Add(esqFirstFilter);

                entities = esq.GetEntityCollection(BPM_UserConnection);
                foreach (Entity item in entities)
                {
                    serviceID = (string)item.GetColumnValue("StPOSServiceID");
                    serviceName = (string)item.GetColumnValue("StPOSServiceName");

                    fees.Add(new SaleService()
                    {
                        Id=serviceID,
                        Name= serviceName,
                        UpFront=true,                  
                    });
                }
            }
            return fees;
        }

        public List<string> GetSpecialServicesIds()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<string> specialServicesIds = new List<string>();
            string publicId;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceDefinitionInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StServiceID");
            esq.AddColumn("StExcludedFromServiceAddition");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StExcludedFromServiceAddition", true);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                publicId = (string)entity.GetColumnValue("StServiceID");
                specialServicesIds.Add(publicId);

            }
            return specialServicesIds;
        }

       /* public List<string> GetIsRequiredPasswordServicesIds()// MYA: To be tested 
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            var isRequiredPasswordServices = new List<string>();
            string publicId;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSpecialServiceCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StServiceId");
            esq.AddColumn("StIsRequiresChangePassword");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StIsRequiresChangePassword", true);

            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                publicId = (string)entity.GetColumnValue("StServiceId");
                isRequiredPasswordServices.Add(publicId);
            }

            return isRequiredPasswordServices;
        }*/

        public string GetPABXServiceId()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string pabxServiceId=null;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("Id");
            esq.AddColumn("StPABXId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "11C1BFA4-1F9E-41AB-B4D7-FE373DFBFE9C");


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                pabxServiceId = (string)entity.GetColumnValue("StPABXId");

            }

            return pabxServiceId;

        }

        public string GetADSLUserNameDomain()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string domain = null;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("Id");
            esq.AddColumn("StDomain");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "4D5297E3-1681-4672-8F64-22EC9E478065");


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                domain = (string)entity.GetColumnValue("StDomain");

            }

            return domain;

        }

        public int? GetPendingDaysBeforeCancellingRequest() {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string pendindDays = String.Empty;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("Id");
            esq.AddColumn("StCancelRequestAfter");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "72F3DDE4-C52B-4978-BB59-BA2B3A92ED9C");


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                pendindDays = (string)entity.GetColumnValue("StCancelRequestAfter");

            }
            if (String.IsNullOrEmpty(pendindDays))
            return null;
            return  int.Parse(pendindDays);
        }

        public string GetRatePlanNameFromCatalog(string rateplanId)
        {
            string ratePlanName = null;

            if (rateplanId == null)
                return null;

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRatePlansInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StRatePlanID");
            esq.AddColumn("StRatePlanName");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRatePlanID", rateplanId);


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                ratePlanName = (string)entity.GetColumnValue("StRatePlanName");

            }

            return ratePlanName;
        }

        public List<string> GetTelephonyOperations()
        {
            EntitySchemaQuery esq;
            EntityCollection entities;
            List<string> telephonyOperationsIds = new List<string>();


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyOperationsInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StOperationId");

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                string id = (string)entity.GetColumnValue("StOperationId");
                telephonyOperationsIds.Add(id);

            }

            return telephonyOperationsIds;

        }

        public SaleService GetCallDetailServiceByPagesNumber(int pagesNumber)
        {
            string serviceId = null;
            string serviceName = null;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StZeroToFivePagesId");
            esq.AddColumn("StFiveToTenPagesId");
            esq.AddColumn("StAboveTenPagesId");
            esq.AddColumn("StZeroToFivePagesDisplayValue");
            esq.AddColumn("StFiveToTenPagesDisplayValue");
            esq.AddColumn("StAboveTenPagesDisplayValue");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "61FB364E-BA71-437F-9BD1-D9BF9F1AB07C");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                if (pagesNumber < 6)
                {
                    serviceId = entities[0].GetColumnValue("StZeroToFivePagesId").ToString();
                    serviceName = entities[0].GetColumnValue("StZeroToFivePagesDisplayValue").ToString();
                }
                else if (pagesNumber < 11)
                {
                    serviceId = entities[0].GetColumnValue("StFiveToTenPagesId").ToString();
                    serviceName = entities[0].GetColumnValue("StFiveToTenPagesDisplayValue").ToString();
                }
                else if (pagesNumber >= 11)
                {
                    serviceId = entities[0].GetColumnValue("StAboveTenPagesId").ToString();
                    serviceName = entities[0].GetColumnValue("StAboveTenPagesDisplayValue").ToString();
                }
            }

            return new SaleService()
            {
                Id = serviceId,
                Name= serviceName,
                UpFront=true
            };

        }

        public int GetCallDetailsYearsNumber()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StYearsNumbers");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "61FB364E-BA71-437F-9BD1-D9BF9F1AB07C");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {                
                    return entities[0].GetTypedColumnValue<int>("StYearsNumbers");
            }
            return 0;
        }

        public List<DepositDocument> GetForeignerDeposits(string selectedServices)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            string fIGAValue=null;
            string fGAValue = null;
            bool NotAllItemsExist;
            List<string> servicesIds = new List<string>();
            List<DepositDocument> deposits = new List<DepositDocument>();
            List<string> optionalSelectedServices = new List<string>();

            //Get FGA and FIGA values
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StFIGAId");
            esq.AddColumn("StFGAId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "D4A736AB-9C48-4F68-97D8-6285E3E1CAA8");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                 fIGAValue = entities[0].GetColumnValue("StFIGAId").ToString();
                 fGAValue = entities[0].GetColumnValue("StFGAId").ToString();

            }

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceInGeneralSettings");
            esq.AddColumn("StServiceID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StGeneralSettings", "D4A736AB-9C48-4F68-97D8-6285E3E1CAA8");
            esq.Filters.Add(esqFirstFilter);

            var servicesEntities = esq.GetEntityCollection(BPM_UserConnection);
            for (int i = 0; i < servicesEntities.Count; i++)
            {
                string serviceId = servicesEntities[0].GetColumnValue("StServiceID").ToString();
                servicesIds.Add(serviceId);

            }

            //Deserialize
            if (selectedServices != null && selectedServices != "\"\"")
            {
                optionalSelectedServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(selectedServices).Select(p => p.Id).ToList();
                NotAllItemsExist = optionalSelectedServices.Any(p => !servicesIds.Contains(p));
            }
            else
            {
                NotAllItemsExist = true;
            }

            if(NotAllItemsExist)
            {
                deposits.Add(new DepositDocument() { Id = fIGAValue });
                deposits.Add(new DepositDocument() { Id = fGAValue });
                
            }
            else
            {
                deposits.Add(new DepositDocument(){Id = fIGAValue });
            }

            return deposits;

        }

        public int GetNumberOfRecordsPerPage()
        {
            int numberOfRecords = 0;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StNumberOfRecordsPerPage");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "61FB364E-BA71-437F-9BD1-D9BF9F1AB07C");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var stringifiedNumberOfRecords = (string)entities[0].GetColumnValue("StNumberOfRecordsPerPage");
                int.TryParse(stringifiedNumberOfRecords, out numberOfRecords);
            }
            return numberOfRecords;
        }

        public int GetNumberOfInvoicesForTelephonyContractTakeOver()
        {
            int numberOfInvoices = 0;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StNumberOfInvoices");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "204A6410-8947-4E88-AF4E-F705DF7A0984");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var stringifiedNumberOfInvoices = (string)entities[0].GetColumnValue("StNumberOfInvoices");
                int.TryParse(stringifiedNumberOfInvoices, out numberOfInvoices);
            }
            return numberOfInvoices;
        }

        public OperationServices GetOperationServices(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string requestType;
            string catalogId;
            List<SaleService> fees = new List<SaleService>();
            List<VASService> services = new List<VASService>();


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StRequestType");
            esq.AddColumn("StRequestId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                requestType = entities[0].GetTypedColumnValue<string>("StRequestType");

                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPOSServiceOperationCatalog");
                var catId = esq.AddColumn("Id");
                esq.AddColumn("StOperationTypeId");


                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationTypeId", requestType);
                esq.Filters.Add(esqFirstFilter);

                entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    //catalogId = entities[0].GetTypedColumnValue<Guid>("Id");
                    catalogId = entities[0].GetTypedColumnValue<string>(catId.Name);

                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPOSServiceInCatalog");
                    esq.AddColumn("Id");
                    esq.AddColumn("StPOSServiceName");
                    esq.AddColumn("StPOSServiceID");
                    esq.AddColumn("StUpFront");


                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StPOSServiceCatalog", catalogId);
                    esq.Filters.Add(esqFirstFilter);

                    entities = esq.GetEntityCollection(BPM_UserConnection);
                    foreach (Entity entity in entities)
                    {
                        fees.Add(new SaleService()
                        {
                            Id = (string)entity.GetColumnValue("StPOSServiceID"),
                            Name = (string)entity.GetColumnValue("StPOSServiceName"),
                            UpFront = (bool)entity.GetColumnValue("StUpFront"),

                        });

                    }


                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceInOperationCatalog");
                    esq.AddColumn("Id");
                    esq.AddColumn("StServiceName");
                    esq.AddColumn("StServiceID");
                    esq.AddColumn("StIsNetwork");
                    esq.AddColumn("StNeedsProvisioning");


                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StPOSServiceCatalog", catalogId);
                    esq.Filters.Add(esqFirstFilter);

                    entities = esq.GetEntityCollection(BPM_UserConnection);
                    foreach (Entity entity in entities)
                    {
                        services.Add(new VASService()
                        {
                            Id = (string)entity.GetColumnValue("StServiceID"),
                            Name = (string)entity.GetColumnValue("StServiceName"),
                            NeedProvisioning = (bool)entity.GetColumnValue("StNeedsProvisioning"),
                            IsNetwork= (bool)entity.GetColumnValue("StIsNetwork"),

                        });

                    }


                }

            }


            return new OperationServices()
            {
                Fees = fees,
                Services = services
            };

        }

        public string GetExternalCustomerSetId(string segmentId)
        {
            string externalCustomerSetID = null;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSegments");
            esq.AddColumn("StExternalCustomerSetID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", segmentId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                externalCustomerSetID = entities[0].GetTypedColumnValue<string>("StExternalCustomerSetID");
            }

            return externalCustomerSetID;
        }

        public List<SaleService> GetADSLFeesForTelephonyContractTakeOver()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            //IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StContractTakeOverADSLFees");
            var Id = esq.AddColumn("StServiceID");
            esq.AddColumn("StServiceName");

            //esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StGeneralSettings", "204A6410-8947-4E88-AF4E-F705DF7A0984");
            //esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    fees.Add(new SaleService() {

                        Id = item.GetTypedColumnValue<string>(Id.Name),
                        Name = item.GetTypedColumnValue<string>("StServiceName"),
                        UpFront = false
                    });
                }
            }

            return fees;

        }

        public string GetCPTServiceId()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StCptServiceId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "E3CE1E0B-1DBE-4AE0-B80D-CBB2F1E46C63");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<string>("StCptServiceId");
            }
            return null;
        }

        public string GetDeviceTypeIdentifier(string phoneNumber)
        {

            return GetDeviceTypeByName(new InventoryManager().GetTechnicalDetails(phoneNumber).DEV_TYPE).Identifier;

        }

        public string GetDeviceTypeIdentifierByPathId(string pathId)
        {

            return GetDeviceTypeByName(new InventoryManager().GetTechnicalDetailsByPath(pathId).DEV_TYPE).Identifier;

        }

        public DeviceType GetDeviceTypeByName(string deviceTypeName)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeviceType");
            var IdCol = esq.AddColumn("Id");
            esq.AddColumn("StName");
            esq.AddColumn("StInventoryIdentifier");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StName", deviceTypeName);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return new DeviceType
                {
                    Id = entities[0].GetTypedColumnValue<string>(IdCol.Name),
                    Identifier = entities[0].GetTypedColumnValue<string>("StInventoryIdentifier"),
                    Name = entities[0].GetTypedColumnValue<string>("StName")
                };
            }
            return null;
        }

        public DeviceType GetDeviceTypeById(string deviceTypeId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeviceType");
            var IdCol = esq.AddColumn("Id");
            esq.AddColumn("StName");
            esq.AddColumn("StInventoryIdentifier");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", deviceTypeId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return new DeviceType
                {
                    Id = entities[0].GetTypedColumnValue<string>(IdCol.Name),
                    Identifier = entities[0].GetTypedColumnValue<string>("StInventoryIdentifier"),
                    Name = entities[0].GetTypedColumnValue<string>("StName")
                };
            }
            return null;
        }


        public List<DeviceType> GetDevicesTypes()
        {
            List<DeviceType> deviceTypes = new List<DeviceType>();
            EntitySchemaQuery esq;
            
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeviceType");
            var IdCol = esq.AddColumn("Id");
            esq.AddColumn("StName");
            esq.AddColumn("StInventoryIdentifier");

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    string deviceTypeId = item.GetTypedColumnValue<string>(IdCol.Name);
                    string name  = item.GetTypedColumnValue<string>("StName");
                    string identifier = item.GetTypedColumnValue<string>("StInventoryIdentifier");

                    deviceTypes.Add(new DeviceType
                    {
                        Id = deviceTypeId,
                        Identifier= identifier,
                        Name = name
                    });
                }
            }
            return deviceTypes;
        }

        public string GetWaitingListDepositAmount()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StWaitingListDepositAmount");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "825208A4-FA56-4678-9EE4-529D82A8E17D");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<string>("StWaitingListDepositAmount");
            }
            return null;
        }

        public string GetDowngradeSpeedServiceId()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StDowngradeSpeedServiceId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "76F1F8CB-B826-41BD-B087-5C4F4F42AD44");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<string>("StDowngradeSpeedServiceId");
            }
            return null;
        }

        public string GetUpgradSpeedServiceId()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StUpgradeSpeedServiceId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "76F1F8CB-B826-41BD-B087-5C4F4F42AD44");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<string>("StUpgradeSpeedServiceId");
            }
            return null;
        }

        public SaleService GetVPNServiceFee()
        {
            string serviceId;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("StVPNServiceFeeId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "1FFF1557-AF95-458D-ACE6-EF3E5B1FDA48");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                serviceId = entities[0].GetTypedColumnValue<string>("StVPNServiceFeeId");

                return new SaleService()
                {
                    Id = serviceId
                };              
            }
            return null;
        }

        public List<SaleService> GetADSLFeesForLineTermination()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            //IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineTerminationADSLFees");
            var Id = esq.AddColumn("StServiceID");
            esq.AddColumn("StServiceName");

            //esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StGeneralSettings", "08C36F0B-2509-40D8-AD0A-4AAEC7B6DFF2");
            //esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    fees.Add(new SaleService()
                    {

                        Id = item.GetTypedColumnValue<string>(Id.Name),
                        Name = item.GetTypedColumnValue<string>("StServiceName"),
                        UpFront = false
                    });
                }
            }

            return fees;

        }

        public List<SaleService> GetVPNDivisionServices()
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDivisionServices");
            var Id = esq.AddColumn("StServiceID");
            esq.AddColumn("StServiceName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StDivision", "DBBFA17A-51B6-49A9-BA90-F22B949CBD5E");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    fees.Add(new SaleService()
                    {

                        Id = item.GetTypedColumnValue<string>(Id.Name),
                        Name = item.GetTypedColumnValue<string>("StServiceName"),
                        UpFront = false
                    });
                }
            }
            return fees;
        }

        public string GetDivisionByRatePlanId(string ratePlanId)
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRatePlansInCatalog");
            var division= esq.AddColumn("StRatePlanCatalog.StSubTypeDivision.StName");
            esq.AddColumn("StRatePlanID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRatePlanID", ratePlanId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<string>(division.Name);
             
            }

            return null;

        }

        public int GetRankByRatePlanId(string ratePlanId)
        {
            List<SaleService> fees = new List<SaleService>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRatePlansInCatalog");
            //var rank = esq.AddColumn("StRatePlanCatalog.StRank");
            esq.AddColumn("StRatePlanID");
            esq.AddColumn("StRank");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRatePlanID", ratePlanId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetTypedColumnValue<int>("StRank");
            }
            return -1;
        }

        public Dictionary<string,string> GetDiscountServicesByServiceId() // To be Tested
        {
            var discountServices = new Dictionary<string, string>();
            EntitySchemaQuery esq;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServicesDiscount");
            esq.AddColumn("StServiceId");
            esq.AddColumn("StDiscountId");

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    discountServices.Add(item.GetTypedColumnValue<string>("StServiceId"), item.GetTypedColumnValue<string>("StDiscountId"));
                }
            }
            return discountServices;
        }

        #endregion
    }


    public class OperationServices
    {
        public List<SaleService> Fees { get; set; }
        public List<VASService> Services { get; set; }

    }

}
