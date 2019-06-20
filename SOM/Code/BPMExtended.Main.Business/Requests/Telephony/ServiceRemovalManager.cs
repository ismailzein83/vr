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
    public class ServiceRemovalManager
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
        public List<ContractAvailableServiceOutput> GetContractServicesWithExcludedPackages(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<ContractAvailableServiceOutput> items = new List<ContractAvailableServiceOutput>();
            List<string> packagesIds = new List<string>();


            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            packagesIds.Add(packages.Core);
            packagesIds.Add(packages.Telephony);


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceRemoval");
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");          
                var somRequestInput = new ServiceRemovalContractServicesInput()
                {
                    ContractId = contractId.ToString(),
                    ExcludedPackages = packagesIds
                };

                //call api
                using (var client = new SOMClient())
                {
                    items = client.Post<ServiceRemovalContractServicesInput, List<ContractAvailableServiceOutput>>("api/SOM.ST/Billing/GetContractAvailableServices", somRequestInput);
                }

            }


            return items;

        }

        public void SubmitServiceRemovalToOM(Guid requestid)
        {

        }

        public void ActivateServiceRemovalToOM(Guid requestid)
        {

        }

        #endregion
    }
}
