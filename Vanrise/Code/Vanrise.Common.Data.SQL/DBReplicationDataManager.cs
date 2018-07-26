using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Linq;

namespace Vanrise.Common.Data.SQL
{
    public class DBReplicationDataManager : BaseSQLDataManager, IDBReplicationDataManager
    {
        Dictionary<string, List<TableInfo>> TableInfoListByTargetServer;
        const string TempTableNameSuffix = "_temp";

        #region public methods
        public void Initialise(IDBReplicationInitializeContext context)
        {
            if (context.DBReplicationTableDetailsListByTargetServer == null)
                context.DBReplicationTableDetailsListByTargetServer.ThrowIfNull("context.DBReplicationTableDetailsListBySourceConnection");

            TableInfoListByTargetServer = new Dictionary<string, List<TableInfo>>();

            foreach (var dbReplicationTableDetailsListKvp in context.DBReplicationTableDetailsListByTargetServer)
            {
                string targetServerConnectionString = dbReplicationTableDetailsListKvp.Key;
                SqlConnectionStringBuilder targetConnectionString = new SqlConnectionStringBuilder(targetServerConnectionString);
                string targetServerAddress = targetConnectionString.DataSource;

                Server targetServer = new Server(targetServerAddress);

                List<TableInfo> tableInfoList = TableInfoListByTargetServer.GetOrCreateItem(targetServerAddress);
                foreach (var dbReplicationTableDetails in dbReplicationTableDetailsListKvp.Value)
                {
                    var sourceConnectionString = ConfigurationManager.ConnectionStrings[dbReplicationTableDetails.SourceConnectionStringName];
                    SqlConnection sourceSQLConnection = new SqlConnection(sourceConnectionString.ConnectionString);
                    ServerConnection sourceServerConnection = new ServerConnection(sourceSQLConnection);
                    Server sourceServer = new Server(sourceServerConnection);

                    Table table = GetTableReference(sourceServer, sourceSQLConnection.Database, dbReplicationTableDetails.TableName, dbReplicationTableDetails.TableSchema);
                    TableInfo tableInfo = new TableInfo()
                    {
                        DBReplicationTableDetails = dbReplicationTableDetails,
                        SourceServer = sourceServer,
                        FKsScripts = ScriptAllFKs(table),
                        IndexesScripts = ScriptIndexes(table),
                        TargetServer = targetServer,
                        Table = table,
                        DataManager = new DBReplicationTableDataManager(sourceConnectionString.ConnectionString),
                        TargetDatabaseName = targetConnectionString.InitialCatalog,
                        TargetConnectionString = targetServerConnectionString
                    };

                    CreateTempTable(tableInfo);
                    tableInfoList.Add(tableInfo);
                    context.WriteInformation(string.Format("Temporary table {0} created", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));
                }
            }
        }

        public void MigrateData(IDBReplicationMigrateDataContext context)
        {
            foreach (var tableInfoListKvp in TableInfoListByTargetServer)
            {
                List<TableInfo> tableInfoList = tableInfoListKvp.Value;
                foreach (TableInfo tableInfo in tableInfoList)
                {
                    DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
                    context.WriteInformation(string.Format("Data Replication for table {0} started", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));

                    DBReplicationTableMigrateDataContext dbReplicationTableMigrateDataContext = BuildDBReplicationTableMigrateDataContext(context, tableInfo);
                    tableInfo.DataManager.MigrateData(dbReplicationTableMigrateDataContext);
                    context.WriteInformation(string.Format("Data Replication for table {0} done", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));
                }
            }
        }

        public void Finalize(IDBReplicationFinalizeContext context)
        {
            foreach (var tableInfoListKvp in TableInfoListByTargetServer)
            {
                List<TableInfo> tableInfoList = tableInfoListKvp.Value;
                ServerConnection targetServerConnection = tableInfoList.First().TargetServer.ConnectionContext;
                try
                {
                    targetServerConnection.BeginTransaction();
                    foreach (TableInfo tableInfo in tableInfoList)
                    {
                        DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
                        DropFKs(context, tableInfo);
                    }

                    foreach (TableInfo tableInfo in tableInfoList)
                    {
                        DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
                        DropOriginalTables(context, tableInfo);
                        RenameTempTables(context, tableInfo);
                        CreateIndexes(context, tableInfo);
                    }

                    foreach (TableInfo tableInfo in tableInfoList)
                    {
                        DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
                        CreateFKs(context, tableInfo);
                        context.WriteInformation(string.Format("Finalizing table {0} done", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));
                    }

                    targetServerConnection.CommitTransaction();
                }
                catch (Exception ex)
                {
                    targetServerConnection.RollBackTransaction();
                    throw;
                }
            }
        }

