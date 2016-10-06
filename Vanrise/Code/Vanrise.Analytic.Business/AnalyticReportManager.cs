﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Analytic.Data;
using Vanrise.Common.Business;
namespace Vanrise.Analytic.Business
{
    public class AnalyticReportManager : IAnalyticReportManager
    {
        #region Fields

        int _loggedInUserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId();

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<AnalyticReportDetail> GetFilteredAnalyticReports(Vanrise.Entities.DataRetrievalInput<AnalyticReportQuery> input)
        {
            var analyticReports = GetCachedAnalyticReports();

            Func<AnalyticReport, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, analyticReports.ToBigResult(input, filterExpression, AnalyticReportDetailMapper));
        }
        public IEnumerable<AnalyticReportInfo> GetAnalyticReportsInfo(AnalyticReportInfoFilter filter)
        {
            var analyticReports = GetCachedAnalyticReports();
            if(filter !=null)
            {
                Func<AnalyticReport, bool> filterExpression = (prod) =>
                 (filter.TypeId == null || prod.Settings.ConfigId == filter.TypeId)
                 && (filter.TypeName == null || prod.Settings.ConfigId == GetAnalyticReportConfigTypeByName(filter.TypeName).ExtensionConfigurationId)
                 && (prod.AccessType == AccessType.Public || prod.UserID == _loggedInUserId);
                 return analyticReports.MapRecords(AnalyticReportInfoMapper, filterExpression);
            }

            return analyticReports.MapRecords(AnalyticReportInfoMapper, x => x.AccessType == AccessType.Public || x.UserID == _loggedInUserId);
        }
        public AnalyticReport GetAnalyticReportById(Guid analyticReportId)
        {
            var analyticReports = GetCachedAnalyticReports();
            return analyticReports.GetRecord(analyticReportId);
        }
        public Vanrise.Entities.InsertOperationOutput<AnalyticReportDetail> AddAnalyticReport(AnalyticReport analyticReport)
        {
            InsertOperationOutput<AnalyticReportDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AnalyticReportDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            analyticReport.AnalyticReportId = Guid.NewGuid();
            IAnalyticReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticReportDataManager>();
            analyticReport.UserID = _loggedInUserId;
            bool insertActionSucc = dataManager.AddAnalyticReport(analyticReport);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = AnalyticReportDetailMapper(analyticReport);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AnalyticReportDetail> UpdateAnalyticReport(AnalyticReport analyticReport)
        {
            IAnalyticReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticReportDataManager>();
            bool updateActionSucc = dataManager.UpdateAnalyticReport(analyticReport);
            UpdateOperationOutput<AnalyticReportDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AnalyticReportDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject =AnalyticReportDetailMapper(analyticReport) ;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<AnalyticReportConfiguration> GetAnalyticReportConfigTypes()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<AnalyticReportConfiguration>(AnalyticReportConfiguration.EXTENSION_TYPE);
        }
        public AnalyticReportConfiguration GetAnalyticReportConfigTypeByName(string name)
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            var analyticReportConfiguration = manager.GetExtensionConfigurationByName<AnalyticReportConfiguration>(name, AnalyticReportConfiguration.EXTENSION_TYPE);
            if (analyticReportConfiguration == null)
                throw new NullReferenceException("analyticReportConfiguration");
            return analyticReportConfiguration;
        }
        #endregion

        #region Private Methods

        Dictionary<Guid, AnalyticReport> GetCachedAnalyticReports()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAnalyticReports",
                () =>
                {
                    IAnalyticReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticReportDataManager>();
                    IEnumerable<AnalyticReport> analyticReports = dataManager.GetAnalyticReports();
                    return analyticReports.ToDictionary(x => x.AnalyticReportId, analyticReport => analyticReport);
                });
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            IAnalyticReportDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticReportDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAnalyticReportUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers
        AnalyticReportDetail AnalyticReportDetailMapper(AnalyticReport analyticReport)
        {
            return new AnalyticReportDetail
            {
                Entity = analyticReport
            };
        }
        AnalyticReportInfo AnalyticReportInfoMapper(AnalyticReport analyticReport)
        {
            return new AnalyticReportInfo()
            {
                AnalyticReportId = analyticReport.AnalyticReportId,
                Name = analyticReport.Name,
            };
        }
        #endregion
    }
}
