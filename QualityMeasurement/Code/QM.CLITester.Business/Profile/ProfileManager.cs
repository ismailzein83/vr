using QM.CLITester.Data;
using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace QM.CLITester.Business
{
    public class ProfileManager
    {
        public Vanrise.Entities.IDataRetrievalResult<ProfileDetail> GetFilteredProfiles(Vanrise.Entities.DataRetrievalInput<ProfileQuery> input)
        {
            var allProfiles = GetCachedProfiles();

            Func<Profile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allProfiles.ToBigResult(input, filterExpression, ProfileDetailMapper));
        }

        public Profile GetProfile(int profileId)
        {
            var profiles = GetCachedProfiles();
            return profiles.GetRecord(profileId);
        }

        public Dictionary<string, long> GetExistingItemIds()
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedProfiles())
            {
                if (item.Value.SourceId != null)
                {
                    //if (sourceItemIds.Contains(item.Value.SourceId))
                        existingItemIds.Add(item.Value.SourceId, (long)item.Value.ProfileId);
                }
            }
            return existingItemIds;
        }

        public IEnumerable<ProfileInfo> GetProfilesInfo()
        {
            var profiles = GetCachedProfiles();
            return profiles.MapRecords(ProfileInfoMapper);
        }

        public Vanrise.Entities.UpdateOperationOutput<ProfileDetail> UpdateProfile(Profile profile)
        {
            if (profile.Settings != null && profile.Settings.ExtendedSettings != null)
            {
                foreach (var extendedSetting in profile.Settings.ExtendedSettings)
                {
                    extendedSetting.Apply(profile);
                }
            }

            IProfileDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<IProfileDataManager>();

            bool updateActionSucc = dataManager.Update(profile);
            Vanrise.Entities.UpdateOperationOutput<ProfileDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                ProfileDetail profileDetail = ProfileDetailMapper(profile);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = profileDetail;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public void InsertProfileSynchronize(Profile profile)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            profile.ProfileId = (int)startingId;
            IProfileDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<IProfileDataManager>();
            dataManager.InsertSynchronize(profile);
        }

        public void UpdateProfileSynchronize(Profile profile)
        {

            IProfileDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<IProfileDataManager>();
            dataManager.UpdateSynchronize(profile);
        }

        public IEnumerable<SourceProfileReaderConfig> GetProfileSourceTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SourceProfileReaderConfig>(SourceProfileReaderConfig.EXTENSION_TYPE);
        }

        #region Private Members

        public Dictionary<int, Profile> GetCachedProfiles()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProfiles",
               () =>
               {
                   IProfileDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<IProfileDataManager>();
                   IEnumerable<Profile> profiles = dataManager.GetProfiles();
                   return profiles.ToDictionary(cn => cn.ProfileId, cn => cn);
               });
        }

        private ProfileInfo ProfileInfoMapper(Profile profile)
        {
            return new ProfileInfo()
            {
                ProfileId = profile.ProfileId,
                Name = profile.Name,
            };
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProfileDataManager _dataManager = CliTesterDataManagerFactory.GetDataManager<IProfileDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreProfilesUpdated(ref _updateHandle);
            }
        }

        private ProfileDetail ProfileDetailMapper(Profile profile)
        {
            var profileDetail = new ProfileDetail
            {
                Entity = profile
            };
            return profileDetail;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(ProfileManager), nbOfIds, out startingId);
        }

        #endregion

    }
}
