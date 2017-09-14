﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.Postgres;
using TOne.WhS.RouteSync.Idb;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.TelesIdb.Postgres
{
    public class IdbPostgresDataManager : BasePostgresDataManager, IIdbDataManager
    {
        public Guid ConfigId { get { return new Guid("34F3483D-2572-4349-A6ED-3504B2D9E714"); } }

        public IdbPostgresDataManager()
        {

        }

        IdbConnectionString _connectionString;
        List<IdbConnectionString> _redundantConnectionStrings;

        Dictionary<int, TelesIdbPostgresDataManager> _telesIdbPostgresDataManagers;

        public IdbConnectionString ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public List<IdbConnectionString> RedundantConnectionStrings
        {
            get { return _redundantConnectionStrings; }
            set { _redundantConnectionStrings = value; }
        }

        public void PrepareTables(ISwitchRouteSynchronizerInitializeContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            {
                telesIdbPostgresDataManager.PrepareTables();
            }, context.SwitchName, context.SwitchId, null, context.WriteBusinessHandledException, true, "initializing", out switchSyncOutput);
            context.SwitchSyncOutput = switchSyncOutput;
        }

        public object PrepareDataForApply(List<ConvertedRoute> idbRoutes)
        {
            return idbRoutes;
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            List<ConvertedRoute> idbRoutes = context.PreparedItemsForApply as List<ConvertedRoute>;

            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            {
                telesIdbPostgresDataManager.ApplyRadiusRoutesForDB(idbRoutes);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, false, null, out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public void SwapTables(ISwapTableContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            {
                string[] args = new string[] { (dataManagerIndex + 1).ToString(), context.SwitchName };

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finalizing Database {0} for Switch '{1}'...", args);

                telesIdbPostgresDataManager.SwapTables(context.IndexesCommandTimeoutInSeconds);

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Database {0} for Switch '{1}' is finalized", args);

            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, true, "finalizing", out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        private void ExecMVTSRadiusSQLDataManagerAction(Action<TelesIdbPostgresDataManager, int> action, string switchName, string switchId, SwitchSyncOutput previousSwitchSyncOutput,
            Action<Exception, bool> writeBusinessHandledException, bool isBusinessException, string businessExceptionMessage, out SwitchSyncOutput switchSyncOutput)
        {
            PrepareDataManagers();
            HashSet<int> failedNodeIndexes = null;
            if (previousSwitchSyncOutput != null && previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList != null)
            {
                failedNodeIndexes = previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList.Select(itm => (itm as TelesIdbSWSyncOutput).ItemIndex).ToHashSet();
                if (failedNodeIndexes != null && failedNodeIndexes.Count == _telesIdbPostgresDataManagers.Count)
                {
                    switchSyncOutput = new SwitchSyncOutput()
                    {
                        SwitchId = switchId,
                        SwitchSyncResult = SwitchSyncResult.Failed
                    };
                    return;
                }
            }

            ConcurrentDictionary<int, SwitchRouteSynchroniserOutput> exceptions = new ConcurrentDictionary<int, SwitchRouteSynchroniserOutput>();

            Parallel.For(0, _telesIdbPostgresDataManagers.Count, (i) =>
            {
                if (failedNodeIndexes == null || !failedNodeIndexes.Contains(i))
                {
                    try
                    {
                        action(_telesIdbPostgresDataManagers[i], i);
                    }
                    catch (Exception ex)
                    {
                        string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                        string exceptionDetail = ex.ToString();
                        exceptions.TryAdd(i, new TelesIdbSWSyncOutput() { ItemIndex = i, ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail });
                        Exception exception = isBusinessException ? new VRBusinessException(string.Format("Error occured while {0} Database {1} for Switch '{2}'", businessExceptionMessage, i + 1, switchName), ex) : ex;
                        writeBusinessHandledException(exception, false);
                    }
                }
            });
            switchSyncOutput = exceptions.Count > 0 ? new SwitchSyncOutput()
            {
                SwitchId = switchId,
                SwitchRouteSynchroniserOutputList = exceptions.Values.ToList(),
                SwitchSyncResult = exceptions.Count == _telesIdbPostgresDataManagers.Count ? SwitchSyncResult.Failed : SwitchSyncResult.PartialFailed
            } : null;
        }

        private void PrepareDataManagers()
        {
            if (_telesIdbPostgresDataManagers == null)
            {
                int counter = 0;
                _telesIdbPostgresDataManagers = new Dictionary<int, TelesIdbPostgresDataManager>();
                _telesIdbPostgresDataManagers.Add(counter, new TelesIdbPostgresDataManager(_connectionString));
                counter++;

                if (_redundantConnectionStrings != null)
                {
                    foreach (IdbConnectionString idbConnectionString in _redundantConnectionStrings)
                    {
                        _telesIdbPostgresDataManagers.Add(counter, new TelesIdbPostgresDataManager(idbConnectionString));
                        counter++;
                    }
                }
            }
        }
    }

    public class TelesIdbPostgresDataManager : BasePostgresDataManager
    {
        const string tableName = "route";
        const string tempTableName = "route_temp";
        const string oldTableName = "route_old";

        public string ConnectionString { get { return GetConnectionString(); } }

        IdbConnectionString _idbConnectionString;
        public TelesIdbPostgresDataManager(IdbConnectionString idbConnectionString)
        {
            _idbConnectionString = idbConnectionString;
        }
        protected override string GetConnectionString()
        {
            return _idbConnectionString.ConnectionString;
        }

        #region Constants

        #endregion

        public void PrepareTables()
        {
            BuildRouteTempTable(tableName);
        }

        public void ApplyRadiusRoutesForDB(List<ConvertedRoute> idbRoutes)
        {
            base.Bulk(idbRoutes, tempTableName);
        }

        public void SwapTables(int indexesCommandTimeoutInSeconds)
        {
            string createindexScript = string.Format("ALTER TABLE {0} ADD constraint route_pkey_{1} PRIMARY KEY (pref)", tempTableName, Guid.NewGuid().ToString("N"));
            string swapTableScript = string.Format("ALTER TABLE {0} RENAME TO {1}; ALTER TABLE {2} RENAME TO {0}; ", tableName, oldTableName, tempTableName);
            ExecuteNonQuery(new string[] { createindexScript, swapTableScript }, indexesCommandTimeoutInSeconds);
        }

        void BuildRouteTempTable(string routeTableName)
        {
            string dropTempTableScript = string.Format("DROP TABLE IF EXISTS {0};", tempTableName);
            string dropOldTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", oldTableName);
            string createTempTableScript = string.Format(@"CREATE TABLE {0} (       
                                                           pref character varying(255) COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::character varying,
                                                           route character varying(255) COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::character varying);", tempTableName);

            ExecuteNonQuery(new string[] { dropTempTableScript, dropOldTableScript, createTempTableScript });
        }
    }
}