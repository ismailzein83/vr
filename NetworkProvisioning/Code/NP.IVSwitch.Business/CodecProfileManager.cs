using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
 
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

        public CodecProfileEditorRuntime GetCodecProfileHistoryDetailbyHistoryId(int codecProfileHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var codecProfileEditorRuntime = s_vrObjectTrackingManager.GetObjectDetailById(codecProfileHistoryId);
            return codecProfileEditorRuntime.CastWithValidate<CodecProfileEditorRuntime>("codecProfileEditorRuntime : historyId ", codecProfileHistoryId);
        }

        public CodecProfile GetCodecProfile(int codecProfileId, bool isViewedFromUI)
        {
            Dictionary<int, CodecProfile> cachedCodecProfile = this.GetCachedCodecProfile();
            var codecProfile = cachedCodecProfile.GetRecord(codecProfileId);
            if (codecProfile != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(CodecProfileLoggableEntity.Instance, codecProfile);
            return codecProfile;
        }

        public CodecProfile GetCodecProfile(int codecProfileId)
        {
            return GetCodecProfile(codecProfileId, false);
        }
        public string GetProfileName(int Id)
        {
            var codecProfile = this.GetCodecProfile(Id);
            if (codecProfile != null)
                return codecProfile.ProfileName;
            else
                return null;
        }
        public IDataRetrievalResult<CodecProfileDetail> GetFilteredCodecProfiles(DataRetrievalInput<CodecProfileQuery> input)
        {
            var allCodecProfiles = this.GetCachedCodecProfile();
            Func<CodecProfile, bool> filterExpression = (x) => (input.Query.Name == null || x.ProfileName.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(CodecProfileLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodecProfiles.ToBigResult(input, filterExpression, CodecProfileDetailMapper));
        }
        public CodecProfileEditorRuntime GetCodecProfileEditorRuntime(int codecProfileId, bool isViewedFromUI)
        {
            CodecProfileEditorRuntime codecProfileEditorRuntime = new CodecProfileEditorRuntime();
            codecProfileEditorRuntime.Entity = GetCodecProfile(codecProfileId);

            if (codecProfileEditorRuntime.Entity == null)
                throw new NullReferenceException(string.Format("codecProfileEditorRuntime.Entity for Profile Codec ID: {0} is null", codecProfileId));

            CodecDefManager codecDefManager = new CodecDefManager();
            codecProfileEditorRuntime.CodecDefList = codecDefManager.GetCodecDefList(codecProfileEditorRuntime.Entity.CodecDefId);
            if (codecProfileEditorRuntime != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(CodecProfileLoggableEntity.Instance, codecProfileEditorRuntime);
            return codecProfileEditorRuntime;
        }
        public CodecProfileEditorRuntime GetCodecProfileEditorRuntime(int codecProfileId)
        {

            return GetCodecProfileEditorRuntime(codecProfileId,false);
        }

        public Vanrise.Entities.InsertOperationOutput<CodecProfileDetail> AddCodecProfile(CodecProfile codecProfileItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CodecProfileDetail>
            {
                Result = Vanrise.Entities.InsertOperationResult.Failed,
                InsertedObject = null
            };


            ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();
            Helper.SetSwitchConfig(dataManager);

            int codecProfileId = -1;

            CodecDefManager codecDefManager = new CodecDefManager();

            if (dataManager.Insert(codecProfileItem, codecDefManager.GetAll(), out  codecProfileId))
            {
                codecProfileItem.CodecProfileId = codecProfileId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(CodecProfileLoggableEntity.Instance, GetCodecProfileEditorRuntime(codecProfileId));
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
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CodecProfileDetail>
            {
                Result = Vanrise.Entities.UpdateOperationResult.Failed,
                UpdatedObject = null
            };


            ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();
            Helper.SetSwitchConfig(dataManager);
            CodecDefManager codecDefManager = new CodecDefManager();

            if (dataManager.Update(codecProfileItem,  codecDefManager.GetAll()))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(CodecProfileLoggableEntity.Instance, GetCodecProfileEditorRuntime(codecProfileItem.CodecProfileId));
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

        private class CodecProfileLoggableEntity : VRLoggableEntityBase
        {
            public static CodecProfileLoggableEntity Instance = new CodecProfileLoggableEntity();

            private CodecProfileLoggableEntity()
            {

            }

            static CodecProfileManager codecProfileManager = new CodecProfileManager();

            public override string EntityUniqueName
            {
                get { return "NP_IVSwitch_CodecProfile"; }
            }

            public override string ModuleName
            {
                get { return "IVSwitch"; }
            }

            public override string EntityDisplayName
            {
                get { return "Codec Profile"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "NP_IVSwitch_CodecProfile_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                CodecProfileEditorRuntime codecProfileEditorRuntime = context.Object.CastWithValidate<CodecProfileEditorRuntime>("context.Object");
                return codecProfileEditorRuntime.Entity.CodecProfileId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                CodecProfileEditorRuntime codecProfileEditorRuntime = context.Object.CastWithValidate<CodecProfileEditorRuntime>("context.Object");
                return codecProfileManager.GetProfileName(codecProfileEditorRuntime.Entity.CodecProfileId);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, CodecProfile> GetCachedCodecProfile()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodecProfile",
                () =>
                {
                    ICodecProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICodecProfileDataManager>();
                    Helper.SetSwitchConfig(dataManager);
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



