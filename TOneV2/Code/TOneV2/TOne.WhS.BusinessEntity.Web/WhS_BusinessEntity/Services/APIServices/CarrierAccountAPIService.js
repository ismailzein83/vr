
(function (appControllers) {

    "use strict";
    serviceObj.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function serviceObj(baseApiService, UtilsService, WhS_BE_ModuleConfig) {

        function GetCarrierAccounts(getCustomers,getSuppliers) {
            return baseApiService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccounts"), {
                getCustomers: getCustomers,
                getSuppliers: getSuppliers
            });

        }
        return ({
            GetCarrierAccounts: GetCarrierAccounts
        });
    }
    appControllers.service('WhS_BE_CarrierAccountAPIService', serviceObj);

})(appControllers);