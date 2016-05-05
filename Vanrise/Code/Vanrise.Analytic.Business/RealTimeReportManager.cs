using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
namespace Vanrise.Analytic.Business
{
    public class RealTimeReportManager
    {
    
        #region Fields

        int _loggedInUserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId();

        #endregion

        #region Public Methods
        public IEnumerable<RealTimeReportInfo> GetRealTimeReportsInfo(RealTimeReportInfoFilter filter)
        {
            var realTimeReports = GetCachedRealTimeReports();
            return realTimeReports.MapRecords(RealTimeReportInfoMapper, x => x.AccessType == AccessType.Public || x.UserID == _loggedInUserId);
        }
        public RealTimeReport GetRealTimeReportById(int realTimeReportId)
        {
            var realTimeReports = GetCachedRealTimeReports();
            return realTimeReports.GetRecord(realTimeReportId);
        }
        public Vanrise.Entities.InsertOperationOutput<RealTimeReport> AddRealTimeReport(RealTimeReport realTimeReport)
        {
            InsertOperationOutput<RealTimeReport> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<RealTimeReport>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int realTimeReportId = -1;

            IRealTimeReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IRealTimeReportDataManager>();
            bool insertActionSucc = dataManager.AddRealTimeReport(realTimeReport, out realTimeReportId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                realTimeReport.RealTimeReportId = realTimeReportId;
                insertOperationOutput.InsertedObject = realTimeReport;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<RealTimeReport> UpdateRealTimeReport(RealTimeReport realTimeReport)
        {
            IRealTimeReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IRealTimeReportDataManager>();
            bool updateActionSucc = dataManager.UpdateRealTimeReport(realTimeReport);
            UpdateOperationOutput<RealTimeReport> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<RealTimeReport>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = realTimeReport;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        Dictionary<int, RealTimeReport> GetCachedRealTimeReports()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedRealTimeReports",
                () =>
                {
                    IRealTimeReportDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IRealTimeReportDataManager>();
                    IEnumerable<RealTimeReport> realTimeReports = dataManager.GetRealTimeReports();
                    return realTimeReports.ToDictionary(x => x.RealTimeReportId, realTimeReport => realTimeReport);
                });
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            IRealTimeReportDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IRealTimeReportDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRealTimeReportUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers
        RealTimeReportInfo RealTimeReportInfoMapper(RealTimeReport realTimeReport)
        {
            return new RealTimeReportInfo()
            {
                RealTimeReportId = realTimeReport.RealTimeReportId,
                Name = realTimeReport.Name,
            };
        }
        #endregion
    }
}
