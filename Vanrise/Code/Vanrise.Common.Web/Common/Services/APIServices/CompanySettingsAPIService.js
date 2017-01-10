(function (appControllers) {

    "use strict";
    companySettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function companySettingsAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'CompanySettings';

        function GetCompanySettingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCompanySettingsInfo"), {
                filter: filter
            });
        }

        return ({
            GetCompanySettingsInfo: GetCompanySettingsInfo
        });
    }

    appControllers.service('VRCommon_CompanySettingsAPIService', companySettingsAPIService);

})(appControllers);