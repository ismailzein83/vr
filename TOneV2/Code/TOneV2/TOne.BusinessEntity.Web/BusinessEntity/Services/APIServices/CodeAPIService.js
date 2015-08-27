'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCodes: GetCodes,
    });

    function GetCodes(zoneID, effectiveOn) {
        return BaseAPIService.get("/api/Code/GetCodes", {
            zoneID: zoneID,
            effectiveOn: effectiveOn
        });
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CodeAPIService', serviceObj);