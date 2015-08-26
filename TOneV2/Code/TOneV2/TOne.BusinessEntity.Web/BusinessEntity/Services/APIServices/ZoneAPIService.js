"use strict";

var serviceObj = function (BaseAPIService) {
    return ({
        GetOwnZones: GetOwnZones,
        GetSupplierZones: GetSupplierZones,
        GetCustomerZones: GetCustomerZones
    });

    function GetOwnZones(nameFilter) {
        return BaseAPIService.get("/api/Zone/GetOwnZones",
            {
                nameFilter: nameFilter
            });
    }

    function GetSupplierZones(nameFilter, supplierId) {
        return BaseAPIService.get("/api/Zone/GetSupplierZones",
            {
                nameFilter: nameFilter,
                supplierId: supplierId
            });

    }
    function GetCustomerZones(nameFilter, customerId) {
        return BaseAPIService.get("/api/Zone/GetCustomerZones",
            {
                nameFilter: nameFilter,
                customerId: customerId
            });

    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('ZoneAPIService', serviceObj);