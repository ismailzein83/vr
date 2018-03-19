using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class CityManager
    {
        #region Public Methods

        public IDataRetrievalResult<CityDetail> GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQuery> input)
        {
            var allCities = GetCachedCities();

            Func<City, bool> filterExpression = (x) =>
            {
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.CountryIds != null && !input.Query.CountryIds.Contains(x.CountryId))
                    return false;
                if (input.Query.RegionIds != null)
                {
                    if (x.Settings == null)
                        return false;
                    if (x.Settings.RegionId == null)
                        return false;
                    if (!input.Query.RegionIds.Contains(x.Settings.RegionId.Value))
                        return false;
                }
                return true;
            };

            ResultProcessingHandler<CityDetail> handler = new ResultProcessingHandler<CityDetail>()
            {
                ExportExcelHandler = new CityExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(CityLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCities.ToBigResult(input, filterExpression, CityDetailMapper), handler);
        }

        public City GetCityHistoryDetailbyHistoryId(int cityHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var city = s_vrObjectTrackingManager.GetObjectDetailById(cityHistoryId);
            return city.CastWithValidate<City>("City : historyId ", cityHistoryId);
        }

        public IEnumerable<CityInfo> GetCitiesInfo(int countryId)
        {
            return this.GetCachedCities().MapRecords(CityInfoMapper, city => city.CountryId == countryId).OrderBy(city => city.Name);
        }

        public IEnumerable<City> GetCitiesByCountryId(int countryId)
        {
            return this.GetCachedCitiesByCountryId().GetRecord(countryId);
        }

        public IEnumerable<int> GetDistinctCountryIdsByCityIds(IEnumerable<int> cityIds)
        {
            return this.GetCachedCities().MapRecords(city => city.CountryId, city => cityIds.Contains(city.CityId)).Distinct();
        }

        public City GetCity(int cityId, bool isViewedFromUI)
        {
            var cities = GetCachedCities();
            var city = cities.GetRecord(cityId);
            if (city != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(CityLoggableEntity.Instance, city);
            return city;
        }
        public City GetCity(int cityId)
        {

            return GetCity(cityId, false);
        }

        public City GetCityBySourceId(string sourceId)
        {
            return GetCachedCitiesBySourceId().GetRecord(sourceId);
        }

        public City GetCityByName(int countryId, string name)
        {
            if (name == null)
                return null;
            string nameLower = name.ToLower();
            IEnumerable<City> cities = GetCitiesByCountryId(countryId);
            return cities.FindRecord(c => c.Name.ToLower() == nameLower);
        }

        public Vanrise.Entities.InsertOperationOutput<CityDetail> AddCity(City city)
        {
            Vanrise.Entities.InsertOperationOutput<CityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int cityId = -1;

            ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            city.CreatedBy = loggedInUserId;
            city.LastModifiedBy = loggedInUserId;

            bool insertActionSucc = dataManager.Insert(city, out cityId);
            if (insertActionSucc)
            {
                city.CityId = cityId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(CityLoggableEntity.Instance, city);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CityDetailMapper(city);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<CityDetail> UpdateCity(City city)
        {
            ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();

            city.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();
            bool updateActionSucc = dataManager.Update(city);
            Vanrise.Entities.UpdateOperationOutput<CityDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(CityLoggableEntity.Instance, city);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CityDetailMapper(city);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public string GetCityName(int cityId)
        {
            var city = this.GetCity(cityId);
            if (city != null)
                return city.Name;
            else
                return null;
        }

        public int? GetCityRegionId(int cityId)
        {
            City city = GetCity(cityId);
            city.ThrowIfNull("city", cityId);
            if (city.Settings == null)
                return null;
            return city.Settings.RegionId;
        }

        #endregion

        #region Private Classes
        private class CityExcelExportHandler : ExcelExportHandler<CityDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CityDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Cities",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "City Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Region" });
                

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CityId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.CountryName });
                            row.Cells.Add(new ExportExcelCell { Value = record.RegionName });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICityDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCitiesUpdated(ref _updateHandle);
            }
        }

        private class CityLoggableEntity : VRLoggableEntityBase
        {
            public static CityLoggableEntity Instance = new CityLoggableEntity();

            private CityLoggableEntity()
            {

            }

            static CityManager s_cityManager = new CityManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_City"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "City"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_City_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                City city = context.Object.CastWithValidate<City>("context.Object");
                return city.CityId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                City city = context.Object.CastWithValidate<City>("context.Object");
                return s_cityManager.GetCityName(city.CityId);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, City> GetCachedCities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCities",
              () =>
              {
                  ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();
                  IEnumerable<City> cities = dataManager.GetCities();
                  return cities.ToDictionary(c => c.CityId, c => c);
              });
        }

        private Dictionary<string, City> GetCachedCitiesBySourceId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCitiesBySourceId",
              () =>
              {
                  return GetCachedCities().Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value);
              });
        }

        private Dictionary<int, List<City>> GetCachedCitiesByCountryId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCitiesByCountryId",
              () =>
              {
                  Dictionary<int, List<City>> citiesByCountryId = new Dictionary<int, List<City>>();
                  var cities = GetCachedCities();
                  if(cities != null)
                  {
                      foreach(var c in cities.Values)
                      {
                          citiesByCountryId.GetOrCreateItem(c.CountryId).Add(c);
                      }
                  }
                  return citiesByCountryId;
              });
        }

        #endregion

        #region Mappers

        public CityDetail CityDetailMapper(City city)
        {
            CityDetail cityDetail = new CityDetail();

            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(city.CountryId);
            string regionName = null;
            if (city.Settings != null && city.Settings.RegionId.HasValue)
            {
                RegionManager regionManager = new RegionManager();
                Region region = regionManager.GetRegion(city.Settings.RegionId.Value);
                regionName = (region != null ? region.Name : string.Empty);
            }


            cityDetail.Entity = city;
            cityDetail.CountryName = (country != null ? country.Name : string.Empty);
            cityDetail.RegionName = regionName;
            return cityDetail;
        }

        public CityInfo CityInfoMapper(City city)
        {
            CityInfo cityInfo = new CityInfo();
            cityInfo.CityId = city.CityId;
            cityInfo.Name = city.Name;
            cityInfo.CountryId = city.CountryId;
            return cityInfo;
        }

        #endregion
    }
}
