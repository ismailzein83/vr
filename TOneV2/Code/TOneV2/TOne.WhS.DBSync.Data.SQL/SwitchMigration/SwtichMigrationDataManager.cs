using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwtichMigrationDataManager : BaseSQLDataManager
    {
        public string ConnectionString { get; set; }

        public SwtichMigrationDataManager(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected override string GetConnectionString()
        {
            return ConnectionString;
        }

        public List<SwitchMappingRules> LoadSwitches()
        {
            return GetItemsText(GetSwitchesQuery, SwitchMapper, null);
        }
        const string GetSwitchesQuery = @"select switchid,Name,Configuration from switch";

        #region mapper
        private SwitchMappingRules SwitchMapper(IDataReader arg)
        {
            SwitchMappingRules currentSwitch = new SwitchMappingRules
            {
                Id = int.Parse(arg["switchid"].ToString()),
                Name = arg["Name"] as string,
                Configuration = arg["Configuration"] as string

            };
            return currentSwitch;
        }
        #endregion
    }

}
