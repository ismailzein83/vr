﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CommonManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public OutputResult ValidateRequest(string contactId, string accountId)
        {
            bool isSkip = false;
            string msgCode = "";
            ResultStatus status;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            Contact customerInfo;

            //TODO:check if contact or account

            customerInfo = GetContact(contactId); 

            // check categories catalog
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StSkipBalanceCheck");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryID", customerInfo.CustomerCategoryID);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isSkip = (bool)entities[0].GetColumnValue("StSkipBalanceCheck");
            }

            if (isSkip)
            {
                msgCode = Constant.VALIDATION_BALANCE_VALID;
                status = ResultStatus.Success;
            }
            else
            {
                //check customer balance
                using (SOMClient client = new SOMClient())
                {
                    decimal? balance = client.Get<decimal?>(String.Format("api/SOM.ST/Billing/GetCustomerBalance?CustomerId={0}", customerInfo.CustomerId));

                    if (balance == 0 || balance == null)
                    {
                        msgCode = Constant.VALIDATION_BALANCE_VALID;
                        status = ResultStatus.Success;                        
                    }
                    else
                    {
                        msgCode = Constant.VALIDATION_BALANCE_NOT_VALID;
                        status = ResultStatus.Error;
                    }
                }               

            }

            return new OutputResult()
            {
                messages = new List<string>() { msgCode },
                status = status
            };

        }
        public string AbortRequest(string requestId)
        {
            var customerRequestManager = new CustomerRequestManager();
            customerRequestManager.SetRequestCompleted(requestId);
            customerRequestManager.CancelRequestHeader(requestId);
            return "";
        }

        public AddressData GetAddressData(string city , string province , string area, string town)
        {
            Guid cityId=Guid.Empty, provinceId= Guid.Empty, areaId= Guid.Empty, townId= Guid.Empty;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            //city
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "City");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", city);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                cityId = entities[0].GetTypedColumnValue<Guid>("Id");
            }

            //province
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Region");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", province);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                provinceId = entities[0].GetTypedColumnValue<Guid>("Id");
            }

            //area
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StAreaLookup");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", area);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                areaId = entities[0].GetTypedColumnValue<Guid>("Id");
            }

            //town
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTownLookup");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", town);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                townId = entities[0].GetTypedColumnValue<Guid>("Id");
            }


            return new AddressData()
            {
                CityId=cityId,
                AreaId = areaId,
                ProvinceId = provinceId,
                TownId = townId,
            };
        }

        public int GetNumberOfAttachments(string schemaName , string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, schemaName+"File");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, schemaName+".Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities != null)
            {
                return entities.Count;
            }

            return 0;
        }

        public string GetSubTypeIdentifier(string subTypeId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSubTypes");
            esq.AddColumn("StSubTypeIdentifier");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal,"Id", subTypeId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return entities[0].GetColumnValue("StSubTypeIdentifier").ToString();
            }

            return null;
        }

        public Contact GetContact(string contactId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            Contact contact = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Name");
            esq.AddColumn("StDocumentID");
            esq.AddColumn("StCSO");
            esq.AddColumn("StCSO.Id");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCustomerCode");
            esq.AddColumn("StCustomerCategoryID");
            esq.AddColumn("StCustomerCategoryName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var csoId = entities[0].GetColumnValue("StCSOId");
                var name = entities[0].GetColumnValue("Name");
                var documentId = entities[0].GetColumnValue("StDocumentID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var customerCode = entities[0].GetColumnValue("StCustomerCode");
                var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                contact = new Contact()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString(),
                    DocumentID = documentId.ToString(),
                    CustomerCategoryID = customerCategoryId.ToString(),
                    CustomerCategoryName = customerCategoryName.ToString(),
                    CSOId = csoId.ToString(),
                    CustomerCode = customerCode.ToString()


                };
            }
            return contact;
        }

        public Account GetAccount(string accountId)
        {
            return null;
        }

        public void UpdateDepositDocumentId(string requestId, List<DepositDocument> depositDocumentId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<DepositDocument> listOfDeposites = new List<DepositDocument>();

            string entityName = new CRMCustomerManager().GetEntityNameByRequestId(requestId);

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            esq.AddColumn("StOperationAddedDeposites");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                string deposites = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();

                if (deposites != "\"\"" && deposites != null && deposites != "")
                {
                    listOfDeposites = JsonConvert.DeserializeObject<List<DepositDocument>>(deposites);
                }

                listOfDeposites.Concat(depositDocumentId);


                //update deposites
                UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var update = new Update(connection, entityName).Set("StOperationAddedDeposites", Column.Parameter(JsonConvert.SerializeObject(listOfDeposites)))
                    .Where("Id").IsEqual(Column.Parameter(requestId));
                update.Execute();


            }

        }

    }
}
