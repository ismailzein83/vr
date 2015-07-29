'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCountries: GetCountries,
        GetCities: GetCities
    });

    function GetCountries() {
        return BaseAPIService.get("/api/LookUp/GetCountries");
    }

    function GetCities() {
        return BaseAPIService.get("/api/LookUp/GetCities");
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('LookUpAPIService', serviceObj);