using System;
using NP.IVSwitch.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class FirewallDataManager : BasePostgresDataManager, IFirewallDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
        }

        public List<Firewall> GetFirewalls()
        {
            string cmdText = @"SELECT host, description, create_date, rec_id
                                FROM fw_hosts;  ";
            return GetItemsText(cmdText, FirewallMapper, null);
        }

        private Firewall FirewallMapper(IDataReader reader)
        {
            Firewall firewall = new Firewall
            {
                Description = reader["description"] as string,
                CreationDate = (DateTime)reader["create_date"],
                RecId = (int)reader["rec_id"]

            };
            System.Net.IPAddress host = GetReaderValue<System.Net.IPAddress>(reader, "host");
            firewall.Host = (host == null) ? null : host.ToString();
            return firewall;
        }
    }
}
