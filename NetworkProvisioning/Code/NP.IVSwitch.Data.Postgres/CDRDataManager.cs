using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class CDRDataManager : BasePostgresDataManager, ICDRDataManager
    {
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
        protected override string GetConnectionString()
        {
            return IvSwitchSync.CdrConnectionString;
        }
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
    }
}
