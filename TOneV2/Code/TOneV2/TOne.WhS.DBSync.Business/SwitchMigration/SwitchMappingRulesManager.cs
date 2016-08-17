using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchMappingRulesManager
    {
        private string ConnectionString { get; set; }

        public SwitchMappingRulesManager(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<SwitchMappingRules> LoadSwitches()
        {
            SwtichMigrationDataManager dataManager = new SwtichMigrationDataManager(ConnectionString);
            return dataManager.LoadSwitches();
        }
    }
}
