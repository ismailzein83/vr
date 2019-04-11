(function (appControllers) {

    'use strict';

    OperatorDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_RA_ModuleConfig'];

    function OperatorDefinitionAPIService(BaseAPIService, UtilsService, Retail_RA_ModuleConfig) {
        var controllerName = 'RAOperatorDeclarationController';

        function GetOperatorDefinitionInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, 'GetOperatorDefinitionInfo'));
        }
        return {
            GetOperatorDefinitionInfo: GetOperatorDefinitionInfo
        };
    }

    appControllers.service('Retail_RA_OperatorDefinitionAPIService', OperatorDefinitionAPIService);

})(appControllers);