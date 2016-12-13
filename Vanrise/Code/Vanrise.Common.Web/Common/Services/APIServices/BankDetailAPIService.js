(function (appControllers) {

    "use strict";
    bankDetailAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function bankDetailAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'BankDetail';

        function GetBankDetailsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetBankDetailsInfo"), {
                filter: filter
            });
        }

        return ({
            GetBankDetailsInfo: GetBankDetailsInfo
        });
    }

    appControllers.service('VRCommon_BankDetailAPIService', bankDetailAPIService);

})(appControllers);