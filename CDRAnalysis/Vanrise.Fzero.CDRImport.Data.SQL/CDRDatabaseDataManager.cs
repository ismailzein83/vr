using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Runtime;
using Vanrise.Common;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDatabaseDataManager : BaseSQLDataManager
    {
        public CDRDatabaseDataManager()
            : base("CDRDBConnectionString")
        {

        }

        
        RunningProcessManager _runningProcessManager = new RunningProcessManager();
        static TimeSpan s_TryLockInterval;
        static int s_lockMaxRetryCount;

        static CDRDatabaseDataManager()
        {

            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["FraudAnalysis_CDRDatabaseLockInterval"], out s_TryLockInterval))
                s_TryLockInterval = new TimeSpan(0, 0, 0, 0, 300);
            if (!int.TryParse(ConfigurationManager.AppSettings["FraudAnalysis_CDRDatabaseLockMaxRetryCount"], out s_lockMaxRetryCount))
                s_lockMaxRetryCount = 30;
        }

        internal void CreateNewDBIfNotCreated(DateTime fromTime)
        {
            int retryCount = 0;
            while(retryCount < s_lockMaxRetryCount)
            {
                int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
                IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
                bool isLocked = (bool)ExecuteScalarSP("FraudAnalysis.sp_CDRDatabase_TryLockIfDBNotReady", fromTime, currentRuntimeProcessId, runningRuntimeProcessesIds != null ? String.Join(",", runningRuntimeProcessesIds) : null);
                if (isLocked)
                {
                    int prefixLength;
                    if (!int.TryParse(ConfigurationManager.AppSettings["FraudAnalysis_CDRNormalPrefixLength"], out prefixLength))
                        prefixLength = 5;
                    CDRDatabaseSettings databaseSettings = new CDRDatabaseSettings
                    {                      
                        PrefixLength = prefixLength,
                        CDRNumberPrefixes = new HashSet<string>()
                    };
                    PartitionedCDRDataManager baseCDRDataManager = new PartitionedCDRDataManager();
                    baseCDRDataManager.DatabaseSettings = databaseSettings;
                    string databaseName;
                    DateTime toTime;
                    baseCDRDataManager.CreateDatabase(fromTime, out databaseName, out toTime);
                    databaseSettings.DatabaseName = databaseName;
                    ExecuteNonQuerySP("[FraudAnalysis].[sp_CDRDatabase_SetReadyAndUnlock]", fromTime, toTime, Vanrise.Common.Serializer.Serialize(databaseSettings, true));
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    return;
                }
                else
                {
                    Thread.Sleep(s_TryLockInterval);
                    object isReadyObject = ExecuteScalarSP("[FraudAnalysis].[sp_CDRDatabase_GetIsReady]", fromTime);
                    if (isReadyObject != null && isReadyObject != DBNull.Value && (bool)isReadyObject)
                        return;
                    else
                    {
                        retryCount++;
                        if (retryCount >= s_lockMaxRetryCount)
                            throw new Exception(String.Format("Max Retry Count '{0}' reached when trying to create CDR database of Start Time '{1}'", retryCount, fromTime));
                    }
                }
            }
        }

        internal bool TryGetReadyDatabase(DateTime fromTime, out CDRDatabaseInfo databaseInfo)
        {
            var cachedDbs = GetAllCachedReadyDatabases();
            if (cachedDbs != null)
                return cachedDbs.TryGetValue(fromTime, out databaseInfo);
            else
            {
                databaseInfo = null;
                return false;
            }
        }

        internal CDRDatabaseInfo GetLastReadyDatabase()
        {
            var cachedDbs = GetAllCachedReadyDatabases();
            if (cachedDbs != null)
                return cachedDbs.Values.LastOrDefault();
            else
                return null;
        }

        internal CDRDatabaseInfo GetWithLock(DateTime fromTime)
        {
            int retryCount = 0;
            while (retryCount < s_lockMaxRetryCount)
            {
                int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
                IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
                CDRDatabaseInfo cdrDataBaseInfo = GetItemSP("[FraudAnalysis].[sp_CDRDatabase_TryGetWithLock]", CDRDatabaseInfoMapper, fromTime, currentRuntimeProcessId, runningRuntimeProcessesIds != null ? String.Join(",", runningRuntimeProcessesIds) : null);
                if (cdrDataBaseInfo != null)
                {
                    return cdrDataBaseInfo;
                }
                else
                {
                    Thread.Sleep(s_TryLockInterval);
                    retryCount++;
                    if (retryCount >= s_lockMaxRetryCount)
                        throw new Exception(String.Format("Max Retry Count '{0}' reached when trying to lock and get CDR database of Start Time '{1}'", retryCount, fromTime));
                }
            }
            return null;
        }

        internal void UpdateSettingsAndUnlock(DateTime fromTime, CDRDatabaseSettings settings)
        {
            ExecuteNonQuerySP("[FraudAnalysis].[sp_CDRDatabase_UpdateSettingsAndUnLock]", fromTime, Vanrise.Common.Serializer.Serialize(settings, true));
        }

        internal void Unlock(DateTime fromTime)
        {
            ExecuteNonQuerySP("[FraudAnalysis].[sp_CDRDatabase_UnLock]", fromTime);
        }

        Dictionary<DateTime, CDRDatabaseInfo> GetAllCachedReadyDatabases()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCachedDatabases",
                () =>
                {
                    var databaseInfos = GetItemsSP("FraudAnalysis.sp_CDRDatabase_GetAllReady", CDRDatabaseInfoMapper);
                    if (databaseInfos != null)
                        return databaseInfos.ToDictionary(itm => itm.FromTime, itm => itm);
                    else
                        return null;
                });
        }

        internal IEnumerable<CDRDatabaseInfo> GetReadyDatabases(DateTime fromTime, DateTime toTime)
        {
            var cachedDBs = GetAllCachedReadyDatabases();
            if (cachedDBs != null)
                return cachedDBs.Values.FindAllRecords(itm => (itm.FromTime <= fromTime && fromTime <= itm.FromTime)
                    || (itm.FromTime <= toTime && toTime <= itm.ToTime));
            else
                return null;
        }

        private CDRDatabaseInfo CDRDatabaseInfoMapper(System.Data.IDataReader reader)
        {
            CDRDatabaseInfo databaseInfo = new CDRDatabaseInfo
            {
                FromTime = (DateTime)reader["FromTime"],
                ToTime = (DateTime)reader["ToTime"]
            };
            string serializedSettings = reader["Settings"] as string;
            if (serializedSettings != null)
                databaseInfo.Settings = Vanrise.Common.Serializer.Deserialize<CDRDatabaseSettings>(serializedSettings);
            return databaseInfo;
        }

        internal bool AreCDRDatabasesChanged(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.CDRDatabase", ref updateHandle);
        }

        internal void SetCacheExpired()
        {
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            CDRDatabaseDataManager _dataManager = new CDRDatabaseDataManager();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCDRDatabasesChanged(ref _updateHandle);
            }

        }

        
    }

}
