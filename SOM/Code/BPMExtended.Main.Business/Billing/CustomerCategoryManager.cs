﻿using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
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
    public class CustomerCategoryManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public List<CustomerCategoryInfo> GetCustomerCategoryInfo()
        {
            var customerCategoryInfoItems = new List<CustomerCategoryInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<CustomerCategory> items= client.Get<List<CustomerCategory>>(String.Format("api/SOM.ST/Billing/GetCustomerCategories"));
                foreach (var item in items)
                {
                    var customerCatergoryInfoItem = CustomerCategoryToInfoMapper(item);
                    customerCategoryInfoItems.Add(customerCatergoryInfoItem);
                }
            }
            return customerCategoryInfoItems;
        }

        public List<CustomerCategoryCatalog> GetCustomerCategoriesCatalogBySegmentId(string segmentId)
        {
            List<CustomerCategoryCatalog> customerCategoryCatalogItems = null;
            Guid id = new Guid(segmentId.ToUpper());
            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoryCatalog");


            esq.AddColumn("StName");
            esq.AddColumn("StCustomerSegmentsId");
            esq.AddColumn("StCustomerCategory");
            esq.AddColumn("StIsSkipPayment");
            esq.AddColumn("StIsNormal");


            var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerSegmentsId", id);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            for(int i=0;i> entities.Count; i++)
                {
                    var name = entities[i].GetColumnValue("StName");
                    var customerSegmentId = entities[i].GetColumnValue("StCustomerSegmentsId");
                    var customerCategories = entities[i].GetColumnValue("StCustomerCategory");
                    var isSkip = entities[i].GetColumnValue("StIsSkipPayment");
                    var isNormal = entities[i].GetColumnValue("StIsNormal");

                    var customerCategoryCatalogItem = new CustomerCategoryCatalog()
                    {
                        Name = name.ToString(),
                        SegmentId = customerSegmentId.ToString(),
                        CustomerCategory = customerCategories.ToString(),
                        IsNormal = isNormal.ToString(),
                        IsSkipPayment = isSkip.ToString()
                    };
                customerCategoryCatalogItems.Add(customerCategoryCatalogItem);
                }
            return customerCategoryCatalogItems;
        }

        public List<CustomerCategoryInfo> GetCustomerCategoryDetail(string catalogId)
        {
            List<string> customerCategoryids = new List<string>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
            var ccId = esq.AddColumn("StCustomerCategoryID");
            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryCatalog", catalogId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    customerCategoryids.Add(item.GetTypedColumnValue<string>(ccId.Name));
                }
            }


            var customerCategoryInfoItems = new List<CustomerCategoryInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<CustomerCategory> items = client.Get<List<CustomerCategory>>(String.Format("api/SOM.ST/Billing/GetCustomerCategories"));
                foreach (var item in items)
                {
                    var customerCatergoryInfoItem = CustomerCategoryToInfoMapper(item);
                    customerCategoryInfoItems.Add(customerCatergoryInfoItem);
                }
            }

            var categories = new List<CustomerCategoryInfo>();
            categories = customerCategoryInfoItems.Where(p => !customerCategoryids.Any(p2 => p2.ToString() == p.CategoryId.ToString())).ToList();
            return categories;


        }

        public CustomerCategoryInfo CustomerCategoryToInfoMapper(CustomerCategory item)
        {
            return new CustomerCategoryInfo
            {
                Name = item.Description,
                CategoryId = item.Id

            };
        }
    }
}
