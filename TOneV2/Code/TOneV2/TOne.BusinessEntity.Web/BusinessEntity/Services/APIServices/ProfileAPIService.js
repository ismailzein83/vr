'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetAllProfiles: GetAllProfiles
    });

    function GetAllProfiles() {
        return BaseAPIService.get("/api/Profile/GetAllProfiles");
    }
}