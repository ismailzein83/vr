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

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public List<string> GetSpecialServicesIds()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<string> specialServicesIds = new List<string>();
            string publicId;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSpecialServiceCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StServiceId");
            esq.AddColumn("StIsSpecial");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StIsSpecial", true);


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                publicId = (string)entity.GetColumnValue("StServiceId");
                specialServicesIds.Add(publicId);

            }

            return specialServicesIds;

        }

        public ServiceInfo GetDepositServiceByPagesNumber(int pagesNumber) // MYA: To Be Tested 
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

            return new ServiceInfo()
            {
                ServiceId = serviceId,
                Name = serviceName
            };

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
            for (int i = 0; i < entities.Count; i++)
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
    }


    public class OperationServices
    {
        public List<SaleService> Fees { get; set; }
        public List<VASService> Services { get; set; }

    }

}
