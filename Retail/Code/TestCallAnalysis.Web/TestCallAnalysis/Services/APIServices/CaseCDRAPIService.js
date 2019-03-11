(function (appControllers) {

    'use strict';

    CaseCDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'TestCallAnalysis_ModuleConfig'];

    function CaseCDRAPIService(BaseAPIService, UtilsService, TestCallAnalysis_ModuleConfig) {
        var controllerName = 'CaseCDR';

        return {
            GetCaseCDREntity: GetCaseCDREntity,
            UpdateCaseCDRStatus: UpdateCaseCDRStatus
        };

        function GetCaseCDREntity(businessEntityDefinitionId, genericBusinessEntityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(TestCallAnalysis_ModuleConfig.moduleName, controllerName, 'GetCaseCDREntity'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function UpdateCaseCDRStatus(caseCDRToUpdate) {
            return BaseAPIService.post(UtilsService.getServiceURL(TestCallAnalysis_ModuleConfig.moduleName, controllerName, 'UpdateCaseCDRStatus'), caseCDRToUpdate);
        }

    }

    appControllers.service('VR_TestCallAnalysis_CaseCDRAPIService', CaseCDRAPIService);

})(appControllers);