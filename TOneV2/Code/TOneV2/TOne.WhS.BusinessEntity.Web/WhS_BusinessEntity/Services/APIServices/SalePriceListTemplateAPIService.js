(function (appControllers) {

	'use strict';

	SalePriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

	function SalePriceListTemplateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

		var controllerName = 'SalePriceListTemplate';

		function GetFilteredSalePriceListTemplates(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredSalePriceListTemplates'), input);
		}

		function GetSalePriceListTemplate(salePriceListTemplateId) {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSalePriceListTemplate'), {
				salePriceListTemplateId: salePriceListTemplateId
			});
		}

		function GetSalePriceListTemplatesInfo(filter) {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSalePriceListTemplatesInfo'), {
				filter: filter
			});
		}

		function GetSalePriceListTemplateSettingsExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSalePriceListTemplateSettingsExtensionConfigs'));
		}

		function GetMappedTablesExtensionConfigs() {
		    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetMappedTablesExtensionConfigs'));
		}

		function GetBasicSettingsMappedValueExtensionConfigs(priceListType) {
		    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetBasicSettingsMappedValueExtensionConfigs'), {
		        priceListType: priceListType
		    });
		}

		function AddSalePriceListTemplate(salePriceListTemplate) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'AddSalePriceListTemplate'), salePriceListTemplate);
		}

		function UpdateSalePriceListTemplate(salePriceListTemplate) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'UpdateSalePriceListTemplate'), salePriceListTemplate);
		}

		function HasAddSalePriceListTemplatePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddSalePriceListTemplate']));
		}

		function HasEditSalePriceListTemplatePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSalePriceListTemplate']));
		}

		return ({
			GetFilteredSalePriceListTemplates: GetFilteredSalePriceListTemplates,
			GetSalePriceListTemplate: GetSalePriceListTemplate,
			GetSalePriceListTemplatesInfo: GetSalePriceListTemplatesInfo,
			GetSalePriceListTemplateSettingsExtensionConfigs: GetSalePriceListTemplateSettingsExtensionConfigs,
			GetBasicSettingsMappedValueExtensionConfigs: GetBasicSettingsMappedValueExtensionConfigs,
			GetMappedTablesExtensionConfigs : GetMappedTablesExtensionConfigs,
			AddSalePriceListTemplate: AddSalePriceListTemplate,
			UpdateSalePriceListTemplate: UpdateSalePriceListTemplate,
			HasAddSalePriceListTemplatePermission: HasAddSalePriceListTemplatePermission,
			HasEditSalePriceListTemplatePermission: HasEditSalePriceListTemplatePermission
		});
	}

	appControllers.service('WhS_BE_SalePriceListTemplateAPIService', SalePriceListTemplateAPIService);

})(appControllers);