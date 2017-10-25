(function (appControllers) {

    "use strict";
    companySettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function companySettingsAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'CompanyDefinitions';

        function GetCompanyDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCompanyDefinitionConfigs"));
        }
        return ({
            GetCompanyDefinitionConfigs: GetCompanyDefinitionConfigs,
        });
    }

    appControllers.service('VRCommon_CompanyDefinitionsAPIService', companySettingsAPIService);

})(appControllers);