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
                Id = (int)reader["rec_id"]

            };
            System.Net.IPAddress host = GetReaderValue<System.Net.IPAddress>(reader, "host");
            firewall.Host = (host == null) ? null : host.ToString();
            return firewall;
        }


        public bool Update(Firewall firewall)
        {
            String cmdText = @"UPDATE fw_hosts
	                             SET host = @host, description = @description 
                                 WHERE  rec_id = @recId and  NOT EXISTS(SELECT 1 FROM  fw_hosts WHERE rec_id != @recId and host = @host);";

            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@recId", firewall.Id);
                cmd.Parameters.AddWithValue("@host", System.Net.IPAddress.Parse(firewall.Host));
                cmd.Parameters.AddWithValue("@description", firewall.Description);
            }
                );
            return (recordsEffected > 0);
        }

        public bool Insert(Firewall firewall, out int insertedId)
        {
            String cmdText = @"INSERT INTO fw_hosts(host, description)
	                             SELECT @host,@description
	                             WHERE (NOT EXISTS(SELECT 1 FROM fw_hosts WHERE  host = @host))
	                             returning  rec_id;";

            var recId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@host", System.Net.IPAddress.Parse(firewall.Host));
                cmd.Parameters.AddWithValue("@description", firewall.Description);
            }
                );
            insertedId = -1;
            if (recId == null) return false;
            insertedId = Convert.ToInt32(recId);
            return true;
        }
    }
}
