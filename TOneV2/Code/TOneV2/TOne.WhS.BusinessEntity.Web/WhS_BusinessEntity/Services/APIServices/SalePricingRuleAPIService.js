(function (appControllers) {

    "use strict";
    salePricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function salePricingRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {


        function GetFilteredSalePricingRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "GetFilteredSalePricingRules"), input);
        }
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

        function AddRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "AddRule"), pricingRuleObj);
        }
        return ({
            AddRule: AddRule,
            GetFilteredSalePricingRules: GetFilteredSalePricingRules
        });
    }

    appControllers.service('WhS_BE_SalePricingRuleAPIService', salePricingRuleAPIService);

})(appControllers);