'use strict'
var serviceObj = function (BaseAPIService) {
    alert("in")
    return ({
        GetFilteredSupplierTOD: GetFilteredSupplierTOD
    });
    function GetFilteredSupplierTOD(input) {
        return BaseAPIService.post("/api/SupplierTOD/GetSupplierTODFromTempTable", input);
    }

   
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SupplierTODAPIService', serviceObj);