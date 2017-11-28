using System;
using System.Text;
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
            ExecdbPostgresDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
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
            List<IdbConvertedRoute> idbRoutes = (context.PreparedItemsForApply as List<ConvertedRoute>).Select(itm => itm as IdbConvertedRoute).ToList();

            SwitchSyncOutput switchSyncOutput;
            ExecdbPostgresDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            {
                telesIdbPostgresDataManager.ApplyIdbRoutesForDB(idbRoutes);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, false, null, out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public void SwapTables(ISwapTableContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecdbPostgresDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            {
                string[] args = new string[] { (dataManagerIndex + 1).ToString(), context.SwitchName };

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finalizing Database {0} for Switch '{1}'...", args);

                telesIdbPostgresDataManager.SwapTables(context.IndexesCommandTimeoutInSeconds);

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Database {0} for Switch '{1}' is finalized", args);

            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, true, "finalizing", out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public bool BlockCustomer(IdbBlockCustomerContext context)
        {
            PrepareDataManagers();
            ConcurrentDictionary<int, Exception> exceptions = new ConcurrentDictionary<int, Exception>();
            Parallel.For(0, _telesIdbPostgresDataManagers.Count, (i) =>
            {
                try
                {
                    _telesIdbPostgresDataManagers[i].BlockCustomer(context.CustomerMappings);
                }
                catch (Exception ex)
                {
                    exceptions.TryAdd(i, ex);
                }
            });

            if (exceptions.Count > 0)
            {
                StringBuilder strBuilder = new StringBuilder();
                foreach (var exception in exceptions)
                    strBuilder.AppendLine(string.Format("Database {0} for Switch '{1}': {2}.", exception.Key + 1, context.SwitchName, exception.Value.Message));

                context.ErrorMessage = strBuilder.ToString();
                return false;
            }

            return true;
        }

        //public void ApplyDifferentialRoutes(IIdbDataManagerApplyDifferentialRoutesContext context)
        //{
        //    List<IdbConvertedRoute> idbRoutes = context.ConvertedUpdatedRoutes.Select(itm => itm as IdbConvertedRoute).ToList();

        //    Parallel.For(0, _telesIdbPostgresDataManagers.Count, (i) =>
        //    {
        //        _telesIdbPostgresDataManagers[i].ApplyDifferentialRoutes(idbRoutes);
        //    });
        //}

        private void ExecdbPostgresDataManagerAction(Action<TelesIdbPostgresDataManager, int> action, string switchName, string switchId, SwitchSyncOutput previousSwitchSyncOutput,
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
        const string TableName = "route";
        const string TempTableName = "route_temp";
        const string OldTableName = "route_old";

        string TableNameWithSchema { get { return !string.IsNullOrEmpty(_idbConnectionString.SchemaName) ? string.Format("{0}.{1}", _idbConnectionString.SchemaName, TableName) : TableName; } }
        string TempTableNameWithSchema { get { return !string.IsNullOrEmpty(_idbConnectionString.SchemaName) ? string.Format("{0}.{1}", _idbConnectionString.SchemaName, TempTableName) : TempTableName; } }
        string OldTableNameWithSchema { get { return !string.IsNullOrEmpty(_idbConnectionString.SchemaName) ? string.Format("{0}.{1}", _idbConnectionString.SchemaName, OldTableName) : OldTableName; } }

        IdbConnectionString _idbConnectionString;

        public string ConnectionString { get { return GetConnectionString(); } }

        public TelesIdbPostgresDataManager(IdbConnectionString idbConnectionString)
        {
            _idbConnectionString = idbConnectionString;
        }

        protected override string GetConnectionString()
        {
            return _idbConnectionString.ConnectionString;
        }

        public void PrepareTables()
        {
            BuildRouteTempTable();
        }

        public void ApplyIdbRoutesForDB(List<IdbConvertedRoute> idbRoutes)
        {
            base.Bulk(idbRoutes, TempTableNameWithSchema);
        }

        public void SwapTables(int indexesCommandTimeoutInSeconds)
        {
            string createindexScript = string.Format("ALTER TABLE {0} ADD constraint route_pkey_{1} PRIMARY KEY (pref)", TempTableNameWithSchema, Guid.NewGuid().ToString("N"));
            string swapTableScript = string.Format("ALTER TABLE IF EXISTS {0} RENAME TO {1}; ALTER TABLE {2} RENAME TO {3}; ", TableNameWithSchema, OldTableName, TempTableNameWithSchema, TableName);
            ExecuteNonQuery(new string[] { createindexScript, swapTableScript }, indexesCommandTimeoutInSeconds);
        }

        //public void ApplyDifferentialRoutes(List<IdbConvertedRoute> idbRoutes)
        //{
        //    if (idbRoutes == null || idbRoutes.Count == 0)
        //        return;

        //    List<string> updateQueries = new List<string>();

        //    foreach (var idbRoute in idbRoutes)
        //        updateQueries.Add(string.Format("Update {0} Set route = '{1}' Where pref = '{2}'", TableNameWithSchema, idbRoute.Route, idbRoute.Pref));

        //    ExecuteNonQuery(updateQueries.ToArray());
        //}

        void BuildRouteTempTable()
        {
            string dropTempTableScript = string.Format("DROP TABLE IF EXISTS {0};", TempTableNameWithSchema);
            string dropOldTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", TableNameWithSchema);
            string createTempTableScript = string.Format(@"CREATE TABLE {0} (       
                                                           pref character varying(255) COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::character varying,
                                                           route character varying(255) COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::character varying);", TempTableNameWithSchema);

            ExecuteNonQuery(new string[] { dropTempTableScript, dropOldTableScript, createTempTableScript });
        }

        internal void BlockCustomer(List<string> customerMappings)
        {
            StringBuilder blockCustomerScript = new StringBuilder();
            blockCustomerScript.AppendFormat("UPDATE {0} SET route = 'BLK' WHERE ", TableNameWithSchema);
            blockCustomerScript.Append(string.Join(" or ", customerMappings.Select(itm => string.Format("pref LIKE '{0}%'", itm))));

            ExecuteNonQuery(new string[] { blockCustomerScript.ToString() });
        }
    }
}