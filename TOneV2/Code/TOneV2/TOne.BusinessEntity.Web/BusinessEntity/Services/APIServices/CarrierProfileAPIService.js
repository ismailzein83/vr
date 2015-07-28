'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCarrierProfiles: GetFilteredCarrierProfiles,
        GetCarrierProfile: GetCarrierProfile,
        UpdateCarrierProfile: UpdateCarrierProfile
    });
    function GetFilteredCarrierProfiles(input) {
        return BaseAPIService.post("/api/CarrierProfile/GetFilteredCarrierProfiles", input);
    }
    function GetCarrierProfile(profileId) {
        return BaseAPIService.get("/api/CarrierProfile/GetCarrierProfile",
            {
                profileId: profileId
            });
    }
    function UpdateCarrierProfile(CarrierProfile) {
        return BaseAPIService.post("/api/CarrierProfile/UpdateCarrierProfile", CarrierProfile);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierProfileAPIService', serviceObj);