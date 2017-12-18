using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRExclusiveSessionManager
    {
        static IVRExclusiveSessionDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRExclusiveSessionDataManager>();
        static Vanrise.Security.Entities.IUserManager s_userManager = Vanrise.Security.Entities.BEManagerFactory.GetManager<Vanrise.Security.Entities.IUserManager>();

        #region Public Methods

        public VRExclusiveSessionTryTakeOutput TryTakeSession(VRExclusiveSessionTryTakeInput input)
        {
            s_dataManager.InsertIfNotExists(input.SessionTypeId, input.TargetId);
            string failureMessage;
            if (TryTakeSession(input.SessionTypeId, input.TargetId, out failureMessage))
            {
                return new VRExclusiveSessionTryTakeOutput
                {
                    IsSucceeded = true
                };
            }
            else
            {
                return new VRExclusiveSessionTryTakeOutput
                {
                    IsSucceeded = false,
                    FailureMessage = failureMessage
                };
            }
        }


        public IEnumerable<VRExclusiveSessionTypeInfo> GetVRExclusiveSessionTypeInfos(VRExclusiveSessionTypeInfoFilter filter)
        {
            Func<VRExclusiveSessionType, bool> filterExpression = null;
            if (filter != null)
            {
                
            }
            VRComponentTypeManager _vrComponentTypeManager = new VRComponentTypeManager();
            return _vrComponentTypeManager.GetComponentTypes<VRExclusiveSessionTypeSettings, VRExclusiveSessionType>().MapRecords(RExclusiveSessionTypeInfoMapper, filterExpression);
        }
        public VRExclusiveSessionTryKeepOutput TryKeepSession(VRExclusiveSessionTryKeepInput input)
        {
            string failureMessage;
            if (TryKeepSession(input.SessionTypeId, input.TargetId, out failureMessage))
            {
                return new VRExclusiveSessionTryKeepOutput
                {
                    IsSucceeded = true
                };
            }
            else
            {
                return new VRExclusiveSessionTryKeepOutput
                {
                    IsSucceeded = false,
                    FailureMessage = failureMessage
                };
            }
        }

        public void ReleaseSession(VRExclusiveSessionReleaseInput input)
        {
            if (input == null) return;
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            s_dataManager.ReleaseSession(input.SessionTypeId, input.TargetId, currentUserId);
        }

        #endregion

        #region Private Methods

        private bool TryTakeSession(Guid sessionTypeId, string targetId, out string failureMessage)
        {
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            int takenByUserId;
            s_dataManager.TryTakeSession(sessionTypeId, targetId, currentUserId, GetTimeOutInSeconds(), out takenByUserId);
            if (currentUserId == takenByUserId)
            {
                failureMessage = null;
                return true;
            }
            else
            {
                failureMessage = String.Format("Session is locked by '{0}'", s_userManager.GetUserName(takenByUserId));
                return false;
            }
        }

        private bool TryKeepSession(Guid sessionTypeId, string targetId, out string failureMessage)
        {
            int currentUserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            int takenByUserId;
            s_dataManager.TryKeepSession(sessionTypeId, targetId, currentUserId, GetTimeOutInSeconds(), out takenByUserId);
            if (currentUserId == takenByUserId)
            {
                failureMessage = null;
                return true;
            }
            else
            {
                failureMessage = String.Format("Session is locked by '{0}'", s_userManager.GetUserName(takenByUserId));
                return false;
            }
        }

        public bool DoesUserHaveTakeAccess(Guid sessionTypeId)
        {
            var extendedTypeSettings = this.GetVRExclusiveSessionTypeExtendedSettingsById(sessionTypeId);
            var context = new VRExclusiveSessionDoesUserHaveTakeAccessContext { UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId() };
            extendedTypeSettings.ThrowIfNull("DoesUserHaveTakeAccess");
            return extendedTypeSettings.DoesUserHaveTakeAccess(context);

        }

        private int GetTimeOutInSeconds()
        {
            var configManager = new ConfigManager();
            return configManager.GetSessionLockTimeOutInSeconds();
        }

        public IEnumerable<VRExclusiveSessionTypeExtendedSettingsConfig> GetVRExclusiveSessionTypeExtendedSettingsConfigs()
        {
            var configManager = new ExtensionConfigurationManager();
            return configManager.GetExtensionConfigurations<VRExclusiveSessionTypeExtendedSettingsConfig>(VRExclusiveSessionTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        private class VRExclusiveSessionDoesUserHaveTakeAccessContext : IVRExclusiveSessionDoesUserHaveTakeAccessContext
        {
            public int UserId
            {
                get;
                set;
            }
        }
        private VRExclusiveSessionTypeInfo RExclusiveSessionTypeInfoMapper(VRExclusiveSessionType vrSessionType)
        {
            return new VRExclusiveSessionTypeInfo
            {
                VRExclusiveSessionTypeId = vrSessionType.VRComponentTypeId,
                Name = vrSessionType.Name,
            };
        }

        #endregion

        public VRExclusiveSessionTypeExtendedSettings GetVRExclusiveSessionTypeExtendedSettingsById(Guid configId)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            var sessionTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<VRExclusiveSessionTypeSettings>(configId);
            sessionTypeSettings.ThrowIfNull("GetVRExclusiveSessionTypeExtendedSettingsById");
            return sessionTypeSettings.ExtendedSettings;
        }
    }
}
