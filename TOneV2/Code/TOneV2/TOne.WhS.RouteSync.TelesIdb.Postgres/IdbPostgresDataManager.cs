using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;
using Vanrise.Data.Postgres;
using System.Linq;
using TOne.WhS.RouteSync.TelesIdb;
using Vanrise.Common;
using Vanrise.Entities;

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

        public object PrepareDataForApply(List<ConvertedRoute> radiusRoutes)
        {
            throw new NotImplementedException();
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public void SwapTables(ISwapTableContext context)
        {
            throw new NotImplementedException();
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

        internal void PrepareTables()
        {
            throw new NotImplementedException();
        }
    }
}