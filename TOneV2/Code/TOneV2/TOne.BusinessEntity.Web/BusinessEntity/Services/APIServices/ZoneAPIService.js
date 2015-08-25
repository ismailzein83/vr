"use strict";

var serviceObj = function (BaseAPIService) {
    return ({
        GetOwnZones: GetOwnZones
    });

    function GetOwnZones(nameFilter) {
        return BaseAPIService.get("/api/Zone/GetOwnZones",
            {
                nameFilter: nameFilter
            });
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('ZoneAPIService', serviceObj);