using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
 
namespace NP.IVSwitch.Business
{
    public class CodecProfileManager
    {
        #region Public Methods

        public IEnumerable<CodecProfileInfo> GetCodecProfilesInfo(CodecProfileFilter filter)
        {
            Func<CodecProfile, bool> filterExpression = null;

            return this.GetCachedCodecProfile().MapRecords(CodecProfileInfoMapper, filterExpression).OrderBy(x => x.ProfileName);
        }
        public CodecProfile GetCodecProfile(int codecProfileId)
        {
            Dictionary<int, CodecProfile> cachedCodecProfile = this.GetCachedCodecProfile();
            return cachedCodecProfile.GetRecord(codecProfileId);
        }

        public IDataRetrievalResult<CodecProfileDetail> GetFilteredCodecProfiles(DataRetrievalInput<CodecProfileQuery> input)
        {
            var allCodecProfiles = this.GetCachedCodecProfile();
            Func<CodecProfile, bool> filterExpression = (x) => (input.Query.Name == null || x.ProfileName.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodecProfiles.ToBigResult(input, filterExpression, CodecProfileDetailMapper));
        }

        public CodecProfileEditorRuntime GetCodecProfileEditorRuntime(int codecProfileId)
        {
            CodecProfileEditorRuntime codecProfileEditorRuntime = new CodecProfileEditorRuntime();
            codecProfileEditorRuntime.Entity =  GetCodecProfile(codecProfileId);

            if (codecProfileEditorRuntime.Entity == null)
                throw new NullReferenceException(string.Format("codecProfileEditorRuntime.Entity for Profile Codec ID: {0} is null", codecProfileId));

            CodecDefManager codecDefManager = new CodecDefManager();
            codecProfileEditorRuntime.CodecDefList = codecDefManager.GetCodecDefList(codecProfileEditorRuntime.Entity.CodecDefId);

            return codecProfileEditorRuntime;
        }

        public Vanrise.Entities.InsertOperationOutput<CodecProfileDetail> AddCodecProfile(CodecProfile codecProfileItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CodecProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();

            int codecProfileId = -1;

            CodecDefManager codecDefManager = new CodecDefManager();

            if (dataManager.Insert(codecProfileItem, codecDefManager.GetAll(), out  codecProfileId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CodecProfileDetailMapper(this.GetCodecProfile(codecProfileId));
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<CodecProfileDetail> UpdateCodecProfile(CodecProfile codecProfileItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CodecProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();

            CodecDefManager codecDefManager = new CodecDefManager();

            if (dataManager.Update(codecProfileItem,  codecDefManager.GetAll()))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CodecProfileDetailMapper(this.GetCodecProfile(codecProfileItem.CodecProfileId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }   

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICodecProfileDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, CodecProfile> GetCachedCodecProfile()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodecProfile",
                () =>
                {
                    ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();
                    return dataManager.GetCodecProfiles().ToDictionary(x => x.CodecProfileId, x => x);
                });
        }

        #endregion

        #region Mappers

        public CodecProfileDetail CodecProfileDetailMapper(CodecProfile codecProfile)
        {
            CodecProfileDetail codecProfileDetail = new CodecProfileDetail()
            {
                Entity = codecProfile
            };

            return codecProfileDetail;
        }

        public CodecProfileInfo CodecProfileInfoMapper(CodecProfile codecProfile)
        {
            CodecProfileInfo codecProfileInfo = new CodecProfileInfo()
            {
                CodecProfileId = codecProfile.CodecProfileId,
                ProfileName = codecProfile.ProfileName,           

            };
            return codecProfileInfo;
        }

        #endregion





    }
}



