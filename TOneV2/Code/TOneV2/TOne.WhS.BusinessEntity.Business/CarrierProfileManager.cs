﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager : IBusinessEntityManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfileDetail> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();

            Func<CarrierProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                  (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || (prod.Settings.CountryId.HasValue && input.Query.CountriesIds.Contains(prod.Settings.CountryId.Value)))
                 &&
                 (input.Query.CarrierProfileIds == null || input.Query.CarrierProfileIds.Contains(prod.CarrierProfileId));

                var resultProcessingHandler = new ResultProcessingHandler<CarrierProfileDetail>()
                {
                    ExportExcelHandler = new CarrierProfileDetailExportExcelHandler()
                };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierProfiles.ToBigResult(input, filterExpression, CarrierProfileDetailMapper), resultProcessingHandler);
        }
        
        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            var carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.GetRecord(carrierProfileId);
        }
        
        public string GetCarrierProfileName(int carrierProfileId)
        {
            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);
            return carrierProfile != null ? carrierProfile.Name : null;
        }
        
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            return GetCachedCarrierProfiles().MapRecords(CarrierProfileInfoMapper).OrderBy(x => x.Name);
        }
        
        public TOne.Entities.InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            ValidateCarrierProfileToAdd(carrierProfile);

            TOne.Entities.InsertOperationOutput<CarrierProfileDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierProfileId = -1;

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierProfile, out carrierProfileId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                carrierProfile.CarrierProfileId = carrierProfileId;
                insertOperationOutput.InsertedObject = CarrierProfileDetailMapper(carrierProfile);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        
        public TOne.Entities.UpdateOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfileToEdit carrierProfile)
        {
            ValidateCarrierProfileToEdit(carrierProfile);

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();

            bool updateActionSucc = dataManager.Update(carrierProfile);
            TOne.Entities.UpdateOperationOutput<CarrierProfileDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CarrierProfileDetailMapper(this.GetCarrierProfile(carrierProfile.CarrierProfileId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public void UpdateCarrierProfileExtendedSetting<T>(int carrierProfileId, T extendedSettings)
        {
            string extendedSettingName = typeof(T).FullName;

            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);

            Dictionary<string, T> extendedSettingsDic = carrierProfile.ExtendedSettings as Dictionary<string, T>;
            if (extendedSettingsDic == null)
                extendedSettingsDic = new Dictionary<string, T>();

            T exitingExtendedSettings ;
            if (extendedSettingsDic.TryGetValue(extendedSettingName, out exitingExtendedSettings))
            {
                extendedSettingsDic[extendedSettingName] = extendedSettings;
            }
            else
            {
                extendedSettingsDic.Add(extendedSettingName, extendedSettings);
            }
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            dataManager.UpdateExtendedSettings(carrierProfileId, extendedSettingsDic);
        }

        public T GetExtendedSettingsObject<T>(int carrierProfileId)
        {
            string extendedSettingName = typeof(T).FullName;
            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);

            Dictionary<string, T> extendedSettingsDic = carrierProfile.ExtendedSettings as Dictionary<string, T>;

            T exitingExtendedSettings;
            if (extendedSettingsDic != null)
            {
                extendedSettingsDic.TryGetValue(extendedSettingName, out exitingExtendedSettings);
                if (exitingExtendedSettings != null)
                    return exitingExtendedSettings;
                else
                    return default(T);
            }
            else
                return default(T);
         }

       
        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCarrierProfileName(Convert.ToInt32(context.EntityId));
        }
        
        public IEnumerable<CarrierProfile> GetCarrierProfiles()
        {
            return GetCachedCarrierProfiles().Values;
        }

        public bool IsCarrierProfileDeleted(int carrierProfileId)
        {
            var carrierProfiles = this.GetCachedCarrierProfilesWithDeleted();
            CarrierProfile carrierProfile = carrierProfiles.GetRecord(carrierProfileId);
            
            if (carrierProfile == null)
                throw new DataIntegrityValidationException(string.Format("Carrier Profile with Id {0} is not found", carrierProfileId));

            return carrierProfile.IsDeleted;
        }
        public int GetCarrierProfileCurrencyId(int carrierProfileId)
        {
            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);
            if (carrierProfile == null)
                throw new NullReferenceException("carrierProfile");
            if (carrierProfile.Settings == null)
                throw new NullReferenceException("carrierProfile.Settings");
            return carrierProfile.Settings.CurrencyId;
        }
        public CompanySetting GetCompanySetting(int carrierProfileId)
        {
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            return configManager.GetDefaultCompanySetting();
        }
        #endregion

        #region Validation Methods

        void ValidateCarrierProfileToAdd(CarrierProfile carrierProfile)
        {
            ValidateCarrierProfile(carrierProfile.Name, carrierProfile.Settings);
        }

        void ValidateCarrierProfileToEdit(CarrierProfileToEdit carrierProfile)
        {
            ValidateCarrierProfile(carrierProfile.Name, carrierProfile.Settings);
        }

        void ValidateCarrierProfile(string cpName, CarrierProfileSettings cpSettings)
        {
            if (String.IsNullOrWhiteSpace(cpName))
                throw new MissingArgumentValidationException("CarrierProfile.Name");

            if (cpSettings == null)
                throw new MissingArgumentValidationException("CarrierProfile.Settings");

            if (String.IsNullOrWhiteSpace(cpSettings.Company))
                throw new MissingArgumentValidationException("CarrierProfile.Settings.Company");
        }

        #endregion

        #region Private Members
        
        public Dictionary<int, CarrierProfile> GetCachedCarrierProfiles()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierProfiles",
               () =>
               {
                   Dictionary<int, CarrierProfile> allCarrierProfile = this.GetCachedCarrierProfilesWithDeleted();
                   Dictionary<int, CarrierProfile> carrierProfiles = new Dictionary<int, CarrierProfile>();
                   
                   foreach (CarrierProfile item in allCarrierProfile.Values)
                   {
                       if (!item.IsDeleted)
                           carrierProfiles.Add(item.CarrierProfileId, item);
                   }

                   return carrierProfiles;
               });
        }

        private Dictionary<int, CarrierProfile> GetCachedCarrierProfilesWithDeleted()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("AllarrierProfiles",
               () =>
               {
                   ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
                   IEnumerable<CarrierProfile> carrierProfiles = dataManager.GetCarrierProfiles();
                   return carrierProfiles.ToDictionary(cn => cn.CarrierProfileId, cn => cn);
               });
        }

        
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICarrierProfileDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierProfilesUpdated(ref _updateHandle);
            }
        }

        private class CarrierProfileDetailExportExcelHandler : ExcelExportHandler<CarrierProfileDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CarrierProfileDetail> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Carrier Profiles";

                sheet.Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Carrier Profile Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Company" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Country" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                    row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CarrierProfileId });
                    row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                    row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Settings.Company });
                    row.Cells.Add(new ExportExcelCell() { Value = record.CountryName });
                    sheet.Rows.Add(row);
                }

                context.MainSheet = sheet;
            }
        }

        #endregion

        #region  Mappers
        private CarrierProfileInfo CarrierProfileInfoMapper(CarrierProfile carrierProfile)
        {
            return new CarrierProfileInfo()
            {
                CarrierProfileId = carrierProfile.CarrierProfileId,
                Name = carrierProfile.Name,
            };
        }
        private CarrierProfileDetail CarrierProfileDetailMapper(CarrierProfile carrierProfile)
        {
            CarrierProfileDetail carrierProfileDetail = new CarrierProfileDetail();
            carrierProfileDetail.Entity = carrierProfile;

            CountryManager countryManager = new CountryManager();
            if (carrierProfile.Settings != null && carrierProfile.Settings.CountryId.HasValue)
                carrierProfileDetail.CountryName = countryManager.GetCountryName(carrierProfile.Settings.CountryId.Value);

            return carrierProfileDetail;
        }
        #endregion

        private BusinessEntityTechnicalSettingsData GetBusinessEntitySettingData()
        {
            SettingManager settingManager = new SettingManager();
            BusinessEntityTechnicalSettingsData setting = settingManager.GetSetting<BusinessEntityTechnicalSettingsData>(BusinessEntityTechnicalSettingsData.BusinessEntityTechnicalSettings);

            if (setting == null)
                throw new NullReferenceException("BusinessEntityTechnicalSettingsData");

            return setting;
        }

        public List<VRTaxItemDefinition> GetTaxesDefinition()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();

            if (setting.TaxesDefinition == null)
                throw new NullReferenceException("setting.TaxesDefinition");

            return setting.TaxesDefinition.ItemDefinitions;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCarrierProfile(context.EntityId);
        }
        
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();
            if (allCarrierProfiles == null)
                return null;
            else
                return allCarrierProfiles.Values.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}
