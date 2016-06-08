(function (appControllers) {

    "use strict";

    currencyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function currencyAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig)
    {
        var controllerName = 'Currency';

        function GetFilteredCurrencies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredCurrencies"), input);
        }

        function GetAllCurrencies(currencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetAllCurrencies"));
        }

        function GetCurrency(currencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCurrency"),
                {
                    currencyId: currencyId
            });
        }

        function GetSystemCurrency() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSystemCurrency"));
        }

        function GetSystemCurrencyId() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSystemCurrencyId"));
        }

        function AddCurrency(currencyObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddCurrency"), currencyObject);
        }

        function UpdateCurrency(currencyObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateCurrency"), currencyObject);
        }

        return ({
            GetFilteredCurrencies: GetFilteredCurrencies,
            GetAllCurrencies: GetAllCurrencies,
            GetCurrency: GetCurrency,
            GetSystemCurrency: GetSystemCurrency,
            GetSystemCurrencyId: GetSystemCurrencyId,
            AddCurrency: AddCurrency,
            UpdateCurrency: UpdateCurrency
        });
    }

    appControllers.service('VRCommon_CurrencyAPIService', currencyAPIService);

})(appControllers);