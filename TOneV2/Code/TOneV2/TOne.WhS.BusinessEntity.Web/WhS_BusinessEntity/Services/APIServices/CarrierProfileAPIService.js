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
        function GetAllCarrierProfiles() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetAllCarrierProfiles"));

        }
        return ({
            GetAllCarrierProfiles: GetAllCarrierProfiles,
            GetFilteredCarrierProfiles: GetFilteredCarrierProfiles,
            GetCarrierProfile: GetCarrierProfile
        });
    }

    appControllers.service('WhS_BE_CarrierProfileAPIService', carrierProfileAPIService);

})(appControllers);