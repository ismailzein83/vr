using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            string ConnectionString = "Server=192.168.110.185;Database=MVTSPro;User ID=development;Password=dev!123";

            Console.WriteLine("Database Sync Task Action Started");

            List<DBTable> context = new List<DBTable>();
            context.Add(new DBTable { Name = "CurrencyExchangeRate", Schema = "Common", Database = "TOneConfiguration_Migration" });
            context.Add(new DBTable { Name = "Currency", Schema = "Common", Database = "TOneConfiguration_Migration" });
            context.Add(new DBTable { Name = "Switch", Schema = "TOneWhS_BE", Database = "TOneV2_Migration" });

            MigrationManager.MigrationCredentials migrationCredentials = new MigrationManager.MigrationCredentials();
            migrationCredentials.MigrationServer = ConfigurationManager.AppSettings["MigrationServer"];
            migrationCredentials.MigrationServerUserID = ConfigurationManager.AppSettings["MigrationServerUserID"];
            migrationCredentials.MigrationServerPassword = ConfigurationManager.AppSettings["MigrationServerPassword"];

            MigrationManager migrationManager = new MigrationManager(migrationCredentials, context);
            migrationManager.PrepareBeforeApplyingRecords();

            SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(new SourceCurrencyMigratorReader(ConnectionString));
            sourceCurrencyMigrator.Migrate(context);

            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(new SourceCurrencyExchangeRateMigratorReader(ConnectionString));
            sourceCurrencyExchangeRateMigrator.Migrate(context);

            migrationManager.FinalizeMigration();
        }
    }
}