        #endregion

        #region private methods
        private void DropFKs(IDBReplicationFinalizeContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
            Table targetTable = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName].Tables[dbReplicationTableDetails.TableName, dbReplicationTableDetails.TableSchema];
            if (targetTable == null)
                return;

            foreach (ForeignKey fk in targetTable.ForeignKeys.Cast<ForeignKey>().ToList())
                fk.Drop();
        }

        private void DropOriginalTables(IDBReplicationFinalizeContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
            Table targetTable = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName].Tables[dbReplicationTableDetails.TableName, dbReplicationTableDetails.TableSchema];
            if (targetTable == null)
                return;

            targetTable.Drop();
        }

        private void RenameTempTables(IDBReplicationFinalizeContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
            Table targetTable = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName].Tables[GetFullTableName(dbReplicationTableDetails.TableName, TempTableNameSuffix), dbReplicationTableDetails.TableSchema];
            targetTable.Rename(tableInfo.DBReplicationTableDetails.TableName);
        }

        private void CreateIndexes(IDBReplicationFinalizeContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
            Database database = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName];
            database.ExecuteNonQuery(tableInfo.IndexesScripts);
            context.WriteInformation(String.Format("Indexes on table '{0}' created", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));
        }

        private void CreateFKs(IDBReplicationFinalizeContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;
            Database database = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName];
            database.ExecuteNonQuery(tableInfo.FKsScripts);
            context.WriteInformation(String.Format("FKs on table '{0}' created", GetFullTableName(dbReplicationTableDetails.TableName, null, dbReplicationTableDetails.TableSchema)));
        }

        private string ScriptIndexes(Table sourceTable)
        {
            string script = string.Empty;
            if (sourceTable.HasIndex)
            {
                foreach (Index index in sourceTable.Indexes)
                {
                    StringCollection sc = index.Script();
                    string[] strings = new string[sc.Count];
                    sc.CopyTo(strings, 0);
                    script = script + string.Join(" ", strings);
                }
            }
            return script;
        }

        private string ScriptAllFKs(Table sourceTable)
        {
            string scriptedFK = string.Empty;
            if (sourceTable.ForeignKeys != null && sourceTable.ForeignKeys.Count > 0)
            {
                foreach (ForeignKey fk in sourceTable.ForeignKeys)
                {
                    StringCollection sc = fk.Script();
                    string[] strings = new string[sc.Count];
                    sc.CopyTo(strings, 0);
                    scriptedFK = scriptedFK + string.Join(" ", strings);
                }
            }
            return scriptedFK;
        }

        private Table GetTableReference(Server server, string databaseName, string tableName, string tableSchema)
        {
            Table sourceTable = server.Databases[databaseName].Tables[tableName, tableSchema];
            return sourceTable;
        }

        private void CreateTempTable(TableInfo tableInfo)
        {
            Database database = tableInfo.TargetServer.Databases[tableInfo.TargetDatabaseName];
            bool tempTableExists = database.Tables.Contains(tableInfo.DBReplicationTableDetails.TableName + TempTableNameSuffix, tableInfo.DBReplicationTableDetails.TableSchema);
            if (tempTableExists)
                database.Tables[tableInfo.DBReplicationTableDetails.TableName + TempTableNameSuffix, tableInfo.DBReplicationTableDetails.TableSchema].Drop();

            List<string> columnsToInsert;

            CreateTable(tableInfo.Table, tableInfo.TargetServer, tableInfo.TargetDatabaseName, TempTableNameSuffix, out columnsToInsert);
            tableInfo.ColumnsToInsert = columnsToInsert;
        }

        private Table CreateTable(Table sourcetable, Server targetServer, string targetDatabase, string tableNameSuffix, out List<string> columnsToInsert)
        {
            Database db = targetServer.Databases[targetDatabase];

            string schemaName = sourcetable.Schema;

            if (!db.Schemas.Contains(schemaName))
            {
                Schema schema = new Schema(db, schemaName);
                schema.Create();
            }

            string tableName = GetFullTableName(sourcetable.Name, tableNameSuffix);

            Table copiedtable = new Table(db, tableName, schemaName);
            Server server = sourcetable.Parent.Parent;
            CreateColumns(sourcetable, copiedtable, out columnsToInsert);

            copiedtable.AnsiNullsStatus = sourcetable.AnsiNullsStatus;
            copiedtable.QuotedIdentifierStatus = sourcetable.QuotedIdentifierStatus;
            copiedtable.TextFileGroup = sourcetable.TextFileGroup;
            copiedtable.FileGroup = sourcetable.FileGroup;
            copiedtable.Create();

            return copiedtable;
        }

        private void CreateColumns(Table sourcetable, Table copiedtable, out List<string> columnsToInsert)
        {
            columnsToInsert = new List<string>();

            Server server = sourcetable.Parent.Parent;
            List<Column> columnsToBeAddedAtEnd = new List<Column>();

            foreach (Column source in sourcetable.Columns)
            {
                Column column = new Column(copiedtable, source.Name, source.DataType);
                column.Collation = source.Collation;
                column.Nullable = source.Nullable;
                column.Computed = source.Computed;
                column.ComputedText = source.ComputedText;

                column.Identity = source.Identity;
                column.IdentityIncrement = source.IdentityIncrement;
                column.IdentitySeed = source.IdentitySeed;

                column.Default = source.Default;

                if (source.DefaultConstraint != null)
                {
                    column.AddDefaultConstraint(Guid.NewGuid().ToString());
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

                if (string.Compare(column.Name, "timestamp", true) != 0 && !column.Computed)
                {
                    columnsToInsert.Add(column.Name);
                    copiedtable.Columns.Add(column);
                }
                else
                {
                    columnsToBeAddedAtEnd.Add(column);
                }
            }

            if (columnsToBeAddedAtEnd.Count > 0)
            {
                foreach (var column in columnsToBeAddedAtEnd)
                    copiedtable.Columns.Add(column);
            }
        }

        private string GetFullTableName(string tableName, string tableSuffix = null, string tableSchema = null)
        {
            string result = tableName;

            if (!string.IsNullOrEmpty(tableSuffix))
                result = string.Format("{0}{1}", tableName, tableSuffix);

            if (!string.IsNullOrEmpty(tableSchema))
                result = string.Format("{0}.{1}", tableSchema, result);

            return result;
        }

        private DBReplicationTableMigrateDataContext BuildDBReplicationTableMigrateDataContext(IDBReplicationMigrateDataContext context, TableInfo tableInfo)
        {
            DBReplicationTableDetails dbReplicationTableDetails = tableInfo.DBReplicationTableDetails;

            return new DBReplicationTableMigrateDataContext()
            {
                Columns = tableInfo.ColumnsToInsert,
                TargetConnectionString = tableInfo.TargetConnectionString,
                TableSchema = dbReplicationTableDetails.TableSchema,
                TargetTempTableName = GetFullTableName(dbReplicationTableDetails.TableName, TempTableNameSuffix),
                SourceTableName = dbReplicationTableDetails.TableName,
                FromTime = context.FromTime,
                ToTime = context.ToTime,
                FilterDateTimeColumn = dbReplicationTableDetails.FilterDateTimeColumn,
                NumberOfDaysPerInterval = context.NumberOfDaysPerInterval,
                WriteInformation = context.WriteInformation,
                ChunkSize = dbReplicationTableDetails.ChunkSize,
                IdColumn = dbReplicationTableDetails.IdColumn,
                DbReplicationPreInsert = dbReplicationTableDetails.DBReplicationPreInsert
            };
        }

        #endregion

        #region private classes
        private class TableInfo
        {
            public DBReplicationTableDetails DBReplicationTableDetails { get; set; }
            public string IndexesScripts { get; set; }
            public string FKsScripts { get; set; }
            public Server SourceServer { get; set; }
            public Server TargetServer { get; set; }
            public Table Table { get; set; }
            public DBReplicationTableDataManager DataManager { get; set; }
            public List<string> ColumnsToInsert { get; set; }
            public string TargetDatabaseName { get; set; }
            public string TargetConnectionString { get; set; }
        }

        #endregion
    }
}