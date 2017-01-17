(function (appControllers) {

    "use strict";

    AccountIdentificationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Voice_ModuleConfig'];

    function AccountIdentificationAPIService(BaseAPIService, UtilsService, Retail_Voice_ModuleConfig) {

        var controllerName = "VoiceAccountIdentification";


        function GetAccountIdentificationTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Voice_ModuleConfig.moduleName, controllerName, "GetAccountIdentificationTemplates"));
        }


        return ({
            GetAccountIdentificationTemplates: GetAccountIdentificationTemplates
        });
    }

    appControllers.service('Retail_Voice_AccountIdentificationAPIService', AccountIdentificationAPIService);

})(appControllers);