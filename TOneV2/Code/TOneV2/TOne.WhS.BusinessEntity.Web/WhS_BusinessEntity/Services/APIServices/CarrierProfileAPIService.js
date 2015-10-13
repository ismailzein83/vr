(function (appControllers) {

    "use strict";
    carrierProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function carrierProfileAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCarrierProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetFilteredCarrierProfiles"), input);
        }

        function GetCarrierProfile(carrierProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfile"), {
                carrierProfileId: carrierProfileId
            });

        }
        function GetCarrierProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfilesInfo"));

        }
        function UpdateCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "UpdateCarrierProfile"), carrierProfileObject);
        }
        function AddCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "AddCarrierProfile"), carrierProfileObject);
        }
        return ({
            GetCarrierProfilesInfo: GetCarrierProfilesInfo,
            GetFilteredCarrierProfiles: GetFilteredCarrierProfiles,
            GetCarrierProfile: GetCarrierProfile,
            AddCarrierProfile:AddCarrierProfile,
            UpdateCarrierProfile: UpdateCarrierProfile
        });
    }

    appControllers.service('WhS_BE_CarrierProfileAPIService', carrierProfileAPIService);

})(appControllers);