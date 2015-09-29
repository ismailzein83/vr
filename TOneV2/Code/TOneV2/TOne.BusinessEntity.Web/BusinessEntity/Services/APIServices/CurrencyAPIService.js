'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCurrencies: GetCurrencies,
        GetVisibleCurrencies: GetVisibleCurrencies
    });

    function GetCurrencies() {
        return BaseAPIService.get("/api/Currency/GetCurrencies");
    }
    function GetVisibleCurrencies() {
        return BaseAPIService.get("/api/Currency/GetVisibleCurrencies");
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CurrencyAPIService', serviceObj);