(function (appControllers) {

    "use strict";

    currencyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function currencyAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService)
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
        function HasAddCurrencyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddCurrency']));
        }
               
        function HasEditCurrencyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateCurrency']));
        }
        return ({
            GetFilteredCurrencies: GetFilteredCurrencies,
            GetAllCurrencies: GetAllCurrencies,
            GetCurrency: GetCurrency,
            GetSystemCurrency: GetSystemCurrency,
            GetSystemCurrencyId: GetSystemCurrencyId,
            AddCurrency: AddCurrency,
            UpdateCurrency: UpdateCurrency,
            HasAddCurrencyPermission: HasAddCurrencyPermission,
            HasEditCurrencyPermission: HasEditCurrencyPermission
        });
    }

    appControllers.service('VRCommon_CurrencyAPIService', currencyAPIService);

})(appControllers);