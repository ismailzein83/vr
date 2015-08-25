"use strict";

var serviceObj = function (BaseAPIService) {
    return ({
        GetOwnZones: GetOwnZones,
        GetZones: GetZones
    });

    function GetOwnZones(nameFilter) {
        return BaseAPIService.get("/api/Zone/GetOwnZones",
            {
                nameFilter: nameFilter
            });
    }

    function GetZones(nameFilter, supplierId) {
        return BaseAPIService.get("/api/Zone/GetZones",
            {
                nameFilter: nameFilter,
                supplierId: supplierId
            });
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('ZoneAPIService', serviceObj);