(function (appControllers) {

    "use strict";
    SMSServiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService','WhS_BE_ModuleConfig'];
    function SMSServiceTypeAPIService(BaseAPIService, UtilsService, SecurityService, WhS_BE_ModuleConfig) {

        var controllerName = "SMSServiceType";

        function GetSMSServicesTypesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSMSServicesTypesInfo"), {
                serializedFilter: serializedFilter,
            });
        }

        return ({
            GetSMSServicesTypesInfo: GetSMSServicesTypesInfo,
        });
    }

    appControllers.service('WhS_BE_SMSServiceTypeAPIService', SMSServiceTypeAPIService);
})(appControllers);