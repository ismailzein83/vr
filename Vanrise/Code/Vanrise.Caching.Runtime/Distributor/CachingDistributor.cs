using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;

namespace Vanrise.Caching.Runtime
{
    internal class CachingDistributor
    {
        internal static CachingDistributor s_current;

        Dictionary<string, int> _runtimeProcessIdByCacheNames = new Dictionary<string, int>();
        Dictionary<int, int> _cacheCountByRuntimeProcessIds = new Dictionary<int, int>();


        RuntimeServiceInstanceManager _runtimeServiceInstanceManager = new RuntimeServiceInstanceManager();


        internal void Initialize()
        {
            var runtimeServiceInstances = _runtimeServiceInstanceManager.GetServices(CachingRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
            var interRuntimeServiceManager = new InterRuntimeServiceManager();
            Parallel.ForEach(runtimeServiceInstances, (serviceInstance) =>
            {
                var processId = serviceInstance.ProcessId;
                List<string> runtimeFullCacheNames = interRuntimeServiceManager.SendRequest(processId, new GetRuntimeAllCacheNamesRequest
                {
                });
                lock (this)
                {
                    foreach (var fullCacheName in runtimeFullCacheNames)
                    {
                        _runtimeProcessIdByCacheNames.Add(fullCacheName, processId);
                    }
                    _cacheCountByRuntimeProcessIds.Add(processId, runtimeFullCacheNames.Count);
                }
            });
        }

        internal void SyncServiceRuntimeProcesses()
        {
            var runtimeServiceInstances = _runtimeServiceInstanceManager.GetServices(CachingRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
            HashSet<int> runningProcessIds = new HashSet<int>(runtimeServiceInstances.Select(itm => itm.ProcessId));
            foreach (var processId in runningProcessIds)
            {
                if (!_cacheCountByRuntimeProcessIds.ContainsKey(processId))
                {
                    lock (this)
                    {
                        _cacheCountByRuntimeProcessIds.Add(processId, 0);
                    }
                }
            }
            if (runningProcessIds.Count != _cacheCountByRuntimeProcessIds.Count)
            {
                HashSet<int> nonRunningProcessIds = new HashSet<int>(_cacheCountByRuntimeProcessIds.Keys.Where(processId => !runningProcessIds.Contains(processId)));
                var cacheNamesWithNonRunningProcesses = _runtimeProcessIdByCacheNames.Where(itm => nonRunningProcessIds.Contains(itm.Value));
                foreach (var cacheNameEntry in cacheNamesWithNonRunningProcesses)
                {
                    lock (this)
                    {
                        _runtimeProcessIdByCacheNames.Remove(cacheNameEntry.Key);
                    }
                }
                foreach (var processId in nonRunningProcessIds)
                {
                    lock (this)
                    {
                        _cacheCountByRuntimeProcessIds.Remove(processId);
                    }
                }
            }
        }

        internal int? GetRuntimeProcessId(string cacheFullName)
        {
            int runtimeProcessId;
            if (_runtimeProcessIdByCacheNames.TryGetValue(cacheFullName, out runtimeProcessId))
                return runtimeProcessId;

            int processIdWithMinCaches = 0;

            lock (this)
            {
                if (_runtimeProcessIdByCacheNames.TryGetValue(cacheFullName, out runtimeProcessId))
                    return runtimeProcessId;
                if (_cacheCountByRuntimeProcessIds.Count == 0)
                    return null;
                processIdWithMinCaches = _cacheCountByRuntimeProcessIds.OrderBy(itm => itm.Value).First().Key;
                _runtimeProcessIdByCacheNames.Add(cacheFullName, processIdWithMinCaches);
                _cacheCountByRuntimeProcessIds[processIdWithMinCaches]++;
            }
            return processIdWithMinCaches;
        }
    }
}
