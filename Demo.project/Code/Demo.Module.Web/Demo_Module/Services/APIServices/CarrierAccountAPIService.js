(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredCarrierAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetFilteredCarrierAccounts"), input);
        }
        
        function GetCarrierAccount(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccount"), {
                carrierAccountId: carrierAccountId
            });

        }
       
        function AddCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "AddCarrierAccount"), carrierAccountObject);
        }
        function UpdateCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "UpdateCarrierAccount"), carrierAccountObject);
        }

        return ({
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount,
            AddCarrierAccount: AddCarrierAccount,
            UpdateCarrierAccount: UpdateCarrierAccount,
        });
    }

    appControllers.service('Demo_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);