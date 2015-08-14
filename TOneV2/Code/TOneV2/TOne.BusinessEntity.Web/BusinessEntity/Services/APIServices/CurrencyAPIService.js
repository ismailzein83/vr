'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCurrencies: GetCurrencies
    });

    function GetCurrencies() {
        return BaseAPIService.get("/api/Currency/GetCurrencies");
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CurrencyAPIService', serviceObj);