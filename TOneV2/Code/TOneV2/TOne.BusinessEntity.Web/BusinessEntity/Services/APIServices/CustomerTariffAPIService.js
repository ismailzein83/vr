'use strict'

var serviceObject = function (BaseAPIService) {
    return ({
        GetFilteredCustomerTariffs: GetFilteredCustomerTariffs
    });

    function GetFilteredCustomerTariffs(input) {
        return BaseAPIService.post('api/CustomerTariff/GetFilteredCustomerTariffs', input);
    }
};

serviceObject.$inject = ['BaseAPIService'];
appControllers.service('CustomerTariffAPIService', serviceObject);
