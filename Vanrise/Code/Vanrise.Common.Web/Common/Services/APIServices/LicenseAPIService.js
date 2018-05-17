(function (appControllers) {

    "use strict";
    licenseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function licenseAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controller = 'License';

        function GetLicenseExpiryDate(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetLicenseExpiryDate"));
        }
        return ({
            GetLicenseExpiryDate: GetLicenseExpiryDate,
        });
    }

    appControllers.service('VRCommon_LicenseAPIService', licenseAPIService);

})(appControllers);