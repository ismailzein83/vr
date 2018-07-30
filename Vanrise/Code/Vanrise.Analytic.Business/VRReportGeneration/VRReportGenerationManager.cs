using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.Caching;
using System.ComponentModel;

namespace Vanrise.Analytic.Business
{
    public class VRReportGenerationManager
    {

        #region Public Methods
        public IDataRetrievalResult<VRReportGenerationDetail> GetFilteredVRReportGenerations(DataRetrievalInput<VRReportGenerationQuery> input)
        {
            var allVRReportGenerations = GetCachedVRReportGenerations();
            var userid = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            Func<VRReportGeneration, bool> filterExpression = (vRReportGeneration) =>
            {
                if (input.Query.Name != null && !vRReportGeneration.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.Owner == ReportOwner.OnlyMyReports && vRReportGeneration.CreatedBy != userid)
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allVRReportGenerations.ToBigResult(input, filterExpression, VRReportGenerationDetailMapper));

        }
        public VRReportGeneration GetVRReportGenerationHistoryDetailbyHistoryId(int reportId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var vRReportGeneration = s_vrObjectTrackingManager.GetObjectDetailById(reportId);
            return vRReportGeneration.CastWithValidate<VRReportGeneration>("VRReportGeneration : historyId ", reportId);
        }

        public InsertOperationOutput<VRReportGenerationDetail> AddVRReportGeneration(VRReportGeneration vRReportGeneration)
        {
            IVRReportGenerationDataManager vRReportGenerationDataManager = AnalyticDataManagerFactory.GetDataManager<IVRReportGenerationDataManager>();
            InsertOperationOutput<VRReportGenerationDetail> insertOperationOutput = new InsertOperationOutput<VRReportGenerationDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long reportId = -1;
            vRReportGeneration.CreatedBy = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            vRReportGeneration.LastModifiedBy = vRReportGeneration.CreatedBy;
            bool insertActionSuccess = vRReportGenerationDataManager.Insert(vRReportGeneration, out reportId);
            if (insertActionSuccess)
            {                
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                VRReportGeneration addedVRReportGeneration = this.GetVRReportGeneration(reportId);
                VRActionLogger.Current.TrackAndLogObjectAdded(VRReportGenerationLoggableEntity.Instance, addedVRReportGeneration);
                insertOperationOutput.InsertedObject = VRReportGenerationDetailMapper(addedVRReportGeneration);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public VRReportGeneration GetVRReportGeneration(long reportId)
        {
            return GetVRReportGeneration(reportId, false);
        }
        public VRReportGeneration GetVRReportGeneration(long reportId, bool isViewedFromUI)
        {
            var vRReportGeneration = GetCachedVRReportGenerations().GetRecord(reportId);
            if (vRReportGeneration != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(VRReportGenerationLoggableEntity.Instance, vRReportGeneration);
            return vRReportGeneration;
        }
        public string GetVRReportGenerationName(long reportId)
        {
            var vRReportGeneration = GetVRReportGeneration(reportId);
            if (vRReportGeneration != null)
                return vRReportGeneration.Name;
            else
                return null;
        }
        
        public UpdateOperationOutput<VRReportGenerationDetail> UpdateVRReportGeneration(VRReportGeneration vRReportGeneration)
        {
            IVRReportGenerationDataManager vRReportGenerationDataManager = AnalyticDataManagerFactory.GetDataManager<IVRReportGenerationDataManager>();
            UpdateOperationOutput<VRReportGenerationDetail> updateOperationOutput = new UpdateOperationOutput<VRReportGenerationDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            vRReportGeneration.LastModifiedBy = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSuccess = vRReportGenerationDataManager.Update(vRReportGeneration);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                VRReportGeneration updatedVRReportGeneration = this.GetVRReportGeneration(vRReportGeneration.ReportId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRReportGenerationLoggableEntity.Instance, updatedVRReportGeneration);
                updateOperationOutput.UpdatedObject = VRReportGenerationDetailMapper(updatedVRReportGeneration);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public IEnumerable<VRReportGenerationActionReportActionConfig> GetReportActionTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<VRReportGenerationActionReportActionConfig>(VRReportGenerationActionReportActionConfig.EXTENSION_TYPE);
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRReportGenerationDataManager vRReportGenerationDataManager = AnalyticDataManagerFactory.GetDataManager<IVRReportGenerationDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return vRReportGenerationDataManager.AreVRReportGenerationUpdated(ref _updateHandle);
            }
        }

        private class VRReportGenerationLoggableEntity : VRLoggableEntityBase
        {
            public static VRReportGenerationLoggableEntity Instance = new VRReportGenerationLoggableEntity();

            private VRReportGenerationLoggableEntity()
            {

            }

            static VRReportGenerationManager s_vRReportGeneration = new VRReportGenerationManager();

            public override string EntityUniqueName
            {
                get { return "VR_Analytic_ReportGeneration"; }
            }

            public override string ModuleName
            {
                get { return "Analytic"; }
            }

            public override string EntityDisplayName
            {
                get { return "ReportGeneration"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Analytic_ReportGeneration_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRReportGeneration vRReportGeneration = context.Object.CastWithValidate<VRReportGeneration>("context.Object");
                return vRReportGeneration.ReportId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRReportGeneration vRReportGeneration = context.Object.CastWithValidate<VRReportGeneration>("context.Object");
                return s_vRReportGeneration.GetVRReportGenerationName(vRReportGeneration.ReportId);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<long, VRReportGeneration> GetCachedVRReportGenerations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedVRReportGenerations", () =>
               {
                   IVRReportGenerationDataManager vRReportGenerationDataManager = AnalyticDataManagerFactory.GetDataManager<IVRReportGenerationDataManager>();
                   List<VRReportGeneration> vRReportGenerations = vRReportGenerationDataManager.GetVRReportGenerations();
                   return vRReportGenerations.ToDictionary(vRReportGeneration => vRReportGeneration.ReportId, vRReportGeneration => vRReportGeneration);
               });
        }
        #endregion

        #region Mappers
        private VRReportGenerationDetail VRReportGenerationDetailMapper(VRReportGeneration vRReportGeneration)
        {
            return new VRReportGenerationDetail
            {
                Name = vRReportGeneration.Name,
                ReportId = vRReportGeneration.ReportId,
                Description = vRReportGeneration.Description,
                CreatedBy = vRReportGeneration.CreatedBy,
                CreatedTime = vRReportGeneration.CreatedTime,
                AccessLevel = Utilities.GetEnumAttribute<AccessLevel, DescriptionAttribute>(vRReportGeneration.AccessLevel).Description,
                LastModifiedBy = vRReportGeneration.LastModifiedBy,
                LastModifiedTime = vRReportGeneration.LastModifiedTime
            };
        }


        #endregion

    }
}