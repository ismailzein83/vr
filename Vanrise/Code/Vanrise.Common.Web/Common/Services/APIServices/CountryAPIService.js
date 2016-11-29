(function (appControllers) {

    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function countryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'Country';

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredCountries"), input);
        }
        function GetCountriesInfo(filter) {
        	return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCountriesInfo"), {
        		filter: filter
        	});
        }
        function GetCountry(countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCountry"), {
                countryId: countryId
            });

        }
        function UpdateCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateCountry"), countryObject);
        }
        function AddCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddCountry"), countryObject);
        }
        function GetCountrySourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCountrySourceTemplates"));
        }

        function DownloadCountriesTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "DownloadCountriesTemplate"),
                {},
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                }
            );
        }

        function DownloadCountryLog(fileID) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "DownloadCountryLog"), { fileID: fileID }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function UploadCountries(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UploadCountries"), { fileId: fileId });
        }

        function HasAddCountryPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddCountry']));
        }

        function HasUploadCountryPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UploadCountries']));
        }

        function HasDownloadCountryPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['DownloadCountriesTemplate']));
        }

        function HasEditCountryPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateCountry']));
        }

        return ({
            GetFilteredCountries: GetFilteredCountries,
            GetCountriesInfo: GetCountriesInfo,
            GetCountry: GetCountry,
            UpdateCountry: UpdateCountry,
            AddCountry: AddCountry,
            GetCountrySourceTemplates: GetCountrySourceTemplates,
            DownloadCountriesTemplate: DownloadCountriesTemplate,
            DownloadCountryLog: DownloadCountryLog,
            UploadCountries: UploadCountries,
            HasAddCountryPermission: HasAddCountryPermission,
            HasUploadCountryPermission: HasUploadCountryPermission,
            HasDownloadCountryPermission: HasDownloadCountryPermission,
            HasEditCountryPermission: HasEditCountryPermission
        });
    }

    appControllers.service('VRCommon_CountryAPIService', countryAPIService);

})(appControllers);