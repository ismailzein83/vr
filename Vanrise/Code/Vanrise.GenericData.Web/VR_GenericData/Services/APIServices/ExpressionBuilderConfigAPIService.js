(function (appControllers) {

    'use strict';

    ExpressionBuilderConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function ExpressionBuilderConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetExpressionBuilderTemplates: GetExpressionBuilderTemplates,
        });
        function GetExpressionBuilderTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "ExpressionBuilderConfig", "GetExpressionBuilderTemplates"));
        }

    }

    appControllers.service('VR_GenericData_ExpressionBuilderConfigAPIService', ExpressionBuilderConfigAPIService);

})(appControllers);
