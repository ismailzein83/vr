using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CatalogManager
    {

        //public UserConnection BPM_UserConnection
        //{
        //    get
        //    {
        //        return (UserConnection)HttpContext.Current.Session["UserConnection"];
        //    }
        //}

        //public OperationServices GetOperationServices(Guid requestId)
        //{
        //    EntitySchemaQuery esq;
        //    IEntitySchemaQueryFilterItem esqFirstFilter;
        //    EntityCollection entities;
        //    string requestType;
        //    string catalogId;
        //    List<ServicePayment> fees = new List<ServicePayment>();
        //    List<ServicePayment> services = new List<ServicePayment>();


        //    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
        //    esq.AddColumn("Id");
        //    esq.AddColumn("StRequestType");
        //    esq.AddColumn("StRequestId");


        //    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);


        //    esq.Filters.Add(esqFirstFilter);

        //    entities = esq.GetEntityCollection(BPM_UserConnection);
        //    if (entities.Count > 0)
        //    {
        //        requestType = entities[0].GetTypedColumnValue<string>("StRequestType");

        //        esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPOSServiceOperationCatalog");
        //        var catId = esq.AddColumn("Id");
        //        esq.AddColumn("StOperationTypeId");


        //        esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationTypeId", requestType);
        //        esq.Filters.Add(esqFirstFilter);

        //        entities = esq.GetEntityCollection(BPM_UserConnection);
        //        if (entities.Count > 0)
        //        {
        //            //catalogId = entities[0].GetTypedColumnValue<Guid>("Id");
        //            catalogId = entities[0].GetTypedColumnValue<string>(catId.Name);

        //            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StPOSServiceInCatalog");
        //            esq.AddColumn("Id");
        //            esq.AddColumn("StPOSServiceName");
        //            esq.AddColumn("StPOSServiceID");
        //            esq.AddColumn("StUpFront");


        //            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StPOSServiceCatalog", catalogId);
        //            esq.Filters.Add(esqFirstFilter);

        //            entities = esq.GetEntityCollection(BPM_UserConnection);
        //            foreach (Entity entity in entities)
        //            {
        //                fees.Add(new ServicePayment()
        //                {
        //                    Id = (string)entity.GetColumnValue("StPOSServiceID"),
        //                    UpFront = (bool)entity.GetColumnValue("StAppFront"),

        //                });

        //            }


        //            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceInOperationCatalog");
        //            esq.AddColumn("Id");
        //            esq.AddColumn("StServiceName");
        //            esq.AddColumn("StServiceID");


        //            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StPOSServiceCatalog", catalogId);
        //            esq.Filters.Add(esqFirstFilter);

        //            entities = esq.GetEntityCollection(BPM_UserConnection);
        //            foreach (Entity entity in entities)
        //            {
        //                services.Add(new ServicePayment()
        //                {
        //                    Id = (string)entity.GetColumnValue("StServiceID"),
        //                    UpFront = true

        //                });

        //            }


        //        }

        //    }


        //    return new OperationServices()
        //    {
        //        Fees = fees,
        //        Services = services
        //    };

        //}
    }
}
