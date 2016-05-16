using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class Migrator
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public DBSyncLogger Logger { get; set; }
        public abstract void Migrate(List<DBTable> context);

        protected Migrator(string connectionString, bool useTempTables, DBSyncLogger logger)
        {
            UseTempTables = useTempTables;
            Logger = logger;
            ConnectionString = connectionString;
        }

    }
}
