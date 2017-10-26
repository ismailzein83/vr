﻿(function (appControllers) {

    "use strict";
    companySettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function companySettingsAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'CompanySettings';

        function GetCompanySettingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCompanySettingsInfo"), {
                filter: filter
            });
        }

        function GetCompanyContactTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCompanyContactTypes"));
        }

        function GetCompanyDefinitionSettings() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCompanyDefinitionSettings"));
        }

        return ({
            GetCompanySettingsInfo: GetCompanySettingsInfo,
            GetCompanyContactTypes: GetCompanyContactTypes,
            GetCompanyDefinitionSettings: GetCompanyDefinitionSettings
        });
    }

    appControllers.service('VRCommon_CompanySettingsAPIService', companySettingsAPIService);

})(appControllers);