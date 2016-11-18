using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class DomainDataManager: BasePostgresDataManager, IDomainDataManager
    {
        public DomainDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private Domain DomainMapper(IDataReader reader)
        {

            Domain domain = new Domain
            {

                DomainId = (Int16)reader["domain_id"],
                Description = reader["description"] as string,
            };

            return domain;
        }


        public List<Domain> GetDomains()
        {
            String cmdText = @"SELECT  domain_id, description            
                                       FROM domains;";
            return GetItemsText(cmdText, DomainMapper, (cmd) =>
            {
            });
        }

    }
}