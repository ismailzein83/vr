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
    public class TariffDataManager : BasePostgresDataManager, ITariffDataManager
    {
        public TariffDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private Tariff TariffMapper(IDataReader reader)
        {

            Tariff tariff = new Tariff
            {

                TariffId = (int)reader["tariff_id"],
                TariffName = reader["tariff_name"] as string,
            };

            return tariff;
        }


        public List<Tariff> GetTariffs()
        {
            String cmdText = @"SELECT  tariff_id, tariff_name            
                                       FROM tariffs;";
            return GetItemsText(cmdText, TariffMapper, (cmd) =>
            {
            });
        }

    }
}