(function (appControllers) {

    "use strict";
    carrierProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function carrierProfileAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredCarrierProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierProfile", "GetFilteredCarrierProfiles"), input);
        }

        function GetCarrierProfile(carrierProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfile"), {
                carrierProfileId: carrierProfileId
            });

        }
        function GetCarrierProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfilesInfo"));

        }
        function UpdateCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierProfile", "UpdateCarrierProfile"), carrierProfileObject);
        }
        function AddCarrierProfile(carrierProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierProfile", "AddCarrierProfile"), carrierProfileObject);
        }
        return ({
            GetCarrierProfilesInfo: GetCarrierProfilesInfo,
            GetFilteredCarrierProfiles: GetFilteredCarrierProfiles,
            GetCarrierProfile: GetCarrierProfile,
            AddCarrierProfile:AddCarrierProfile,
            UpdateCarrierProfile: UpdateCarrierProfile
        });
    }

    appControllers.service('Demo_CarrierProfileAPIService', carrierProfileAPIService);

})(appControllers);