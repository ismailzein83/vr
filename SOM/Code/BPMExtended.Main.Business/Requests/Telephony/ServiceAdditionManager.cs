using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ServiceAdditionManager
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

        public List<ContractAvailableServiceOutput> GetContractAvailableServices(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<ContractAvailableServiceOutput> items = new List<ContractAvailableServiceOutput>();
            List<ContractAvailableServiceOutput> filteredServices = new List<ContractAvailableServiceOutput>();
            List<string> packagesIds = new List<string>();


            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            packagesIds.Add(packages.Core);
            packagesIds.Add(packages.Telephony);


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceAdditionRequest");
            esq.AddColumn("StPathId");
            esq.AddColumn("StContractId");
            esq.AddColumn("StRatePlanId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StPathId");
                var contractId = entities[0].GetColumnValue("StContractId");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");

                var somRequestInput = new ContractAvailableServicesInput()
                {
                    ContractId = contractId.ToString(),
                    RatePlanId = ratePlanId.ToString(),//"TM005",
                    LinePathId = pathId.ToString(),//"184",
                    ExcludedPackages = packagesIds
                };


                //call api
                using (var client = new SOMClient())
                {
                    items = client.Post<ContractAvailableServicesInput, List<ContractAvailableServiceOutput>>("api/SOM.ST/Billing/GetContractAvailableServices", somRequestInput);
                }

            }

            //Get special services from service definition catalog 
            List<string> specialServicesIds = new CatalogManager().GetSpecialServicesIds();

            //filter the ContractAvailableServices (ContractAvailableServices - special services)
            filteredServices = (from item in items
                                where !specialServicesIds.Contains(item.Id)
                                select item).ToList();



            return filteredServices;

        }

        #endregion

    }
}
