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

        function AddBank(bankObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddBank"), bankObject);
        }
        
        return ({
            GetBankDetailsInfo: GetBankDetailsInfo,
            AddBank: AddBank
        });
    }

    appControllers.service('VRCommon_BankDetailAPIService', bankDetailAPIService);

})(appControllers);