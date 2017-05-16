(function (appControllers) {

    'use strict';

    CustomerCountryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function CustomerCountryAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = 'CustomerCountry';

        function AreEffectiveOrFutureCountriesSoldToCustomer(customerId, date) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'AreEffectiveOrFutureCountriesSoldToCustomer'), {
                customerId: customerId,
                date: date
            });
        }

        return {
            AreEffectiveOrFutureCountriesSoldToCustomer: AreEffectiveOrFutureCountriesSoldToCustomer
        };
    }

    appControllers.service('WhS_BE_CustomerCountryAPIService', CustomerCountryAPIService);

})(appControllers);