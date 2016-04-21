using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
namespace Vanrise.Analytic.Business
{
    public class AnalyticTableManager
    {
        #region Public Methods
        public IEnumerable<AnalyticTableInfo> GetAnalyticTablesInfo(AnalyticTableInfoFilter filter)
        {
            var analyticTables = GetCachedAnalyticTables();
            if(filter !=null)
            {
                return analyticTables.MapRecords(AnalyticTableInfoMapper,x => filter.OnlySelectedIds.Contains(x.AnalyticTableId));
            }
           
            return analyticTables.MapRecords(AnalyticTableInfoMapper);
        }
        #endregion

        #region Private Methods

        Dictionary<int, AnalyticTable> GetCachedAnalyticTables()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAnalyticTables",
                () =>
                {
                    IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
                    IEnumerable<AnalyticTable> analyticTables = dataManager.GetAnalyticTables();
                    return analyticTables.ToDictionary(x => x.AnalyticTableId, analyticTable => analyticTable);
                });
        }
        #endregion
      
        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            IAnalyticTableDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAnalyticTableUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        AnalyticTableInfo AnalyticTableInfoMapper(AnalyticTable analyticTable)
        {
            return new AnalyticTableInfo()
            {
                AnalyticTableId = analyticTable.AnalyticTableId,
                Name = analyticTable.Name
            };
        }
        #endregion
    }
}
