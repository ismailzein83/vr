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
                s_TryLockInterval = new TimeSpan(0, 0, 2);
            if (!int.TryParse(ConfigurationManager.AppSettings["FraudAnalysis_CDRDatabaseLockMaxRetryCount"], out s_lockMaxRetryCount))
                s_lockMaxRetryCount = 3;
        }

        internal void CreateNewDBIfNotCreated(DateTime fromTime)
        {
            int retryCount = 0;
            while(retryCount < s_lockMaxRetryCount)
            {
                int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
                IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);
                bool isLocked = (bool)ExecuteScalarSP("FraudAnalysis.sp_CDRDatabase_TryLockIfDBNotReady", fromTime, currentRuntimeProcessId, runningRuntimeProcessesIds != null ? String.Join(",", runningRuntimeProcessesIds) : null);
                if (isLocked)
                {
                    PartitionedCDRDataManager baseCDRDataManager = new PartitionedCDRDataManager();
                    string databaseName;
                    DateTime toTime;
                    baseCDRDataManager.CreateDatabase(fromTime, out databaseName, out toTime);
                    CDRDatabaseSettings settings = new CDRDatabaseSettings
                    {
                        DatabaseName = databaseName
                    };
                    ExecuteNonQuerySP("[FraudAnalysis].[sp_CDRDatabase_SetReadyAndUnlock]", fromTime, toTime, Vanrise.Common.Serializer.Serialize(settings, true));
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    return;
                }
                else
                {
                    Thread.Sleep(s_TryLockInterval);
                    object isReadyObject = ExecuteScalarSP("[FraudAnalysis].[sp_CDRDatabase_GetIsReady]", fromTime);
                    if (isReadyObject != null && (bool)isReadyObject)
                        return;
                    else
                    {
                        if (retryCount >= s_lockMaxRetryCount)
                            throw new Exception(String.Format("Max Retry Count '{0}' reached when trying to create CDR database of Start Time '{1}'", retryCount, fromTime));
                        retryCount++;
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
