using Microsoft.SqlServer;
using Microsoft.SqlServer.Common;
using Microsoft.SqlServer.Management.Adapters;
using Microsoft.SqlServer.Management.Collector;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk;
using Microsoft.SqlServer.Management.Sdk.Differencing;
using Microsoft.SqlServer.Management.Sdk.Differencing.Impl;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Broker;
using Microsoft.SqlServer.Management.Smo.Internal;
using Microsoft.SqlServer.Management.Smo.Mail;
using Microsoft.SqlServer.Management.Smo.RegisteredServers;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;
using Microsoft.SqlServer.Management.Smo.SqlEnum;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Microsoft.SqlServer.Management.Trace;
using Microsoft.SqlServer.Management.Utility;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {

        //private static Column CopyColumn(Table parent, Column sourceColumn)
        //{
        //    Column destinationColumn = new Column(parent, sourceColumn.Name, sourceColumn.DataType);
        //    if (sourceColumn.Computed)
        //    {
        //        destinationColumn.Computed = true;
        //        destinationColumn.ComputedText = sourceColumn.ComputedText;
        //    }
        //    if (sourceColumn.Identity)
        //    {
        //        destinationColumn.Identity = true;
        //        destinationColumn.IdentityIncrement = sourceColumn.IdentityIncrement;
        //        destinationColumn.IdentitySeed = sourceColumn.IdentitySeed;
        //    }
        //    destinationColumn.Default = sourceColumn.Default;
        //    destinationColumn.Nullable = sourceColumn.Nullable;
        //    return destinationColumn;
        //}
        //private static Index CopyIndex(Table parent, Index sourceIndex, string sourceTableName)
        //{
        //    Index destinationIndex = new Index(parent, sourceIndex.Name.Replace(sourceTableName, parent.Name));
        //    destinationIndex.IndexKeyType = sourceIndex.IndexKeyType;
        //    foreach (IndexedColumn sourceIndexedCol in sourceIndex.IndexedColumns)
        //    {
        //        destinationIndex.IndexedColumns.Add(new IndexedColumn(destinationIndex, sourceIndexedCol.Name, sourceIndexedCol.Descending));
        //    }
        //    return destinationIndex;
        //}
        //private static ForeignKey CopyForeignKeys(Table parent, ForeignKey sourceForeignKey, string sourceTableName)
        //{
        //    ForeignKey destinationForeignKey = new ForeignKey(parent, sourceForeignKey.Name.Replace(sourceTableName, parent.Name));
        //    foreach (ForeignKeyColumn fkSourceCol in sourceForeignKey.Columns)
        //        destinationForeignKey.Columns.Add(new ForeignKeyColumn(destinationForeignKey, fkSourceCol.Name, fkSourceCol.ReferencedColumn));
        //    destinationForeignKey.DeleteAction = sourceForeignKey.DeleteAction;
        //    destinationForeignKey.IsChecked = sourceForeignKey.IsChecked;
        //    destinationForeignKey.IsEnabled = sourceForeignKey.IsEnabled;
        //    destinationForeignKey.ReferencedTable = sourceForeignKey.ReferencedTable;
        //    destinationForeignKey.ReferencedTableSchema = sourceForeignKey.ReferencedTableSchema;
        //    destinationForeignKey.UpdateAction = sourceForeignKey.UpdateAction;
        //    return destinationForeignKey;
        //}




        //public void Execute()
        //{
        //    //////////var switches = new List<TOne.WhS.BusinessEntity.Entities.Switch>();
        //    //////////switches.Add(new TOne.WhS.BusinessEntity.Entities.Switch { Name = "Switch 1", SwitchId = 4 });
        //    //////////TOne.WhS.DBSync.Data.SQL.SwitchDataManager switchManager = new TOne.WhS.DBSync.Data.SQL.SwitchDataManager();

        //    //////////switchManager.MigrateSwitchesToDB(switches);

        //    //////////BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
        //    //////////QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

        //    ////////var runtimeServices = new List<RuntimeService>();
        //    //////////runtimeServices.Add(queueActivationService);

        //    //////////runtimeServices.Add(bpService);
        //    ////////SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
        //    ////////runtimeServices.Add(schedulerService);

        //    ////////RuntimeHost host = new RuntimeHost(runtimeServices);
        //    ////////host.Start();



        //    string connectionString = "Server=192.168.110.185;Database=MVTSPro;User ID=development;Password=dev!123";



        //    //// Create Source Switch Reader and Pass Connection string to it.
        //    //SourceSwitchMigratorReader sourceSwitchMigratorReader = new SourceSwitchMigratorReader();
        //    //sourceSwitchMigratorReader.ConnectionString = connectionString;

        //    //// Create Source Switch Migrator and Migrate
        //    //SourceSwitchMigrator sourceSwitchMigrator = new SourceSwitchMigrator(sourceSwitchMigratorReader);
        //    //sourceSwitchMigrator.Migrate();




        //    //// Create Source Currency Reader and Pass Connection string to it.
        //    //SourceCurrencyMigratorReader sourceCurrencyMigratorReader = new SourceCurrencyMigratorReader();
        //    //sourceCurrencyMigratorReader.ConnectionString = connectionString;

        //    //// Create Source Currency Migrator and Migrate
        //    //SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(sourceCurrencyMigratorReader);
        //    //sourceCurrencyMigrator.Migrate();




        //    //// Create Source CurrencyExchangeRate Reader and Pass Connection string to it.
        //    //SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader = new SourceCurrencyExchangeRateMigratorReader();
        //    //sourceCurrencyExchangeRateMigratorReader.ConnectionString = connectionString;

        //    //// Create Source CurrencyExchangeRate Migrator and Migrate
        //    //SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(sourceCurrencyExchangeRateMigratorReader);
        //    //sourceCurrencyExchangeRateMigrator.Migrate();



        //    string serverstr = "192.168.110.185";
        //    string user = "development";
        //    string password = "dev!123";
        //    ServerConnection conn = new ServerConnection(serverstr, user, password);
        //    try
        //    {



        //        Server server = new Server(conn);
        //        foreach (Database database in server.Databases)
        //        {
        //            if (database.Name == "TOneV2_Migration")
        //            {
        //                Table destinationTable = new Table();
        //                Table table = new Table();

        //                foreach (Column col in table.Columns)
        //                    destinationTable.Columns.Add(CopyColumn(destinationTable, col));


        //                foreach (Index index in table.Indexes)
        //                    destinationTable.Indexes.Add(CopyIndex(destinationTable, index, table.Name));


        //                foreach (ForeignKey fk in table.ForeignKeys)
        //                    destinationTable.ForeignKeys.Add(CopyForeignKeys(destinationTable, fk, table.Name));


        //                ScriptingOptions tableOptions = new Microsoft.SqlServer.Management.Smo.ScriptingOptions();
        //                tableOptions.Indexes = false;
        //                tableOptions.NoCollation = true;



        //                Console.WriteLine("------- Create Table Statement ---------------");
        //                System.Collections.Specialized.StringCollection sc = table.Script(tableOptions);
        //                foreach (string st in sc)
        //                {
        //                    Console.WriteLine(st);
        //                }

        //                table.Drop();
        //                table.Create();



        //                Console.WriteLine("----------------------------------------------");





        //                Console.WriteLine("------- Drop and Create PK Statement ---------------");

        //                // object to hold the index script
        //                StringCollection pk_script = new StringCollection();

        //                Index pk = table.Indexes.Cast<Index>().SingleOrDefault(index => index.IndexKeyType == IndexKeyType.DriPrimaryKey);
        //                if (pk != null)
        //                {
        //                    // script the index
        //                    pk_script = pk.Script();
        //                    pk.Drop();
        //                    table.Alter();
        //                }

        //                // iterate through script StringCollection
        //                foreach (String tsql in pk_script)
        //                {
        //                    database.ExecuteNonQuery(tsql);
        //                    Console.WriteLine(tsql);
        //                }
        //                Console.WriteLine("----------------------------------------------");




        //                Console.WriteLine("------- Drop and Create FK Statement ---------------");

        //                // object to hold the index script
        //                StringCollection fk_script = new StringCollection();

        //                ForeignKeyCollection fks = table.ForeignKeys;
        //                IndexCollection indexes = table.Indexes;
        //                Index primaryKeys = table.Indexes.Cast<Index>().SingleOrDefault(x => x.IndexKeyType == IndexKeyType.DriPrimaryKey);
        //                fks.drop














        //                //Connect to the local, default instance of SQL Server. 
        //                Server srv;
        //                srv = new Server();
        //                //Reference the AdventureWorks2012 database. 
        //                Database db;
        //                db = srv.Databases["AdventureWorks2012"];
        //                //Declare another Table object variable and reference the EmployeeDepartmentHistory table. 
        //                Table tbea;
        //                tbea = db.Tables["EmployeeDepartmentHistory", "HumanResources"];
        //                //Define a Foreign Key object variable by supplying the EmployeeDepartmentHistory as the parent table and the foreign key name in the constructor. 
        //                ForeignKey fk;
        //                fk = new ForeignKey(tbea, "test_foreignkey");
        //                //Add BusinessEntityID as the foreign key column. 
        //                ForeignKeyColumn fkc;
        //                fkc = new ForeignKeyColumn(fk, "BusinessEntityID", "BusinessEntityID");
        //                fk.Columns.Add(fkc);
        //                //Set the referenced table and schema. 
        //                fk.ReferencedTable = "Employee";
        //                fk.ReferencedTableSchema = "HumanResources";
        //                //Create the foreign key on the instance of SQL Server. 
        //                fk.Create();





        //                //foreach (var i in fks)
        //                //{
        //                //    i.Drop()
        //                //}

        //                if (fk != null)
        //                {
        //                    // script the index
        //                    fk_script = fk.Script();
        //                    fk.Drop();
        //                    table.Alter();
        //                }

        //                // iterate through script StringCollection
        //                foreach (String tsql in fk_script)
        //                {
        //                    database.ExecuteNonQuery(tsql);
        //                    Console.WriteLine(tsql);
        //                }
        //                Console.WriteLine("----------------------------------------------");




        //            }


        //        }
        //    }
        //    catch (Exception err)
        //    {

        //    }





        //}


        private static Table createTempTable(Table sourcetable)
        {
            Database db = sourcetable.Parent;
            string schema = sourcetable.Schema;
            Table copiedtable = new Table(db, sourcetable.Name + "Temp", schema);
            Server server = sourcetable.Parent.Parent;

            createColumns(sourcetable, copiedtable);

            copiedtable.AnsiNullsStatus = sourcetable.AnsiNullsStatus;
            copiedtable.QuotedIdentifierStatus = sourcetable.QuotedIdentifierStatus;
            copiedtable.TextFileGroup = sourcetable.TextFileGroup;
            copiedtable.FileGroup = sourcetable.FileGroup;


            Table oldTable = server.Databases["TOne"].Tables["HospitalsTemp", "dbo"];
            if (oldTable != null)
                oldTable.Drop();


            copiedtable.Create();







            return copiedtable;
        }

        private static void createColumns(Table sourcetable, Table copiedtable)
        {
            Server server = sourcetable.Parent.Parent;

            foreach (Column source in sourcetable.Columns)
            {
                Column column = new Column(copiedtable, source.Name, source.DataType);
                column.Collation = source.Collation;
                column.Nullable = source.Nullable;
                column.Computed = source.Computed;
                column.ComputedText = source.ComputedText;
                column.Default = source.Default;

                if (source.DefaultConstraint != null)
                {
                    string tabname = copiedtable.Name;
                    string constrname = source.DefaultConstraint.Name;
                    column.AddDefaultConstraint(tabname + "_" + constrname);
                    column.DefaultConstraint.Text = source.DefaultConstraint.Text;
                }

                column.IsPersisted = source.IsPersisted;
                column.DefaultSchema = source.DefaultSchema;
                column.RowGuidCol = source.RowGuidCol;

                if (server.VersionMajor >= 10)
                {
                    column.IsFileStream = source.IsFileStream;
                    column.IsSparse = source.IsSparse;
                    column.IsColumnSet = source.IsColumnSet;
                }

                copiedtable.Columns.Add(column);
            }
        }

        public void Execute()
        {
            string connString = "server=192.168.110.185;database=TOneV2_Migration;uid=development;password=dev!123;";

            ServerConnection serverConnection = null;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                try
                {
                    serverConnection = new ServerConnection(sqlConnection);
                    Server server = new Server(serverConnection);
                    serverConnection.BeginTransaction();
                    Table sourceTable = server.Databases["TOneV2_Migration"].Tables["Country", "Common"];
                    IndexCollection indexes = sourceTable.Indexes;
                    ForeignKeyCollection foreignKeys = sourceTable.ForeignKeys;
                    createTempTable(sourceTable);
                    serverConnection.CommitTransaction();
                }
                catch
                {
                    serverConnection.RollBackTransaction();
                }
                finally
                {
                    serverConnection.Disconnect();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }




            //string serverstr = "192.168.110.185";
            //string user = "development";
            //string password = "dev!123";
            //ServerConnection conn = new ServerConnection(serverstr, user, password);
            //try
            //{
            //    Server server = new Server(conn);

            //    Database database = server.Databases.Cast<Database>().Where(x => x.Name == "TOneV2_Migration").FirstOrDefault();

            //    if (database != null)
            //    {
            //        Table table = database.Tables.Cast<Table>().Where(x => x.Name == "Country").FirstOrDefault();
            //        var foreignKeys = table.ForeignKeys;
            //        var indexes = table.Indexes;


            //        foreach (ForeignKey fk in foreignKeys)
            //            table.ForeignKeys.Remove(fk);

            //        foreach (Index index in indexes)
            //            table.Indexes.Remove(index);

            //        table.Name = table.Name + "Temp";

            //        table.Drop();

            //        table.Create();

            //        table.Rename(table.Name.Replace("Temp", ""));


            //        foreach (ForeignKey fk in foreignKeys)
            //            table.ForeignKeys.Add(fk);

            //        foreach (Index index in indexes)
            //            table.Indexes.Add(index);


            //        table.Alter();
            //    }

            //}
            //catch
            //{

            //}


        }



    }
}




