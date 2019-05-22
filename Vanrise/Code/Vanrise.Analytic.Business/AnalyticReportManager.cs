using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;

namespace Vanrise.Analytic.Business
{
    public class AnalyticReportManager : IAnalyticReportManager
    {
        #region Fields

        int _loggedInUserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId();
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AnalyticReportDetail> GetFilteredAnalyticReports(Vanrise.Entities.DataRetrievalInput<AnalyticReportQuery> input)
        {
            var analyticReports = GetCachedAnalyticReports();

            Func<AnalyticReport, bool> filterExpression = (prod) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(prod.DevProjectId))
                    return false;

                if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.DevProjectIds != null && (!prod.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(prod.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(AnalyticReportLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, analyticReports.ToBigResult(input, filterExpression, AnalyticReportDetailMapper));
        }

        public IEnumerable<AnalyticReportInfo> GetAnalyticReportsInfo(AnalyticReportInfoFilter filter)
        {
            var analyticReports = GetCachedAnalyticReports();

            Func<AnalyticReport, bool> filterExpression = (item) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(item.DevProjectId))
                    return false;

                if (filter != null)
                {
                    if (filter.TypeId != null && item.Settings.ConfigId != filter.TypeId)
                        return false;
                    if (filter.TypeName != null && item.Settings.ConfigId != GetAnalyticReportConfigTypeByName(filter.TypeName).ExtensionConfigurationId)
                        return false;
                }

                if (item.AccessType != AccessType.Public && item.UserID != _loggedInUserId)
                    return false;

                return true;
            };

            return analyticReports.MapRecords(AnalyticReportInfoMapper, filterExpression);
        }

        public string GetAnalyticReportName(AnalyticReport analyticReport)
        {
            if (analyticReport != null)
               return analyticReport.Name;
            return null;
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
                VRActionLogger.Current.TrackAndLogObjectAdded(AnalyticReportLoggableEntity.Instance, analyticReport);
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(AnalyticReportLoggableEntity.Instance, analyticReport);
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
        public IEnumerable<string> CheckRecordStoragesAccess(Guid analyticReportId)
        {
            HashSet<string> availableSources = new HashSet<string>();
            var analyticReport = GetAnalyticReportById(analyticReportId);
            analyticReport.ThrowIfNull("analyticReport", analyticReportId);
            analyticReport.Settings.ThrowIfNull("analyticReport.Settings", analyticReportId);
            var dataRecordSearchPageSettings = analyticReport.Settings.CastWithValidate<DataRecordSearchPageSettings>("DataRecordSearchPageSettings");
            if(dataRecordSearchPageSettings != null && dataRecordSearchPageSettings.Sources != null)
            {
                DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
                var userId = SecurityContext.Current.GetLoggedInUserId();
               foreach(var source in dataRecordSearchPageSettings.Sources)
               {
                   if (dataRecordStorageManager.DoesUserHaveAccess(userId, source.RecordStorageIds) && dataRecordStorageManager.DoesUserHaveFieldsAccess(userId, source.RecordStorageIds, source.GridColumns.MapRecords(x => x.FieldName)))
                   {
                       availableSources.Add(source.Name);
                   }
                 
               }
            }
            return availableSources;
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

        private class AnalyticReportLoggableEntity : VRLoggableEntityBase
        {
            public static AnalyticReportLoggableEntity Instance = new AnalyticReportLoggableEntity();

            private AnalyticReportLoggableEntity()
            {

            }

            static AnalyticReportManager s_AnalyticReportManager = new AnalyticReportManager();

            public override string EntityUniqueName
            {
                get { return "VR_Analytic_AnalyticReport"; }
            }

            public override string ModuleName
            {
                get { return "Analytic"; }
            }

            public override string EntityDisplayName
            {
                get { return "Analytic Report"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Analytic_AnalyticReport_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                AnalyticReport analyticReport = context.Object.CastWithValidate<AnalyticReport>("context.Object");
                return analyticReport.AnalyticReportId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                AnalyticReport analyticReport = context.Object.CastWithValidate<AnalyticReport>("context.Object");
                return s_AnalyticReportManager.GetAnalyticReportName(analyticReport);
            }
        }

        #endregion

        #region Mappers

        AnalyticReportDetail AnalyticReportDetailMapper(AnalyticReport analyticReport)
        {
            string devProjectName = null;
            if (analyticReport.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(analyticReport.DevProjectId.Value);
            }
            return new AnalyticReportDetail
            {
                Entity = analyticReport,
                DevProjectName=devProjectName
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