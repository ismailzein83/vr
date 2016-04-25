﻿using System;
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
        public Vanrise.Entities.IDataRetrievalResult<AnalyticTableDetail> GetFilteredAnalyticTables(Vanrise.Entities.DataRetrievalInput<AnalyticTableQuery> input)
        {
            var analyticTables = GetCachedAnalyticTables();

            Func<AnalyticTable, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, analyticTables.ToBigResult(input, filterExpression, AnalyticTableDetailMapper));
        }
        public IEnumerable<AnalyticTableInfo> GetAnalyticTablesInfo(AnalyticTableInfoFilter filter)
        {
            var analyticTables = GetCachedAnalyticTables();
            if(filter !=null)
            {
                return analyticTables.MapRecords(AnalyticTableInfoMapper,x => filter.OnlySelectedIds.Contains(x.AnalyticTableId));
            }
           
            return analyticTables.MapRecords(AnalyticTableInfoMapper);
        }
        public AnalyticTable GetAnalyticTableById(int analyticTableId)
        {
            var analyticTables = GetCachedAnalyticTables();
            return analyticTables.GetRecord(analyticTableId);
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

        AnalyticTableDetail AnalyticTableDetailMapper(AnalyticTable analyticTable)
        {
            return new AnalyticTableDetail()
            {
                Entity = analyticTable,
            };

        }
        AnalyticTableInfo AnalyticTableInfoMapper(AnalyticTable analyticTable)
        {
            return new AnalyticTableInfo()
            {
                AnalyticTableId = analyticTable.AnalyticTableId,
                Name = analyticTable.Name,

            };
        }
        #endregion
    }
}
