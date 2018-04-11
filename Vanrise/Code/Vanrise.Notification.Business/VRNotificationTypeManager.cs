using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using Vanrise.Entities;

namespace Vanrise.Notification.Business
{
	public class VRNotificationTypeManager : IVRNotificationTypeManager
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

		public List<VRAlertLevelWithStyleSettings> GetVRNotificationTypeLegendData(Guid notificationTypeId)
		{
			var componentTypeSettings = _vrComponentTypeManager.GetComponentTypeSettings<VRNotificationTypeSettings>(notificationTypeId);
			componentTypeSettings.ThrowIfNull("componentTypeSettings", notificationTypeId);

			var alertLevels = new VRAlertLevelManager().GetBusinessEntityDefinitionAlertLevels(componentTypeSettings.VRAlertLevelDefinitionId);
			if (alertLevels == null || alertLevels.Count() == 0)
				return null;

			alertLevels = alertLevels.OrderByDescending(item => item.Settings.Weight);
			
			List<VRAlertLevelWithStyleSettings> alertLevelsWithColor = new List<VRAlertLevelWithStyleSettings>();
			var styleDefinitionManager = new StyleDefinitionManager();

			foreach (var alertLevel in alertLevels)
			{
				alertLevel.Settings.ThrowIfNull("alertLevel.Settings", alertLevel.VRAlertLevelId);
				var styleDefinition = styleDefinitionManager.GetStyleDefinition(alertLevel.Settings.StyleDefinitionId);

				StyleFormatingSettings alertLevelStyle = null;
				if (styleDefinition != null && styleDefinition.StyleDefinitionSettings != null)
					alertLevelStyle = styleDefinition.StyleDefinitionSettings.StyleFormatingSettings;

				alertLevelsWithColor.Add(new VRAlertLevelWithStyleSettings() { Entity = alertLevel, AlertLevelStyle = alertLevelStyle });
			}
			return alertLevelsWithColor;
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
		private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
		{
			int userId = ContextFactory.GetContext().GetLoggedInUserId();
			return DoesUserHaveAccess(userId, requiredPermission);

		}
		private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
		{
			SecurityManager secManager = new SecurityManager();
			if (!secManager.IsAllowed(requiredPermission, userId))
				return false;
			return true;

		}

		#endregion

		#region security

		public bool DoesUserHaveViewAccess(Guid vrNotificationTypeId)
		{
			int userId = SecurityContext.Current.GetLoggedInUserId();
			var genericRuleDefinition = GetNotificationTypeSettings(vrNotificationTypeId);
			return DoesUserHaveViewAccess(userId, genericRuleDefinition);
		}
		public bool DoesUserHaveViewAccess(VRNotificationTypeSettings vrNotificationTypeSettings)
		{
			int userId = SecurityContext.Current.GetLoggedInUserId();
			return DoesUserHaveViewAccess(userId, vrNotificationTypeSettings);
		}
		public bool DoesUserHaveViewAccess(int userId, List<Guid> vrNotificationTypeIds)
		{
			foreach (var guid in vrNotificationTypeIds)
			{
				var genericRuleDefinition = GetNotificationTypeSettings(guid);
				if (DoesUserHaveViewAccess(userId, genericRuleDefinition))
					return true;
			}
			return false;
		}
		public bool DoesUserHaveViewAccess(int userId, VRNotificationTypeSettings vrNotificationTypeSettings)
		{
			if (vrNotificationTypeSettings.Security != null && vrNotificationTypeSettings.Security.ViewRequiredPermission != null)
				return DoesUserHaveAccess(userId, vrNotificationTypeSettings.Security.ViewRequiredPermission);
			else
				return true;
		}
		#endregion
	}
}
