﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Business.CarrierProfiles;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager : BaseBusinessEntityManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods

        public CarrierProfile GetCarrierProfileHistoryDetailbyHistoryId(int carrierProfileHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var carrierProfile = s_vrObjectTrackingManager.GetObjectDetailById(carrierProfileHistoryId);
            return carrierProfile.CastWithValidate<CarrierProfile>("Carrier Profile : historyId ", carrierProfileHistoryId);
        }

        #region CarrierProfile
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfileDetail> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();

            Func<CarrierProfile, bool> filterExpression = (prod) =>
            {
                if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.Company != null && !prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                    return false;
                if (input.Query.CountriesIds != null && input.Query.CountriesIds.Count != 0 && (prod.Settings.CountryId.HasValue && !input.Query.CountriesIds.Contains(prod.Settings.CountryId.Value)))
                    return false;
                if (input.Query.CarrierProfileIds != null && !input.Query.CarrierProfileIds.Contains(prod.CarrierProfileId))
                    return false;
                if (input.Query.PortalAccountEmail != null)
                {
                    PortalAccountManager portalAccountManager = new PortalAccountManager();
                    var portalAccounts = portalAccountManager.GetCarrierProfilePortalAccounts(prod.CarrierProfileId);
                    if (portalAccounts.FindRecord(x => x.Email.ToLower().Contains(input.Query.PortalAccountEmail.ToLower())) == null)
                        return false;
                }
                return true;
            };
        

            var resultProcessingHandler = new ResultProcessingHandler<CarrierProfileDetail>()
            {
                ExportExcelHandler = new CarrierProfileDetailExportExcelHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(CarrierProfileLoggableEntity.Instance, input);
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
            return GetCarrierProfileName(carrierProfile);
        }
        public CarrierProfileCarrierAccounts GetCarrierProfileCarrierAccountsByUserId(int userId)
        {
            var cachedCarrierProfilePortalAccountSettingsByUserId = GetCachedCarrierProfilePortalAccountSettingsByUserId(userId);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierProfileCarrierAccounts = new CarrierProfileCarrierAccounts
            {
                CarrierAccountIds = new List<int>()
            };
            if (cachedCarrierProfilePortalAccountSettingsByUserId != null && cachedCarrierProfilePortalAccountSettingsByUserId.Count > 0)
            {
                var carrierProfilePortalAccountSettings = cachedCarrierProfilePortalAccountSettingsByUserId.GetRecord(userId);
                if (carrierProfilePortalAccountSettings != null)
                {
                    carrierProfileCarrierAccounts.CarrierAccountIds = carrierProfilePortalAccountSettings.PortalCarrierAccounts.Select<PortalCarrierAccount, int>(x => x.CarrierAccountId).ToList();
                }
            }
            return carrierProfileCarrierAccounts;
        }

        public string GetCarrierProfileName(CarrierProfile carrierProfile)
        {
            return carrierProfile != null ? carrierProfile.Name : null;
        }
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            return GetCachedCarrierProfiles().MapRecords(CarrierProfileInfoMapper).OrderBy(x => x.Name);
        }

        public bool HasCarrierPortalAccess(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var carrierPortalConnection = connectionManager.GetVRConnection(connectionId);
            return carrierPortalConnection != null;
        }
        public CarrierContact GetCarrierContact(int carrierProfileId, CarrierContactType contactType)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId);
            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", carrierProfileId);
            carrierProfile.Settings.Contacts.ThrowIfNull("carrierProfile.Settings.Contacts", carrierProfileId);
            return carrierProfile.Settings.Contacts.FindRecord(x => x.Type == contactType);
        }
        public string GetAccountManagerName(int accountId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierProfileId = carrierAccountManager.GetCarrierProfileId(accountId);
            if (carrierProfileId.HasValue)
            {
                var accountManagerContact = GetCarrierContact(carrierProfileId.Value, CarrierContactType.AccountManagerContact);
                return accountManagerContact != null ? accountManagerContact.Description : null;
            }
            return null;
        }
        public InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            ValidateCarrierProfileToAdd(carrierProfile);

            InsertOperationOutput<CarrierProfileDetail> insertOperationOutput = new InsertOperationOutput<CarrierProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierProfileId = -1;

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            carrierProfile.Settings.CustomerActivationStatus = CarrierProfileActivationStatus.InActive;
            carrierProfile.Settings.SupplierActivationStatus = CarrierProfileActivationStatus.InActive;

            List<long> fileIds = new List<long>();
            if (carrierProfile.Settings != null)
            {
                if (carrierProfile.Settings.Documents != null && carrierProfile.Settings.Documents.Count > 0)
                    fileIds = carrierProfile.Settings.Documents.Select(itm => itm.FileId).ToList();
                if (carrierProfile.Settings.CompanyLogo.HasValue)
                    fileIds.Add(carrierProfile.Settings.CompanyLogo.Value);
            }

            if (fileIds != null && fileIds.Any())
                SetFilesUsed(fileIds, null);
            
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            carrierProfile.CreatedBy = loggedInUserId;
            carrierProfile.LastModifiedBy = loggedInUserId;

            bool insertActionSucc = dataManager.Insert(carrierProfile, out carrierProfileId);
            if (insertActionSucc)
            {
                if (fileIds != null && fileIds.Any())
                    SetFilesUsed(fileIds, carrierProfileId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                carrierProfile.CarrierProfileId = carrierProfileId;
                VRActionLogger.Current.TrackAndLogObjectAdded(CarrierProfileLoggableEntity.Instance, carrierProfile);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CarrierProfileDetailMapper(carrierProfile);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public UpdateOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfileToEdit carrierProfileToEdit)
        {
            CarrierProfile existingCarrierProfile = GetCarrierProfile(carrierProfileToEdit.CarrierProfileId);
            existingCarrierProfile.ThrowIfNull("existingCarrierProfile", carrierProfileToEdit.CarrierProfileId);
            ValidateCarrierProfileToEdit(carrierProfileToEdit);

            List<long> newFileIds = new List<long>();
            if (carrierProfileToEdit.Settings != null)
            {
                if (carrierProfileToEdit.Settings.Documents != null && carrierProfileToEdit.Settings.Documents.Count > 0)
                {
                    List<long> existingFileIds = null;
                    if (existingCarrierProfile.Settings != null && existingCarrierProfile.Settings.Documents != null && existingCarrierProfile.Settings.Documents.Count > 0)
                    {
                        existingFileIds = existingCarrierProfile.Settings.Documents.Select(itm => itm.FileId).ToList();
                    }
                    newFileIds = carrierProfileToEdit.Settings.Documents.Where(doc => existingFileIds == null || !existingFileIds.Contains(doc.FileId)).Select(itm => itm.FileId).ToList();
                }

                if (carrierProfileToEdit.Settings.CompanyLogo.HasValue)
                {
                    if (existingCarrierProfile.Settings == null || !existingCarrierProfile.Settings.CompanyLogo.HasValue || existingCarrierProfile.Settings.CompanyLogo != carrierProfileToEdit.Settings.CompanyLogo)
                        newFileIds.Add(carrierProfileToEdit.Settings.CompanyLogo.Value);
                }
            }


            if (newFileIds != null && newFileIds.Any())
                SetFilesUsed(newFileIds, carrierProfileToEdit.CarrierProfileId);

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            carrierProfileToEdit.Settings.CustomerActivationStatus = GetCarrierProfileCustomerActivationStatus(carrierProfileToEdit.CarrierProfileId);
            carrierProfileToEdit.Settings.SupplierActivationStatus = GetCarrierProfileSupplierActivationStatus(carrierProfileToEdit.CarrierProfileId);

            carrierProfileToEdit.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            bool updateActionSucc = dataManager.Update(carrierProfileToEdit);
            UpdateOperationOutput<CarrierProfileDetail> updateOperationOutput = new UpdateOperationOutput<CarrierProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var carrierProfile = GetCarrierProfile(carrierProfileToEdit.CarrierProfileId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(CarrierProfileLoggableEntity.Instance, carrierProfile);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CarrierProfileDetailMapper(this.GetCarrierProfile(carrierProfileToEdit.CarrierProfileId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        public IEnumerable<CarrierProfile> GetCarrierProfiles()
        {
            return GetCachedCarrierProfiles().Values;
        }
        public CarrierProfileActivationStatus GetCarrierProfileCustomerActivationStatus(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId);
            return carrierProfile.Settings.CustomerActivationStatus;
        }
        public CarrierProfileActivationStatus GetCarrierProfileSupplierActivationStatus(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId);
            return carrierProfile.Settings.SupplierActivationStatus;
        }
        public bool IsCarrierProfileActive(int carrierProfileId)
        {
            return IsCarrierProfileCustomerActive(carrierProfileId) || IsCarrierProfileSupplierActive(carrierProfileId);
        }
        public bool IsCarrierProfileCustomerActive(int carrierProfileId)
        {
            return GetCarrierProfileCustomerActivationStatus(carrierProfileId) != CarrierProfileActivationStatus.InActive;
        }
        public bool IsCarrierProfileSupplierActive(int carrierProfileId)
        {
            return GetCarrierProfileSupplierActivationStatus(carrierProfileId) != CarrierProfileActivationStatus.InActive;
        }
        public void EvaluateAndUpdateCarrierProfileStatus(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId);
            CarrierProfileActivationStatus supplierActivationStatus = carrierProfile.Settings.SupplierActivationStatus;
            CarrierProfileActivationStatus customerActivationStatus = carrierProfile.Settings.CustomerActivationStatus;
            EvaluateCarrierProfileStatus(carrierProfileId, true, true, ref supplierActivationStatus, ref  customerActivationStatus);

            if (carrierProfile.Settings.CustomerActivationStatus != customerActivationStatus || carrierProfile.Settings.SupplierActivationStatus != supplierActivationStatus)
            {
                var carrierProfileSettingsCopy = carrierProfile.Settings.VRDeepCopy();
                carrierProfileSettingsCopy.CustomerActivationStatus = customerActivationStatus;
                carrierProfileSettingsCopy.SupplierActivationStatus = supplierActivationStatus;
                ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
                bool updateActionSucc = dataManager.Update(new CarrierProfileToEdit
                {
                    CarrierProfileId = carrierProfile.CarrierProfileId,
                    CreatedTime = carrierProfile.CreatedTime,
                    Name = carrierProfile.Name,
                    Settings = carrierProfileSettingsCopy,
                    SourceId = carrierProfile.SourceId
                });
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VREventManager vrEventManager = new VREventManager();
                vrEventManager.ExecuteEventHandlersAsync(new CarrierProfileStatusChangedEventPayload { CarrierProfileId = carrierProfileId });
            }
        }

        public void EvaluateCarrierProfileStatus(int carrierProfileId, bool asCustomer, bool asSupplier, ref CarrierProfileActivationStatus supplierActivationStatus, ref CarrierProfileActivationStatus customerActivationStatus)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var profileCarrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierProfileId, asCustomer, asSupplier);
            if (profileCarrierAccounts != null)
            {
                if (asCustomer)
                {
                    customerActivationStatus = CarrierProfileActivationStatus.InActive;
                }
                if (asSupplier)
                {
                    supplierActivationStatus = CarrierProfileActivationStatus.InActive;
                }
                foreach (var profileCarrierAccount in profileCarrierAccounts)
                {
                    if (profileCarrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active)
                    {
                        if (profileCarrierAccount.AccountType == CarrierAccountType.Customer || profileCarrierAccount.AccountType == CarrierAccountType.Exchange)
                        {
                            customerActivationStatus = CarrierProfileActivationStatus.Active;
                        }
                        if (profileCarrierAccount.AccountType == CarrierAccountType.Supplier || profileCarrierAccount.AccountType == CarrierAccountType.Exchange)
                        {
                            supplierActivationStatus = CarrierProfileActivationStatus.Active;
                        }
                    }
                }
            }
        }

        #endregion

        #region Special Methods
        public void UpdateCarrierProfileExtendedSetting<T>(int carrierProfileId, T extendedSettings) where T : class
        {
            string extendedSettingName = typeof(T).FullName;

            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);

            Dictionary<string, Object> extendedSettingsDic = carrierProfile.ExtendedSettings as Dictionary<string, Object>;
            if (extendedSettingsDic == null)
                extendedSettingsDic = new Dictionary<string, Object>();

            Object exitingExtendedSettings;
            if (extendedSettingsDic.TryGetValue(extendedSettingName, out exitingExtendedSettings))
            {
                extendedSettingsDic[extendedSettingName] = extendedSettings;
            }
            else
            {
                extendedSettingsDic.Add(extendedSettingName, extendedSettings);
            }
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();

            if (dataManager.UpdateExtendedSettings(carrierProfileId, extendedSettingsDic))
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        public T GetExtendedSettings<T>(int carrierProfileId) where T : class
        {
            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);
            return carrierProfile != null ? GetExtendedSettings<T>(carrierProfile) : null;
        }
        public T GetExtendedSettings<T>(CarrierProfile carrierProfile) where T : class
        {
            string extendedSettingName = typeof(T).FullName;

            Dictionary<string, Object> extendedSettingsDic = carrierProfile.ExtendedSettings as Dictionary<string, Object>;

            Object exitingExtendedSettings;
            if (extendedSettingsDic != null)
            {
                extendedSettingsDic.TryGetValue(extendedSettingName, out exitingExtendedSettings);
                if (exitingExtendedSettings != null)
                    return exitingExtendedSettings as T;
                else
                    return default(T);
            }
            else
                return default(T);
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
            return GetCarrierProfileCurrencyId(carrierProfile);
        }

        public int GetCarrierProfileCurrencyId(CarrierProfile carrierProfile)
        {
            if (carrierProfile == null)
                throw new NullReferenceException("carrierProfile");
            if (carrierProfile.Settings == null)
                throw new NullReferenceException("carrierProfile.Settings");
            return carrierProfile.Settings.CurrencyId;
        }
        public int GetCustomerTimeZoneId(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            if (carrierProfile == null)
                throw new NullReferenceException("carrierProfile");
            if (carrierProfile.Settings == null)
                throw new NullReferenceException("carrierProfile.Settings");
            return carrierProfile.Settings.DefaultCusotmerTimeZoneId;
        }
        public int GetSupplierTimeZoneId(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            if (carrierProfile == null)
                throw new NullReferenceException("carrierProfile");
            if (carrierProfile.Settings == null)
                throw new NullReferenceException("carrierProfile.Settings");
            return carrierProfile.Settings.DefaultSupplierTimeZoneId;
        }
        #endregion

        #region Settings
        public CompanySetting GetCompanySetting(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            if (carrierProfile.Settings.CompanySettingId.HasValue)
            {
                return configManager.GetCompanySettingById(carrierProfile.Settings.CompanySettingId.Value);
            }
            return configManager.GetDefaultCompanySetting();
        }

        public Dictionary<Guid, InvoiceReportFile> GetCompanySettingInvoiceReportFiles(int carrierProfileId)
        {
            return GetCompanySetting(carrierProfileId).InvoiceReportFiles;
        }
        public IEnumerable<VRTaxItemDetail> GetTaxItemDetails(int carrierProfileId)
        {
            List<VRTaxItemDetail> taxItemDetails = new List<VRTaxItemDetail>();
            var taxesDefinitions = GetTaxesDefinition();
            if (taxesDefinitions != null)
            {
                var carrierProfile = GetCarrierProfile(carrierProfileId);
                if (carrierProfile.Settings.TaxSetting != null)
                {
                    if (carrierProfile.Settings.TaxSetting.VAT.HasValue)
                    {
                        taxItemDetails.Add(new VRTaxItemDetail
                        {
                            TaxName = "VAT",
                            Value = carrierProfile.Settings.TaxSetting.VAT.Value,
                            IsVAT=true
                        });
                    }
                   
                    foreach (var tax in carrierProfile.Settings.TaxSetting.Items)
                    {
                        var taxDefinition = taxesDefinitions.FirstOrDefault(x => x.ItemId == tax.ItemId);
                        if (taxDefinition != null)
                        {
                            taxItemDetails.Add(new VRTaxItemDetail
                            {
                                TaxName = taxDefinition.Title,
                                Value = tax.Value
                            });
                        }

                    }
                }
            }
            return taxItemDetails;
        }
        public IEnumerable<Guid> GetBankDetails(int carrierProfileId)
        {
            var carrierProfile = GetCarrierProfile(carrierProfileId);
            if (carrierProfile.Settings.BankDetailsIds != null && carrierProfile.Settings.BankDetailsIds.Count > 0)
            {
                return carrierProfile.Settings.BankDetailsIds;
            }else
            {
                Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
                var companySettings = GetCompanySetting(carrierProfileId);
                return companySettings.BankDetails;
            }
        }
        public List<VRTaxItemDefinition> GetTaxesDefinition()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();
            if (setting.TaxesDefinition == null)
                throw new NullReferenceException("setting.TaxesDefinition");
            return setting.TaxesDefinition.ItemDefinitions;
        }

        public List<CarrierProfileTicketContactInfo> GetCarrierProfileTicketContactsInfo(TicketContactInfoFilter filter)
        {
            List<CarrierProfileTicketContactInfo> ticketContactInfos = null;

            if (filter != null)
            {
                int? carrierProfileId = new CarrierAccountManager().GetCarrierProfileId(filter.CarrierAccountId);

                if (carrierProfileId.HasValue)
                {
                    CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId.Value);

                    if (carrierProfile.Settings != null && carrierProfile.Settings.TicketContacts != null)
                    {
                        ticketContactInfos = new List<CarrierProfileTicketContactInfo>();
                        for (int i = 0; i < carrierProfile.Settings.TicketContacts.Count; i++)
                        {
                            ticketContactInfos.Add(CarrierProfileTicketContactInfoMapper(carrierProfile.Settings.TicketContacts[i], i));

                        }
                    }
                }

            }
            
            return ticketContactInfos;
        }

        public List<SMSServiceType> GetCarrierProfileCustomerSMSServiceTypes(int carrierProfileId)
        {
            List<SMSServiceType> smsServiceTypes = new List<SMSServiceType>();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccount> carrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierProfileId, true, false);

            foreach (var crAccount in carrierAccounts)
            {
                List<SMSServiceType> crAccountSMSServiceTypes = carrierAccountManager.GetCustomerSMSServiceTypes(crAccount);
                if(crAccountSMSServiceTypes != null && crAccountSMSServiceTypes.Count> 0)
                {
                    foreach (var crAccountSMSServiceType in crAccountSMSServiceTypes)
                    {
                        if (!smsServiceTypes.Any(x => x.SMSServiceTypeId == crAccountSMSServiceType.SMSServiceTypeId))
                            smsServiceTypes.Add(crAccountSMSServiceType);
                    }
                }
                  
            }
            return smsServiceTypes;
        }

        public List<SMSServiceType> GetCarrierProfileSupplierSMSServiceTypes(int carrierProfileId)
        {
            List<SMSServiceType> smsServiceTypes = new List<SMSServiceType>();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccount> carrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierProfileId, false, true);

            foreach (var crAccount in carrierAccounts)
            {
                List<SMSServiceType> crAccountSMSServiceTypes = carrierAccountManager.GetSupplierSMSServiceTypes(crAccount);
                if (crAccountSMSServiceTypes != null && crAccountSMSServiceTypes.Count > 0)
                {
                    foreach (var crAccountSMSServiceType in crAccountSMSServiceTypes)
                    {
                        if(!smsServiceTypes.Any(x=>x.SMSServiceTypeId == crAccountSMSServiceType.SMSServiceTypeId))
                            smsServiceTypes.Add(crAccountSMSServiceType);
                    }
                }

            }
            return smsServiceTypes;
        }

        #endregion

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
        private Dictionary<int, CarrierProfilePortalAccountSettings> GetCachedCarrierProfilePortalAccountSettingsByUserId(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedCarrierProfilePortalAccountSettingsByUserId_{0}", userId),
             () =>
             {
                 ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
                 IEnumerable<CarrierProfile> carrierProfiles = dataManager.GetCarrierProfiles();
                 Dictionary<int, CarrierProfilePortalAccountSettings> carrierProfilePortalSettings = new Dictionary<int, CarrierProfilePortalAccountSettings>();
                 if (carrierProfiles != null && carrierProfiles.Count() > 0)
                 {
                     foreach (var cp in carrierProfiles)
                     {
                         if (cp.ExtendedSettings != null && cp.ExtendedSettings.Count > 0)
                         {
                             var extendedSettings = cp.ExtendedSettings.GetRecord("TOne.WhS.BusinessEntity.Entities.PortalAccountSettings");
                             if (extendedSettings != null)
                             {
                                 var portalAccountSettings = extendedSettings.CastWithValidate<PortalAccountSettings>("PortalAccountSettings");
                                 portalAccountSettings.ThrowIfNull("portalAccountSettings");
                                 portalAccountSettings.CarrierProfilePortalAccounts.ThrowIfNull("portalAccountSettings.CarrierProfilePortalAccounts");
                                 foreach (var cpPortalAccount in portalAccountSettings.CarrierProfilePortalAccounts)
                                 {
                                     if (cpPortalAccount.Type == PortalCarrierAccountType.All)
                                     {
                                         CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                                         var carrierAccounts = carrierAccountManager.GetCarriersByProfileId(cp.CarrierProfileId, true, true);
                                         carrierProfilePortalSettings.Add(cpPortalAccount.UserId, new CarrierProfilePortalAccountSettings
                                         {
                                             PortalCarrierAccounts = carrierAccounts.Select<CarrierAccount, PortalCarrierAccount>(x => new PortalCarrierAccount
                                             {
                                                 CarrierAccountId = x.CarrierAccountId
                                             }).ToList(),
                                             CarrierProfileId = cp.CarrierProfileId,
                                             PortalAccountSettings = portalAccountSettings
                                         });
                                     }
                                     else
                                     {
                                         carrierProfilePortalSettings.Add(cpPortalAccount.UserId, new CarrierProfilePortalAccountSettings
                                         {
                                             PortalCarrierAccounts = cpPortalAccount.CarrierAccounts,
                                             CarrierProfileId = cp.CarrierProfileId,
                                             PortalAccountSettings = portalAccountSettings
                                         });
                                     }
                                 }
                             }
                         }
                     }
                 }
                 return carrierProfilePortalSettings;
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
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Carrier Profiles",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Carrier Profile Name", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Company" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Country" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CarrierProfileId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Settings == null ? "" : record.Entity.Settings.Company });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CountryName });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        private BusinessEntityTechnicalSettingsData GetBusinessEntitySettingData()
        {
            SettingManager settingManager = new SettingManager();
            BusinessEntityTechnicalSettingsData setting = settingManager.GetSetting<BusinessEntityTechnicalSettingsData>(BusinessEntityTechnicalSettingsData.BusinessEntityTechnicalSettings);

            if (setting == null)
                throw new NullReferenceException("BusinessEntityTechnicalSettingsData");
            return setting;
        }

        public void SetFilesUsed(List<long> fileIds, int? carrierProfileId)
        {
            if (fileIds != null && fileIds.Count > 0)
            {
                VRFileManager fileManager = new VRFileManager();
                foreach (var fileId in fileIds)
                {
                    var fileSettings = new VRFileSettings { ExtendedSettings = new CarrierProfileFileSettings { CarrierProfileId = carrierProfileId } };
                    fileManager.SetFileUsedAndUpdateSettings(fileId, fileSettings);
                }
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

            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();

            var financialAccountsData = financialAccountManager.GetEffectiveFinancialAccountsDataByCarrierProfileId(carrierProfile.CarrierProfileId, DateTime.Now);
            if (financialAccountsData != null && financialAccountsData.Count() > 0)
            {
                var financialAccountData = financialAccountsData.FindRecord(x => x.FinancialAccount.CarrierProfileId.HasValue);
                if (financialAccountData != null)
                {
                    carrierProfileDetail.InvoiceSettingName = financialAccountManager.GetFinancialInvoiceSettingName(financialAccountData.FinancialAccount.FinancialAccountDefinitionId, financialAccountData.FinancialAccount.FinancialAccountId.ToString(), financialAccountData.InvoiceData.InvoiceTypeId);
                    carrierProfileDetail.InvoiceTypeDescription = "Profile";

                }
                else
                {
                    carrierProfileDetail.InvoiceTypeDescription = "Account";
                }
            }
            return carrierProfileDetail;
        }

        private CarrierProfileTicketContactInfo CarrierProfileTicketContactInfoMapper(CarrierProfileTicketContact carrierProfileTicketContact, int index)
        {
            return new CarrierProfileTicketContactInfo()
            {
                CarrierProfileTicketContactId = carrierProfileTicketContact.CarrierProfileTicketContactId,
                Name = string.Format("{0} - {1}", index + 1, carrierProfileTicketContact.Name),
                NameDescription = carrierProfileTicketContact.Name,
                PhoneNumber = carrierProfileTicketContact.PhoneNumber,
                Emails = carrierProfileTicketContact.Emails

            };
        }
        #endregion
        public class CarrierProfileLoggableEntity : VRLoggableEntityBase
        {
            public static CarrierProfileLoggableEntity Instance = new CarrierProfileLoggableEntity();

            private CarrierProfileLoggableEntity()
            {

            }

            static CarrierProfileManager s_carrierProfileManager = new CarrierProfileManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_CarrierProfile"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Carrier Profile"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_CarrierProfile_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                CarrierProfile carrierProfile = context.Object.CastWithValidate<CarrierProfile>("context.Object");
                return carrierProfile.CarrierProfileId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                CarrierProfile carrierProfile = context.Object.CastWithValidate<CarrierProfile>("context.Object");
                return s_carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
            }
        }
        #region IBusinessEntityManager

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCarrierProfile(context.EntityId);
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();
            if (allCarrierProfiles == null)
                return null;
            else
                return allCarrierProfiles.Values.Select(itm => itm as dynamic).ToList();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCarrierProfileName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var carrierProfile = context.Entity as CarrierProfile;
            return carrierProfile.CarrierProfileId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CarrierProfileFileSettings : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("871CFB3E-8957-44C2-A3F6-454F4465CDC7"); }
        }

        public int? CarrierProfileId { get; set; }

        Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            return s_securityManager.HasPermissionToActions("WhS_BE/CarrierProfile/GetFilteredCarrierProfiles", context.UserId);
        }
    }

    public class CarrierProfileFilterPersonalizationExtendedSetting : EntityPersonalizationExtendedSetting
    {
        public string Name { get; set; }

        public string Company { get; set; }

        public List<int> CountryIds { get; set; }
    }

    public class CarrierProfileGridPersonalizationExtendedSetting : EntityPersonalizationExtendedSetting
    {
        public GridPersonalizationExtendedSetting BaseGridPersonalization { get; set; }
    }
}
