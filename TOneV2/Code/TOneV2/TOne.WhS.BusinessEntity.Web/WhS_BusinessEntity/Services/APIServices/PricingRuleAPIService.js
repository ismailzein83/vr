(function (appControllers) {

    "use strict";
    pricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function pricingRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        //function GetFilteredCarrierProfiles(input) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetFilteredCarrierProfiles"), input);
        //}

        //function GetCarrierProfile(carrierProfileId) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfile"), {
        //        carrierProfileId: carrierProfileId
        //    });

        //}
        //function GetCarrierProfilesInfo() {
        //    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "GetCarrierProfilesInfo"));

        //}
        //function UpdateCarrierProfile(carrierProfileObject) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierProfile", "UpdateCarrierProfile"), carrierProfileObject);
        //}
        function AddPricingRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PricingRule", "AddPricingRule"), pricingRuleObj);
        }
        return ({
            AddPricingRule: AddPricingRule,
        });
    }

    appControllers.service('WhS_BE_PricingRuleAPIService', pricingRuleAPIService);

})(appControllers);