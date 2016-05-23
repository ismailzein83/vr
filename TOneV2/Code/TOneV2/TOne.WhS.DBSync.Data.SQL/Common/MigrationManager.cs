﻿using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

        private Table createTable(Table sourcetable)
        {
            Database db = sourcetable.Parent;
            string schema = sourcetable.Schema;
            Table copiedtable = new Table(db, sourcetable.Name + Constants._Temp, schema);
            Server server = sourcetable.Parent.Parent;

            createColumns(sourcetable, copiedtable);

            copiedtable.AnsiNullsStatus = sourcetable.AnsiNullsStatus;
            copiedtable.QuotedIdentifierStatus = sourcetable.QuotedIdentifierStatus;
            copiedtable.TextFileGroup = sourcetable.TextFileGroup;
            copiedtable.FileGroup = sourcetable.FileGroup;
            copiedtable.Create();

            return copiedtable;
        }

        private void createColumns(Table sourcetable, Table copiedtable)
        {
            Server server = sourcetable.Parent.Parent;

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

                copiedtable.Columns.Add(column);
            }
        }

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

                table.DBFKs = new List<DBForeignKey>();
                foreach (ForeignKey fk in table.Info.ForeignKeys)
                {
                    table.DBFKs.Add(new DBForeignKey { ReferencedKey = fk.ReferencedKey, ReferencedTable = fk.ReferencedTable, ReferencedTableSchema = fk.ReferencedTableSchema });
                }

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


        private void ScriptAllFKs()
        {
            foreach (var db in databaseScripts)
                foreach (Table table in _Server.Databases[db.Database].Tables)
                {
                    string scriptedFK = string.Empty;
                    if (table.ForeignKeys.Count > 0)
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
                createTable(dbTable.Info);
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
            foreach (DBTable dbTable in _Context.DBTables.Values)
            {
                dbTable.Info.Drop();
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
