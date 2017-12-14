using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class VRLocalizationLanguageManager
    {
        public IDataRetrievalResult<VRLocalizationLanguageDetail> GetFilteredVRLocalizationLanguages(DataRetrievalInput<VRLocalizationLanguageQuery> input)
        {
            var allVRLocalizationLanguages = GetCachedVRLocalizationLanguages();
            Func<VRLocalizationLanguage, bool> filterExpression = (x) =>
            {
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationLanguages.ToBigResult(input, filterExpression, VRLocalizationLanguageDetailMapper));
        }

        public VRLocalizationLanguage GetVRLocalizationLanguage(Guid vrLocalizationLanguageId)
        {
            var vrLocalizationLanguages = GetCachedVRLocalizationLanguages();
            if (vrLocalizationLanguages == null)
                return null;
            return vrLocalizationLanguages.GetRecord(vrLocalizationLanguageId);
        }
        public string GetVRLocalizationLanguageName(Guid vrLocalizationLanguageId)
        {
            var vrLocalizationLanguage = GetVRLocalizationLanguage(vrLocalizationLanguageId);
            if (vrLocalizationLanguage == null)
                return null;
            return vrLocalizationLanguage.Name;
        }
        public bool IsRTL(Guid languageId)
        {
            var vrLocalizationLanguage = GetVRLocalizationLanguage(languageId);
            if (vrLocalizationLanguage != null)
            {
                vrLocalizationLanguage.ThrowIfNull("vrLocalizationLanguage", languageId);
                vrLocalizationLanguage.Settings.ThrowIfNull("rLocalizationLanguage.Settings", languageId);
                return vrLocalizationLanguage.Settings.IsRTL;
            }
            return false;
        }
        public InsertOperationOutput<VRLocalizationLanguageDetail> AddVRLocalizationLanguage(VRLocalizationLanguage vrLocalizationLanguage)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationLanguageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRLocalizationLanguageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationLanguageDataManager>();

            vrLocalizationLanguage.VRLanguageId = Guid.NewGuid();

            if (dataManager.Insert(vrLocalizationLanguage))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRLocalizationLanguageDetailMapper(vrLocalizationLanguage);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<VRLocalizationLanguageDetail> UpdateVRLocalizationLanguage(VRLocalizationLanguage vrLocalizationLanguage)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationLanguageDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRLocalizationLanguageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationLanguageDataManager>();

            if (dataManager.Update(vrLocalizationLanguage))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRLocalizationLanguageDetailMapper(this.GetVRLocalizationLanguage(vrLocalizationLanguage.VRLanguageId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Dictionary<Guid, VRLocalizationLanguage>  GetAllLanguages()
        {
            return GetCachedVRLocalizationLanguages();
        }
      
        private Dictionary<Guid, VRLocalizationLanguage> GetCachedVRLocalizationLanguages()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRLocalizationLanguages",
               () =>
               {
                   IVRLocalizationLanguageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationLanguageDataManager>();
                   IEnumerable<VRLocalizationLanguage> vrLocalizationLanguages = dataManager.GetVRLocalizationLanguages();
                   return vrLocalizationLanguages.ToDictionary(itm => itm.VRLanguageId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRLocalizationLanguageDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationLanguageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRLocalizationLanguagesUpdated(ref _updateHandle);
            }
        }

        VRLocalizationLanguageDetail VRLocalizationLanguageDetailMapper(VRLocalizationLanguage localizationLanguage)
        {

            VRLocalizationLanguageDetail vrLocalizationLanguageDetail = new Entities.VRLocalizationLanguageDetail
            {
                VRLanguageId = localizationLanguage.VRLanguageId,
                Name = localizationLanguage.Name,
                ParentLanguageId = localizationLanguage.ParentLanguageId,
            };
            if (localizationLanguage.ParentLanguageId.HasValue)
            {
                vrLocalizationLanguageDetail.ParentLanguageName = GetVRLocalizationLanguageName(localizationLanguage.ParentLanguageId.Value);
            }
            return vrLocalizationLanguageDetail;
        }

        public IEnumerable<VRLocalizationLanguageInfo> GetVRLocalizationLanguagesInfo(VRLocalizationLanguageInfoFilter filter)
        {
            Func<VRLocalizationLanguage, bool> filterExpression = (x) =>
            {
                if(filter != null)
                {
                    if (filter.ExcludedIds != null && filter.ExcludedIds.Contains(x.VRLanguageId))
                        return false;
                }
                return true;
            };
            return this.GetCachedVRLocalizationLanguages().MapRecords(VRLocalizationLanguageInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public VRLocalizationLanguageInfo VRLocalizationLanguageInfoMapper(VRLocalizationLanguage vrLocalizationLanguage)
        {
            VRLocalizationLanguageInfo vrLocalizationLanguageInfo = new VRLocalizationLanguageInfo()
            {
                LocalizationLanguageId = vrLocalizationLanguage.VRLanguageId,
                Name = vrLocalizationLanguage.Name
            };
            return vrLocalizationLanguageInfo;
        }

        public IEnumerable<Guid> GetAllLanguagesIds()
        {
            var allLanguages = GetAllLanguages();
            if (allLanguages == null)
                return null;
            return GetAllLanguages().Values.MapRecords(x => x.VRLanguageId);
        }
    }
}
