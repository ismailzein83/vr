using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Business
{
    public class VRNotificationTypeManager
    {
        #region Ctor/Properties

        VRComponentTypeManager _vrComponentTypeManager = new VRComponentTypeManager();

        #endregion

        #region Public Methods

        public VRNotificationType GetNotificationType(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentType<VRNotificationTypeSettings, VRNotificationType>(notificationTypeId);
        }

        public VRNotificationTypeSettings GetNotificationTypeSettings(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<VRNotificationTypeSettings>(notificationTypeId);
        }

        public VRNotificationTypeExtendedSettings GetVRNotificationTypeExtendedSettings(Guid notificationTypeId)
        {
            VRNotificationTypeSettings vrNotificationTypeSettings = this.GetNotificationTypeSettings(notificationTypeId);
            if (vrNotificationTypeSettings == null)
                throw new NullReferenceException(string.Format("vrNotificationTypeSettings of vrNotificationTypeId: {0}", notificationTypeId));

            if (vrNotificationTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException(string.Format("vrNotificationTypeSettings.ExtendedSettings of vrNotificationTypeId: {0}", notificationTypeId));

            return vrNotificationTypeSettings.ExtendedSettings;
        }

        public T GetVRNotificationTypeExtendedSettings<T>(Guid notificationTypeId) where T : VRNotificationTypeExtendedSettings
        {
            T vrNotificationTypeExtendedSettings = this.GetVRNotificationTypeExtendedSettings(notificationTypeId) as T;
            if (vrNotificationTypeExtendedSettings == null)
                throw new NullReferenceException(string.Format("vrNotificationTypeExtendedSettings should be of type {0} and not of type {1}", typeof(T), vrNotificationTypeExtendedSettings.GetType()));

            return vrNotificationTypeExtendedSettings;
        }

        public IEnumerable<VRNotificationTypeConfig> GetVRNotificationTypeDefinitionConfigSettings()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRNotificationTypeConfig>(VRNotificationTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRNotificationTypeSettingsInfo> GetVRNotificationTypeSettingsInfo(VRNotificationTypeSettingsFilter filter)
        {
            IEnumerable<VRNotificationType> vrNotificationTypeSettings = GetCachedVRNotificationTypeSettingsInfo();

            Func<VRNotificationType, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (vrNotificationType) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(vrNotificationType, filter.Filters))
                        return false;
                    return true;
                };
            }
            return vrNotificationTypeSettings.MapRecords(VRNotificationTypeSettingInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Private Methods

        private bool CheckIfFilterIsMatch(VRNotificationType notificationType, List<IVRNotificationTypeFilter> filters)
        {
            VRNotificationTypeFilterContext context = new VRNotificationTypeFilterContext { VRNotificationType = notificationType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        private IEnumerable<VRNotificationType> GetCachedVRNotificationTypeSettingsInfo()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<VRNotificationTypeSettings, VRNotificationType>();
        }

        #endregion

        #region Mappers

        private VRNotificationTypeSettingsInfo VRNotificationTypeSettingInfoMapper(VRNotificationType vrNotificationType)
        {
            return new VRNotificationTypeSettingsInfo
            {
                Id = vrNotificationType.VRComponentTypeId,
                Name = vrNotificationType.Name,
                BodyDirective = vrNotificationType.Settings.ExtendedSettings.BodyRuntimeEditor,
                SearchDirective = vrNotificationType.Settings.ExtendedSettings.SearchRuntimeEditor
            };
        }

        #endregion
    }
}
