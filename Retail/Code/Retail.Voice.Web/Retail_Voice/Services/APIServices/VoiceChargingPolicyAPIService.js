(function (appControllers) {

    "use strict";

    VoiceChargingPolicyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Voice_ModuleConfig', 'SecurityService'];

    function VoiceChargingPolicyAPIService(BaseAPIService, UtilsService, Retail_Voice_ModuleConfig, SecurityService) {

        var controllerName = "VoiceChargingPolicy";


        function GetVoiceChargingPolicyEvaluatorTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Voice_ModuleConfig.moduleName, controllerName, "GetVoiceChargingPolicyEvaluatorTemplateConfigs"));
        }


        return ({
            GetVoiceChargingPolicyEvaluatorTemplateConfigs: GetVoiceChargingPolicyEvaluatorTemplateConfigs
        });
    }

    appControllers.service('Retail_BE_VoiceChargingPolicyAPIService', VoiceChargingPolicyAPIService);

})(appControllers);