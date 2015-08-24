'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCustomerTOD: GetFilteredCustomerTOD,
    });

    function GetFilteredCustomerTOD(input) {
        return BaseAPIService.post("/api/TOD/GetCutomerTODFromTempTable", input);
    }
   
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('TODAPIService', serviceObj);