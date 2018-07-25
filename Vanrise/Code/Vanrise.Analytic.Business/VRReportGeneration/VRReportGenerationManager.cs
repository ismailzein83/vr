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
                insertOperationOutput.InsertedObject = VRReportGenerationDetailMapper(this.GetVRReportGeneration(reportId));
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public VRReportGeneration GetVRReportGeneration(long reportId)
        {
            return GetCachedVRReportGenerations().GetRecord(reportId);
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
                updateOperationOutput.UpdatedObject = VRReportGenerationDetailMapper(this.GetVRReportGeneration(vRReportGeneration.ReportId));
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