'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetSalesZones: GetSalesZones,
        GetOwnZones: GetOwnZones
    });

    function GetSalesZones(nameFilter) {
        return BaseAPIService.get("/api/BusinessEntity/GetSalesZones",
            {
                nameFilter: nameFilter
            });
    }

    function GetOwnZones(nameFilter) {
        return BaseAPIService.get("/api/BusinessEntity/GetOwnZones",
            {
                nameFilter: nameFilter
            });
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('ZoneAPIService', serviceObj);