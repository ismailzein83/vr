using System;
using NP.IVSwitch.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.Postgres;
using Npgsql;

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
			int hostOrdinal = reader.GetOrdinal("host");

			Firewall firewall = new Firewall
            {
                Description = reader["description"] as string,
                CreationDate = (DateTime)reader["create_date"],
                Id = (int)reader["rec_id"]
			};
			NpgsqlDataReader npgsqlreader = (NpgsqlDataReader)reader;
			string hostObj = npgsqlreader.GetProviderSpecificValue(hostOrdinal).ToString();
			firewall.Host = hostObj;
			return firewall;
        }


        public bool Update(Firewall firewall)
        {
            string cmdText =string.Format(@"UPDATE fw_hosts
	                             SET host = '{0}', description = '{1}' 
                                 WHERE  rec_id = @recId and  NOT EXISTS(SELECT 1 FROM  fw_hosts WHERE rec_id != @recId and host = '{0}');", firewall.Host, firewall.Description);

            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@recId", firewall.Id);
            }
                );
            return (recordsEffected > 0);
        }
	
		public bool Insert(Firewall firewall, out int insertedId)
        {
            string cmdText =string.Format(@"INSERT INTO fw_hosts(host, description)
	                             SELECT '{0}','{1}'	
	       WHERE (NOT EXISTS(SELECT 1 FROM fw_hosts WHERE  host = '{0}'))
	                             returning  rec_id;", firewall.Host, firewall.Description);

            var recId = ExecuteScalarText(cmdText, (cmd) =>{});
            insertedId = -1;
            if (recId == null) return false;
            insertedId = Convert.ToInt32(recId);
            return true;
        }
    }
}
