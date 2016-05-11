using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Data.SQL;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            //string ConnectionStringTOneV2 = "Server=192.168.110.185;Database=TOneV2_Migration;User ID=development;Password=dev!123";

            MigrationManager migrationManager = new MigrationManager();
            migrationManager.ExecuteMigrationPhase1();

            string ConnectionStringTOneV1 = "Server=192.168.110.185;Database=MVTSPro;User ID=development;Password=dev!123";


            // Create Source Currency Reader and Pass Connection string to it.
            SourceCurrencyMigratorReader sourceCurrencyMigratorReader = new SourceCurrencyMigratorReader();
            sourceCurrencyMigratorReader.ConnectionString = ConnectionStringTOneV1;

            // Create Source Currency Migrator and Migrate
            SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(sourceCurrencyMigratorReader);
            sourceCurrencyMigrator.Migrate();


            // Create Source CurrencyExchangeRate Reader and Pass Connection string to it.
            SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader = new SourceCurrencyExchangeRateMigratorReader();
            sourceCurrencyExchangeRateMigratorReader.ConnectionString = ConnectionStringTOneV1;

            // Create Source CurrencyExchangeRate Migrator and Migrate
            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(sourceCurrencyExchangeRateMigratorReader);
            sourceCurrencyExchangeRateMigrator.Migrate();

            migrationManager.ExecuteMigrationPhase2();

            migrationManager.ExecuteMigrationPhase3();

        }
    }
}




