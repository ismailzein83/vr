using NP.IVSwitch.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.IVSwitch;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class CDRDataManager : BasePostgresDataManager, ICDRDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.CdrConnectionString;
        }

        public bool InsertHelperUser(int accountId, string logAlias)
        {
            string query = @"INSERT INTO ui_helper_accounts(
	                            account_id, log_alias)
	                            SELECT @account_id, @log_alias WHERE NOT EXISTS 
                                ( SELECT 1 FROM ui_helper_accounts WHERE (account_id = @account_id AND log_alias = @log_alias))";
            int recordsEffected = ExecuteNonQueryText(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.Parameters.AddWithValue("@log_alias", logAlias);
            });
            return recordsEffected > 0;
        }

        public IEnumerable<Entities.LiveCdrItem> GetFilteredLiveCdrs(List<int> endPointIds, List<int> routeIds, string sourceIP, string routeIP, CallsMode callsMode, TimeUnit timeUnit, double time)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                                SELECT user_id,src_ip,det_date,cli,dest_code,dest_name,route_id,route_ip,route_dest_code,route_dest_name,
                                prg_date,con_date,case when con_date is not null then extract(epoch from now()-con_date) else 0 end FROM cdrs_buffer WHERE 1=1 ");
            if (endPointIds != null && endPointIds.Count() > 0)
            {
                string ePIds = null;
                ePIds = string.Join<int>(",", endPointIds);
                queryBuilder.Append(string.Format(" AND user_id in ({0})", ePIds));
            }
            if (routeIds != null && routeIds.Count() > 0)
            {
                string rtIds = null;
                rtIds = string.Join<int>(",", routeIds);
                queryBuilder.Append(string.Format(" AND route_id in ({0})",rtIds));
            }
            if (sourceIP != null)
            {
                queryBuilder.Append(string.Format(" AND src_ip like '%{0}%' ", sourceIP));
            }
            if (routeIP != null)
            {
                queryBuilder.Append(string.Format(" AND route_ip like '%{0}%' ", routeIP));
            }
            if (callsMode != CallsMode.None && timeUnit != TimeUnit.None && time != 0)
            {
                double compareTime=0;
                if (timeUnit == TimeUnit.Minutes)
                {
                    compareTime = time * 60;
                }
                else {
                    compareTime = time;
                }
                if (callsMode == CallsMode.OlderThem)
                {
                    queryBuilder.Append(string.Format(" AND det_date < (timestamp 'now' - interval '{0} seconds')", compareTime));
                }
                else
                {
                    queryBuilder.Append(string.Format(" AND det_date >= (timestamp 'now' - interval '{0} seconds')", compareTime));
                }
            }
            return GetItemsText(queryBuilder.ToString(), LiveCdrMapper, (cmd) =>
            {
            });
        }


        private LiveCdrItem LiveCdrMapper(IDataReader reader)
        {
            LiveCdrItem liveCdrItem = new LiveCdrItem
            {
                customerId = (int)reader["user_id"],
                sourceIP = reader["src_ip"] as string,
                attemptDate = (DateTime)reader["det_date"],
                cli = reader["cli"] as string,
                destinationCode = reader["dest_code"] as string,
                destinationName = reader["dest_name"] as string,
                routeId = (int)reader["route_id"],
                routeIP = reader["route_ip"] as string,
                supplierCode = reader["route_dest_code"] as string,
                supplierZone = reader["route_dest_name"] as string,
                alertDate = GetReaderValue<DateTime>(reader, "prg_date"),
                connectDate = GetReaderValue<DateTime>(reader, "con_date"),
                duration = GetReaderValue<double>(reader, "case")
            };

            return liveCdrItem;
        }
    }
}
