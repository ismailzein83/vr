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
        VRComponentTypeManager _vrComponentTypeManager = new VRComponentTypeManager();

        public VRNotificationType GetNotificationType(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentType<VRNotificationTypeSettings, VRNotificationType>(notificationTypeId);
        }
        public VRNotificationTypeSettings GetNotificationTypeSettings(Guid notificationTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<VRNotificationTypeSettings>(notificationTypeId);
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
        bool CheckIfFilterIsMatch(VRNotificationType notificationType, List<IVRNotificationViewFilter> filters)
        {
            VRNotificationViewFilterContext context = new VRNotificationViewFilterContext { VRNotificationTypeId = notificationType.VRComponentTypeId };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }
        IEnumerable<VRNotificationType> GetCachedVRNotificationTypeSettingsInfo()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<VRNotificationTypeSettings, VRNotificationType>();
        }
        VRNotificationTypeSettingsInfo VRNotificationTypeSettingInfoMapper(VRNotificationType vrNotificationType)
        {
            return new VRNotificationTypeSettingsInfo
            {
                Id = vrNotificationType.VRComponentTypeId,
                Name = vrNotificationType.Name,
                BodyDirective = vrNotificationType.Settings.ExtendedSettings.BodyRuntimeEditor,
                SearchDirective = vrNotificationType.Settings.ExtendedSettings.SearchRuntimeEditor
            };
        }
    }
}
