using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class CountryManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<CountryDetail> GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            var allCountries = GetCachedCountries();

            Func<Country, bool> filterExpression = (prod) =>
               (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            ResultProcessingHandler<CountryDetail> handler = new ResultProcessingHandler<CountryDetail>()
            {
                ExportExcelHandler = new CountryExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(CountryLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression, CountryDetailMapper), handler);
        }
        public Country GetCountryHistoryDetailbyHistoryId(int countryHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var country = s_vrObjectTrackingManager.GetObjectDetailById(countryHistoryId);
            return country.CastWithValidate<Country>("Country : historyId ", countryHistoryId);
        }
        public IEnumerable<CountryInfo> GeCountriesInfo(CountryFilter filter)
        {
            IEnumerable<Country> allCountries = GetCachedCountries().Values;

            Func<Country, bool> filterFunc = null;
            List<object> customObjects = null;

            if (filter != null)
            {
                if (filter.Filters != null)
                {
                    customObjects = new List<object>();
                    foreach (ICountryFilter countryFilter in filter.Filters)
                        customObjects.Add(null);
                }

                filterFunc = (country) =>
                {
                    if (filter.ExcludedCountryIds != null && filter.ExcludedCountryIds.Contains(country.CountryId))
                        return false;

                    if (filter.Filters != null)
                    {
                        for (int i = 0; i < filter.Filters.Count(); i++)
                        {
                            var context = new CountryFilterContext() { Country = country, CustomObject = customObjects[i] };
                            if (filter.Filters.ElementAt(i).IsExcluded(context))
                                return false;
                            customObjects[i] = context.CustomObject;
                        }
                    }

                    return true;
                };
            }
            IEnumerable<Country> filteredCountries = (filterFunc != null) ? allCountries.FindAllRecords(filterFunc) : allCountries;
            return filteredCountries.MapRecords(CountryInfoMapper).OrderBy(x => x.Name);
        }

        public IEnumerable<Country> GetAllCountries()
        {
            var allCountries = GetCachedCountries();
            if (allCountries == null)
                return null;

            return allCountries.Values;
        }

       public IEnumerable<Country> GetCountriesByCountryIds(IEnumerable<int> countryIds)
        {
            if (countryIds == null)
                throw new ArgumentNullException("countryIds");
            return GetCachedCountries().Values.FindAllRecords(x => countryIds.Contains(x.CountryId));
        }

        public Country GetCountry(int countryId, bool isViewedFromUI)
        {
            var countries = GetCachedCountries();
            var country = countries.GetRecord(countryId);
            if (country != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(CountryLoggableEntity.Instance, country);
            return country;
        }
        public Country GetCountry(int countryId)
        {
           
            return  GetCountry (countryId,false);
        }
        public string GetCountryName(int countryId)
        {
            var country = GetCountry(countryId);
            if (country != null)
                return country.Name;
            else
                return null;
        }

        public Country GetCountry(string countryName)
        {
            if (countryName == null)
                return null;
            var countries = GetCachedCountriesByNames();
            return countries.GetRecord(countryName.ToLower());
        }

        public Country GetCountryBySourceId(string sourceId)
        {
            return GetCachedCountries().FindRecord(itm => itm.SourceId == sourceId);
        }

        public UploadCountryLog AddCountries(int fileId)
        {
            UploadCountryLog uploadCountryLog = new UploadCountryLog();

            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            ExportTableOptions options = new ExportTableOptions();
            options.CheckMixedValueType = true;
            Workbook wbk = new Workbook(fileStream);
            Worksheet worksheet = wbk.Worksheets[0];
            List<String> headers = new List<string>();
            headers.Add(worksheet.Cells[0, 0].StringValue);
            headers.Add("Result");
            headers.Add("Error Message");

            wbk.CalculateFormula();
            List<string> addedCountries = new List<string>();
            int count = 1;
            while (count < worksheet.Cells.Rows.Count)
            {
                string country = worksheet.Cells[count, 0].StringValue.Trim();
                addedCountries.Add(country);
                count++;
            }


            //Return Excel Result
            Workbook returnedExcel = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            returnedExcel.Worksheets.Clear();
            Worksheet CountryWorkSheet = returnedExcel.Worksheets.Add("Result");

            int rowIndex = 0;
            int colIndex = 0;

            foreach (var header in headers)
            {

                CountryWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                CountryWorkSheet.Cells[rowIndex, colIndex].PutValue(header);
                Cell cell = CountryWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0);
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            rowIndex++;
            colIndex = 0;


            foreach (var addedCountry in addedCountries)
            {
                CountryWorkSheet.Cells[rowIndex, colIndex].PutValue(addedCountry);
                colIndex++;

                Country country = GetCachedCountries().FindRecord(it => it.Name.Equals(addedCountry, StringComparison.InvariantCultureIgnoreCase));
                if (!String.IsNullOrEmpty(addedCountry))
                {
                    if (country == null)
                    {
                        country = new Country();
                        long startingId;
                        ReserveIDRange(1, out startingId);
                        country.CountryId = (int)startingId;
                        country.Name = addedCountry;

                        ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                        bool insertActionSucc = dataManager.Insert(country);
                        if (insertActionSucc)
                        {
                            CountryWorkSheet.Cells[rowIndex, colIndex].PutValue("Succeed");
                            uploadCountryLog.CountOfCountriesAdded++;
                            colIndex = 0;
                            rowIndex++;
                        }
                        else
                        {
                            CountryWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");
                            colIndex++;
                            CountryWorkSheet.Cells[rowIndex, colIndex].PutValue("Country already exists");
                            uploadCountryLog.CountOfCountriesExist++;
                            colIndex = 0;
                            rowIndex++;
                        }
                    }
                    else
                    {
                        CountryWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");
                        colIndex++;
                        CountryWorkSheet.Cells[rowIndex, colIndex].PutValue("Country already exists");
                        uploadCountryLog.CountOfCountriesExist++;
                        colIndex = 0;
                        rowIndex++;
                    }
                }
                else
                    colIndex = 0;
            }

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = returnedExcel.SaveToStream();

            VRFile saveFile = new VRFile()
            {
                Content = memoryStream.ToArray(),
                Name = "CountryLog",
                Extension = ".xlsx",
                IsTemp = true
            };
            VRFileManager manager = new VRFileManager();
            uploadCountryLog.fileID = manager.AddFile(saveFile);

            return uploadCountryLog;
        }

        public Vanrise.Entities.InsertOperationOutput<CountryDetail> AddCountry(Country country)
        {
            Vanrise.Entities.InsertOperationOutput<CountryDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CountryDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            long startingId;
            ReserveIDRange(1, out startingId);
            country.CountryId = (int)startingId;

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            bool insertActionSucc = dataManager.Insert(country);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(CountryLoggableEntity.Instance, country);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CountryDetailMapper(country);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public void AddCountryFromSource(Country country)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            country.CountryId = (int)startingId;

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            dataManager.InsertFromSource(country);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

        }

        public void UpdateCountryFromSource(Country country)
        {

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            dataManager.UpdateFromSource(country);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public Vanrise.Entities.UpdateOperationOutput<CountryDetail> UpdateCountry(Country country)
        {
            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();

            bool updateActionSucc = dataManager.Update(country);
            Vanrise.Entities.UpdateOperationOutput<CountryDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CountryDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(CountryLoggableEntity.Instance, country);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CountryDetailMapper(country);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public IEnumerable<SourceCountryReaderConfig> GetCountrySourceTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SourceCountryReaderConfig>(SourceCountryReaderConfig.EXTENSION_TYPE);
        }

        public Dictionary<string, long> GetExistingItemIds()
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedCountries())
            {
                if (item.Value.SourceId != null)
                {
                    //if (sourceItemIds.Contains(item.Value.SourceId))
                    existingItemIds.Add(item.Value.SourceId, (long)item.Value.CountryId);
                }
            }
            return existingItemIds;
        }

        public int GetCountryTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetCountryType());
        }

        public Type GetCountryType()
        {
            return this.GetType();
        }

        public byte[] DownloadCountryLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }

        public IEnumerable<int> GetCountryIdsBySubstring(string substring)
        {
            if (string.IsNullOrWhiteSpace(substring))
                throw new Vanrise.Entities.MissingArgumentValidationException("substring");

            Dictionary<int, Country> countriesById = GetCachedCountries();

            if (countriesById == null || countriesById.Count == 0)
                return null;

            string targetSubstring = substring.Trim().ToLower();
            IEnumerable<Country> countries = countriesById.Values.FindAllRecords(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name.Trim().ToLower().Contains(targetSubstring));

            if (countries == null || countries.Count() == 0)
                return null;

            return countries.MapRecords(x => x.CountryId);
        }

        public string GetDescription(IEnumerable<int> countryIds)
        {
            if (countryIds == null)
                return null;

            IEnumerable<Country> countries = GetCountriesByCountryIds(countryIds);
            if (countries == null)
                return null;

            return string.Join(", ", countries.Select(x => x.Name));
        }

        public IEnumerable<CountryCriteriaGroupConfig> GetCountryCriteriaGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CountryCriteriaGroupConfig>(CountryCriteriaGroupConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Classes
        private class CountryExcelExportHandler : ExcelExportHandler<CountryDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CountryDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Countries",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country Name", Width = 50});

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CountryId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class CountryLoggableEntity : VRLoggableEntityBase
        {
            public static CountryLoggableEntity Instance = new CountryLoggableEntity();

            private CountryLoggableEntity()

            {

            }

            static CountryManager s_countryManager = new CountryManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_Country"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Country"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_Country_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Country country = context.Object.CastWithValidate<Country>("context.Object");
                return country.CountryId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Country country = context.Object.CastWithValidate<Country>("context.Object");
                return s_countryManager.GetCountryName(country.CountryId);
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICountrytDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCountriesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        public Dictionary<int, Country> GetCachedCountries()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountries",
               () =>
               {
                   ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   return countries.ToDictionary(cn => cn.CountryId, cn => cn);
               });
        }

        public Dictionary<string, Country> GetCachedCountriesByNames()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountriesByName",
               () =>
               {
                   ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   Dictionary<string, Country> dic = new Dictionary<string, Country>();
                   if (countries != null)
                   {
                       foreach (var c in countries)
                       {
                           if (!dic.ContainsKey(c.Name.ToLower()))
                               dic.Add(c.Name.ToLower(), c);
                       }
                   }
                   return dic;
               });
        }

        private CountryDetail CountryDetailMapper(Country country)
        {
            CountryDetail countryDetail = new CountryDetail();

            countryDetail.Entity = country;
            return countryDetail;
        }

        private CountryInfo CountryInfoMapper(Country country)
        {
            CountryInfo countryInfo = new CountryInfo();

            countryInfo.CountryId = country.CountryId;
            countryInfo.Name = country.Name;
            return countryInfo;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(CountryManager), nbOfIds, out startingId);
        }

        #endregion

        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCountryName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var country = context.Entity as Country;
            return country.CountryId;
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllCountries().Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCountry(context.EntityId);
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
