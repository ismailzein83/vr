'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCustomerTOD: GetFilteredCustomerTOD,
        GetFilteredSupplierTOD: GetFilteredSupplierTOD
    });

    function GetFilteredCustomerTOD(input) {
        return BaseAPIService.post("/api/TOD/GetCutomerTODFromTempTable", input);
    }
    function GetFilteredSupplierTOD(input) {
        return BaseAPIService.post("/api/TOD/GetSupplierTODFromTempTable", input);
    }

   
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('TODAPIService', serviceObj);