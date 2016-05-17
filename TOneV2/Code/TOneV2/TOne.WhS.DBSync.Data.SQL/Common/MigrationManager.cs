using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationManager : BaseSQLDataManager
    {

        ServerConnection _ServerConnection = null;
        Server _Server;
        MigrationContext _Context;
        List<DatabaseScripts> databaseScripts = new List<DatabaseScripts>();

        #region Public Methods
        public MigrationManager(MigrationContext context)
        {
            _ServerConnection = new ServerConnection(context.MigrationCredentials.MigrationServer, context.MigrationCredentials.MigrationServerUserID, context.MigrationCredentials.MigrationServerPassword);
            _Server = new Server(_ServerConnection);
            _Context = context;
        }

        public bool PrepareBeforeApplyingRecords()
        {
            bool Executed = false;
            try
            {
                _ServerConnection.BeginTransaction();
                GetAllDatabasesofTables();
                DefineTables();
                ScriptAllFKs();
                CreateTempTables();
                _ServerConnection.CommitTransaction();
                Executed = true;
            }
            catch (Exception ex)
            {
                _ServerConnection.RollBackTransaction();
                throw ex;
            }
            return Executed;
        }

        public bool FinalizeMigration()
        {
            bool Executed = false;
            try
            {
                _ServerConnection.BeginTransaction();
                DropFKs();
                DropOriginalTables();
                RenameTempTables();
                CreateIndexes();
                CreateFKs();
                _ServerConnection.CommitTransaction();
                Executed = true;
            }
            catch (Exception ex)
            {
                _ServerConnection.RollBackTransaction();
                throw ex;
            }
            return Executed;
        }

        #endregion

        #region Private Methods

        private void GetAllDatabasesofTables()
        {
            foreach (DBTable dbTable in _Context.DBTables.Values)
                if (!databaseScripts.Exists(x => x.Database == dbTable.Database))
                    databaseScripts.Add(new DatabaseScripts { Database = dbTable.Database });
        }

        private void DefineTables()
        {
            foreach (DBTable table in _Context.DBTables.Values)
            {
                Table sourceTable = GetTableReference(table);
                table.Info = sourceTable;
                table.ScriptedIndexes = ScriptIndexes(sourceTable);
                table.ScriptedTempTable = ScriptTempTable(sourceTable);
            }
        }

        private Table GetTableReference(DBTable dbTable)
        {
            Table sourceTable = _Server.Databases[dbTable.Database].Tables[dbTable.Name, dbTable.Schema];
            return sourceTable;
        }

        private string ScriptIndexes(Table sourceTable)
        {
            string script = string.Empty;
            if (sourceTable.HasIndex)
                foreach (Index index in sourceTable.Indexes)
                {
                    StringCollection sc = index.Script();
                    string[] strings = new string[sc.Count];
                    sc.CopyTo(strings, 0);
                    script = script + string.Join(" ", strings);
                }
            return script;
        }

        private string ScriptTempTable(Table sourceTable)
        {
            ScriptingOptions so = new ScriptingOptions();
            so.DriAllKeys = false;
            string script = string.Empty;
            StringCollection sc = sourceTable.Script(so);
            string[] strings = new string[sc.Count];
            sc.CopyTo(strings, 0);
            script = script + string.Join(" ", strings);
            script = script.Replace("[" + sourceTable.Schema + "].[" + sourceTable.Name + "]", "[" + sourceTable.Schema + "].[" + sourceTable.Name + Constants._Temp + "]");
            return script;
        }

        private void ScriptAllFKs()
        {
            foreach (var db in databaseScripts)
                foreach (Table table in _Server.Databases[db.Database].Tables)
                {
                    string scriptedFK = string.Empty;
                    if (table.EnumForeignKeys().Rows.Count > 0)
                        foreach (ForeignKey fk in table.ForeignKeys)
                        {
                            StringCollection sc = fk.Script();
                            string[] strings = new string[sc.Count];
                            sc.CopyTo(strings, 0);
                            scriptedFK = scriptedFK + string.Join(" ", strings);
                        }
                    if (db.ScriptCreateFKs == null)
                        db.ScriptCreateFKs = new StringCollection();
                    if (scriptedFK != string.Empty)
                        db.ScriptCreateFKs.Add(scriptedFK);
                }
        }

        public void CreateTempTables()
        {
            foreach (DBTable dbTable in _Context.DBTables.Values)
            {
                Database database = _Server.Databases[dbTable.Database];
                bool tableExists = database.Tables.Contains(dbTable.Name + Constants._Temp, dbTable.Schema);
                if (tableExists)
                {
                    database.Tables[dbTable.Name + Constants._Temp, dbTable.Schema].Drop();
                }

                database.ExecuteNonQuery(dbTable.ScriptedTempTable);
            }
        }

        private void DropFKs()
        {
            foreach (var db in databaseScripts)
                foreach (Table table in _Server.Databases[db.Database].Tables)
                    foreach (ForeignKey fk in table.ForeignKeys.Cast<ForeignKey>().ToList())
                        fk.Drop();
        }

        private void DropOriginalTables()
        {
            //Set Priority by References
            foreach (DBTable table in _Context.DBTables.Values)
            {
                table.DBFKs = new List<DBForeignKey>();
                foreach (ForeignKey fk in table.Info.ForeignKeys)
                {
                    table.DBFKs.Add(new DBForeignKey { ReferencedKey = fk.ReferencedKey, ReferencedTable = fk.ReferencedTable, ReferencedTableSchema = fk.ReferencedTableSchema });
                }
            }
            DropTables();
        }

        private void DropTables()
        {
            bool hasUnDropped = _Context.DBTables.Values.ToList().Exists(x => x.DroppedOriginal == false);
            if (hasUnDropped)
            {
                // Drop Original Tables
                foreach (DBTable table in _Context.DBTables.Values.Where(x => x.DroppedOriginal == false))
                {
                    bool isReferenced = false;

                    foreach (DBTable otherTable in _Context.DBTables.Values.Where(x => x.Name != table.Name && !x.DroppedOriginal))
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
                DropTables();
            }
        }

        private void RenameTempTables()
        {
            foreach (DBTable dbTable in _Context.DBTables.Values)
            {
                DBTable dbTempTable = new DBTable();
                dbTempTable.Name = dbTable.Name + Constants._Temp;
                dbTempTable.Schema = dbTable.Schema;
                dbTempTable.Database = dbTable.Database;
                _Server.Databases[dbTempTable.Database].Tables.Refresh();
                Table tempTable = _Server.Databases[dbTempTable.Database].Tables[dbTempTable.Name, dbTempTable.Schema];
                tempTable.Rename(tempTable.Name.Replace(Constants._Temp, ""));
            }
        }

        private void CreateIndexes()
        {
            //Create Indexes Keys for Tables
            foreach (DBTable table in _Context.DBTables.Values)
                if (table.ScriptedIndexes != "")
                    _Server.Databases[table.Database].ExecuteNonQuery(table.ScriptedIndexes);
        }

        private void CreateFKs()
        {
            foreach (var db in databaseScripts)
                foreach (string script in db.ScriptCreateFKs)
                    _Server.Databases[db.Database].ExecuteNonQuery(script);
        }

        #endregion
    }
}
