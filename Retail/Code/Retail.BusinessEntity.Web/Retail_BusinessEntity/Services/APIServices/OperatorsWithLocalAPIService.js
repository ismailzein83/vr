(function (appControllers) {

    'use strict';

    Retail_BE_ICX_OperatorsWithLocalAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function Retail_BE_ICX_OperatorsWithLocalAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {
        var controllerName = 'OperatorsWithLocal';

        function GetOperatorsWithLocalInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetOperatorsWithLocalInfo"), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        return {
            GetOperatorsWithLocalInfo: GetOperatorsWithLocalInfo
        };
    }

    appControllers.service('Retail_BE_ICX_OperatorsWithLocalAPIService', Retail_BE_ICX_OperatorsWithLocalAPIService);

})(appControllers);