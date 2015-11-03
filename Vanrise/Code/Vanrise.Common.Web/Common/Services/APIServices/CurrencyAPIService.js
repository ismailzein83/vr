(function (appControllers) {

    "use strict";
    currencyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function currencyAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        function GetFilteredCurrencies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Currency", "GetFilteredCurrencies"), input);
        }

        function GetAllCurrencies(currencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Currency", "GetAllCurrencies"));
        }

        function GetCurrency(currencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Currency", "GetCurrency"),
                {
                    currencyId: currencyId
            });
        }

        function AddCurrency(currencyObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Currency", "AddCurrency"), currencyObject);
        }
        

        function UpdateCurrency(currencyObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Currency", "UpdateCurrency"), currencyObject);
        }

        return ({
            GetFilteredCurrencies: GetFilteredCurrencies,
            GetAllCurrencies: GetAllCurrencies,
            GetCurrency:GetCurrency,
            AddCurrency: AddCurrency,
            UpdateCurrency: UpdateCurrency

        });
    }

    appControllers.service('VRCommon_CurrencyAPIService', currencyAPIService);

})(appControllers);