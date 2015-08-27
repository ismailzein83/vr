'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCustomerTOD: GetFilteredCustomerTOD
    });

    function GetFilteredCustomerTOD(input) {
        return BaseAPIService.post("/api/CustomerTOD/GetCutomerTODFromTempTable", input);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CustomerTODAPIService', serviceObj);