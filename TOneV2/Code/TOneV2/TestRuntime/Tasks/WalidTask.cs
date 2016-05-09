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

        const string _Temp = "_Temp";


        private static string ScriptIndexes(Table sourceTable)
        {
            string script = string.Empty;

            foreach (Index index in sourceTable.Indexes)
            {
                StringCollection sc = index.Script();
                string[] strings = new string[sc.Count];
                sc.CopyTo(strings, 0);
                script = script + string.Join(" ", strings);
            }
            return script;
        }


        private static string ScriptFKs(Table sourceTable)
        {
            string script = string.Empty;

            foreach (ForeignKey fk in sourceTable.ForeignKeys)
            {
                StringCollection sc = fk.Script();
                string[] strings = new string[sc.Count];
                sc.CopyTo(strings, 0);
                script = script + string.Join(" ", strings);
            }
            return script;
        }


        private static string ScriptTempTable(Table sourceTable)
        {
            ScriptingOptions so = new ScriptingOptions();
            so.DriAllKeys = false;
            string script = string.Empty;
            StringCollection sc = sourceTable.Script(so);
            string[] strings = new string[sc.Count];
            sc.CopyTo(strings, 0);
            script = script + string.Join(" ", strings);
            script = script.Replace("[" + sourceTable.Schema + "].[" + sourceTable.Name + "]", "[" + sourceTable.Schema + "].[" + sourceTable.Name + _Temp + "]");
            return script;
        }

        class DBForeignKey
        {
            public string ReferencedKey { get; set; }
            public string ReferencedTable { get; set; }
            public string ReferencedTableSchema { get; set; }
        }

        class DBTable
        {
            public string Name { get; set; }
            public string Schema { get; set; }
            public string Database { get; set; }
            public string ScriptedIndexes { get; set; }
            public string ScriptedFKs { get; set; }
            public string ScriptedTempTable { get; set; }
            public List<DBForeignKey> DBFKs { get; set; }
            public Table Info { get; set; }

            public bool DroppedOriginal { get; set; }
        }

        public void Execute()
        {
            List<DBTable> dbTables = new List<DBTable>();
            string connString = "server=192.168.110.185;database=TOneV2_Migration;uid=development;password=dev!123;";

            ServerConnection serverConnection = null;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                try
                {
                    serverConnection = new ServerConnection(sqlConnection);
                    Server server = new Server(serverConnection);
                    serverConnection.BeginTransaction();

                    DefineTables(dbTables, server);
                    CreateTempTables(dbTables, sqlConnection);
                    FillTables(dbTables);
                    DropOriginalTables(dbTables);

                    serverConnection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    serverConnection.RollBackTransaction();
                    throw ex;
                }
            }


            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                try
                {
                    serverConnection = new ServerConnection(sqlConnection);
                    Server server = new Server(serverConnection);
                    serverConnection.BeginTransaction();

                    RenameTempTables(dbTables, server);
                    CreateIndexes(dbTables, sqlConnection);
                    CreateForeignKeys(dbTables, sqlConnection);

                    serverConnection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    serverConnection.RollBackTransaction();
                    throw ex;
                }
            }

        }

        private static void DropOriginalTables(List<DBTable> dbTables)
        {
            //Set Priority by References
            foreach (DBTable table in dbTables)
            {
                table.DBFKs = new List<DBForeignKey>();
                foreach (ForeignKey fk in table.Info.ForeignKeys)
                {
                    table.DBFKs.Add(new DBForeignKey { ReferencedKey = fk.ReferencedKey, ReferencedTable = fk.ReferencedTable, ReferencedTableSchema = fk.ReferencedTableSchema });
                }
            }
            DropTables(dbTables);
        }

        private static void CreateForeignKeys(List<DBTable> dbTables, SqlConnection sqlConnection)
        {
            //Create Foreign Keys for Tables
            foreach (DBTable table in dbTables)
                ExecuteSql.ExecuteImmediate(table.ScriptedFKs, sqlConnection);
        }

        private static void CreateIndexes(List<DBTable> dbTables, SqlConnection sqlConnection)
        {
            //Create Indexes Keys for Tables
            foreach (DBTable table in dbTables)
                ExecuteSql.ExecuteImmediate(table.ScriptedIndexes, sqlConnection);
        }

        private static void RenameTempTables(List<DBTable> dbTables, Server server)
        {
            foreach (DBTable dbTable in dbTables)
            {
                DBTable dbTempTable = new DBTable();
                dbTempTable.Name = dbTable.Name + _Temp;
                dbTempTable.Schema = dbTable.Schema;
                dbTempTable.Database = dbTable.Database;

                Table tempTable = server.Databases[dbTempTable.Database].Tables[dbTempTable.Name, dbTempTable.Schema];
                tempTable.Rename(tempTable.Name.Replace(_Temp, ""));
            }
        }

        private static void FillTables(List<DBTable> dbTables)
        {
            //Fill Temp Tables
            foreach (DBTable table in dbTables)
            {

            }
        }


        private static void CreateTempTables(List<DBTable> dbTables, SqlConnection sqlConnection)
        {
            // Create Temp Tables
            foreach (DBTable table in dbTables)
            {
                ExecuteSql.ExecuteImmediate(table.ScriptedTempTable, sqlConnection);
            }
        }

        private static void DefineTables(List<DBTable> dbTables, Server server)
        {
            dbTables.Add(new DBTable { Name = "CurrencyExchangeRate", Schema = "Common", Database = "TOneV2_Migration" });
            dbTables.Add(new DBTable { Name = "Currency", Schema = "Common", Database = "TOneV2_Migration" });
            dbTables.Add(new DBTable { Name = "CurrencyExchangeRateZone", Schema = "Common", Database = "TOneV2_Migration" });

            foreach (DBTable table in dbTables)
            {
                Table sourceTable = GetTableReference(server, table);
                table.Info = sourceTable;
                table.ScriptedIndexes = ScriptIndexes(sourceTable);
                table.ScriptedFKs = ScriptFKs(sourceTable);
                table.ScriptedTempTable = ScriptTempTable(sourceTable);
            }
        }

        private static void DropTables(List<DBTable> dbTables)
        {
            bool hasUnDropped = dbTables.Exists(x => x.DroppedOriginal == false);
            if (hasUnDropped)
            {
                // Drop Original Tables
                foreach (DBTable table in dbTables)
                {
                    bool isReferenced = false;

                    foreach (DBTable otherTable in dbTables.Where(x => x.Name != table.Name))
                    {
                        if (otherTable.DBFKs.Exists(x => x.ReferencedTable == table.Name && x.ReferencedTableSchema == table.Schema))
                        {
                            isReferenced = true;
                            break;
                        }
                    }


                    if (!isReferenced)
                    {
                        table.Info.Drop();
                        table.DroppedOriginal = true;
                    }
                }
                DropTables(dbTables.Where(x => x.DroppedOriginal == false).ToList());
            }
        }

        private static Table GetTableReference(Server server, DBTable dbTable)
        {
            Table sourceTable = server.Databases[dbTable.Database].Tables[dbTable.Name, dbTable.Schema];
            return sourceTable;
        }


    }
}




