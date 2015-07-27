'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredProfiles: GetFilteredProfiles,
        GetCarrierProfile: GetCarrierProfile,
        UpdateCarrierProfile: UpdateCarrierProfile
    });
    function GetFilteredProfiles(resultKey, name, companyName, billingEmail, from, to) {
        return BaseAPIService.get("/api/Profile/GetFilteredProfiles",
            {
                resultKey: resultKey,
                name: name,
                companyName: companyName,
                billingEmail: billingEmail,
                from: from,
                to: to
            });
    }
    function GetCarrierProfile(profileId) {
        return BaseAPIService.get("/api/Profile/GetCarrierProfile",
            {
                profileId: profileId
            });
    }
    function UpdateCarrierProfile(CarrierProfile) {
        return BaseAPIService.post("/api/Profile/UpdateCarrierProfile", CarrierProfile);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierProfileAPIService', serviceObj);