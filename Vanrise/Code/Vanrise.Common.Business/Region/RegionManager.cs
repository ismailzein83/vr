using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class RegionManager : IBusinessEntityManager
    {
        #region Public Methods

        public IDataRetrievalResult<RegionDetail> GetFilteredRegions(Vanrise.Entities.DataRetrievalInput<RegionQuery> input)
        {
            var allRegions = GetCachedRegions();

            Func<Region, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CountryIds == null || input.Query.CountryIds.Contains(prod.CountryId));

            ResultProcessingHandler<RegionDetail> handler = new ResultProcessingHandler<RegionDetail>()
            {
                ExportExcelHandler = new RegionExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(RegionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRegions.ToBigResult(input, filterExpression, RegionDetailMapper), handler);
        }

        public Region GetRegionHistoryDetailbyHistoryId(int RegionHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var Region = s_vrObjectTrackingManager.GetObjectDetailById(RegionHistoryId);
            return Region.CastWithValidate<Region>("Region : historyId ", RegionHistoryId);
        }

        public IEnumerable<RegionInfo> GetRegionsInfo(RegionInfoFilter filter)
        {
            Func<Region, bool> filterExpression = null;
            if (filter!=null)
             filterExpression = (x) =>
            {
                if (filter.CountryId.HasValue  && x.CountryId != filter.CountryId.Value)
                    return false;             
                return true;
            };
            return this.GetCachedRegions().MapRecords(RegionInfoMapper,filterExpression).OrderBy(Region => Region.Name);
        }

        public IEnumerable<int> GetDistinctCountryIdsByRegionIds(IEnumerable<int> RegionIds)
        {
            return this.GetCachedRegions().MapRecords(Region => Region.CountryId, Region => RegionIds.Contains(Region.RegionId)).Distinct();
        }

        public Region GetRegion(int RegionId, bool isViewedFromUI)
        {
            var Regions = GetCachedRegions();
            var Region= Regions.GetRecord(RegionId);
            if (Region != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(RegionLoggableEntity.Instance, Region);
            return Region;
        }
        public Region GetRegion(int RegionId)
        {
            
            return GetRegion(RegionId,false);
        }

        public Vanrise.Entities.InsertOperationOutput<RegionDetail> AddRegion(Region Region)
        {
            Vanrise.Entities.InsertOperationOutput<RegionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<RegionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int RegionId = -1;

            IRegionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRegionDataManager>();

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            Region.CreatedBy = loggedInUserId;
            Region.LastModifiedBy = loggedInUserId;

            bool insertActionSucc = dataManager.Insert(Region, out RegionId);
            if (insertActionSucc)
            {
                Region.RegionId = RegionId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(RegionLoggableEntity.Instance, Region);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RegionDetailMapper(Region);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<RegionDetail> UpdateRegion(Region Region)
        {
            IRegionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRegionDataManager>();

            Region.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            bool updateActionSucc = dataManager.Update(Region);
            Vanrise.Entities.UpdateOperationOutput<RegionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<RegionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(RegionLoggableEntity.Instance, Region);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RegionDetailMapper(Region);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public string GetRegionName(int RegionId)
        {
            var Region = this.GetRegion(RegionId);
            if (Region != null)
                return Region.Name;
            else
                return null;
        }

        public IEnumerable<Region> GetAllRegions()
        {
            var allRegions = GetCachedRegions();
            if (allRegions == null)
                return null;

            return allRegions.Values;
        }

        #endregion

        #region Private Classes
        private class RegionExcelExportHandler : ExcelExportHandler<RegionDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RegionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Regions",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Region Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.RegionId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.CountryName });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRegionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IRegionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRegionsUpdated(ref _updateHandle);
            }
        }

        private class RegionLoggableEntity : VRLoggableEntityBase
        {
            public static RegionLoggableEntity Instance = new RegionLoggableEntity();

            private RegionLoggableEntity()
            {

            }

            static RegionManager s_RegionManager = new RegionManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_Region"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Region"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_Region_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Region Region = context.Object.CastWithValidate<Region>("context.Object");
                return Region.RegionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Region Region = context.Object.CastWithValidate<Region>("context.Object");
                return s_RegionManager.GetRegionName(Region.RegionId);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, Region> GetCachedRegions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRegions",
              () =>
              {
                  IRegionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IRegionDataManager>();
                  IEnumerable<Region> Regions = dataManager.GetRegions();
                  return Regions.ToDictionary(c => c.RegionId, c => c);
              });
        }

        #endregion

        #region Mappers

        public RegionDetail RegionDetailMapper(Region Region)
        {
            RegionDetail RegionDetail = new RegionDetail();

            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(Region.CountryId);

            RegionDetail.Entity = Region;
            RegionDetail.CountryName = (country != null ? country.Name : string.Empty);
            return RegionDetail;
        }

        public RegionInfo RegionInfoMapper(Region Region)
        {
            RegionInfo RegionInfo = new RegionInfo();
            RegionInfo.RegionId = Region.RegionId;
            RegionInfo.Name = Region.Name;
            RegionInfo.CountryId = Region.CountryId;
            return RegionInfo;
        }

        #endregion


        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetRegionName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var country = context.Entity as Country;
            return country.CountryId;
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllRegions().Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetRegion(context.EntityId);
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
