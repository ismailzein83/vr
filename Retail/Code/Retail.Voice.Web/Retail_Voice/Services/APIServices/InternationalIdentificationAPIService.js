(function (appControllers) {

    "use strict";

    InternationalIdentificationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Voice_ModuleConfig'];

    function InternationalIdentificationAPIService(BaseAPIService, UtilsService, Retail_Voice_ModuleConfig) {

        var controllerName = "VoiceInternationalIdentification";


        function GetInternationalIdentificationTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Voice_ModuleConfig.moduleName, controllerName, "GetInternationalIdentificationTemplates"));
        }


        return ({
            GetInternationalIdentificationTemplates: GetInternationalIdentificationTemplates
        });
    }

    appControllers.service('Retail_Voice_InternationalIdentificationAPIService', InternationalIdentificationAPIService);

})(appControllers);