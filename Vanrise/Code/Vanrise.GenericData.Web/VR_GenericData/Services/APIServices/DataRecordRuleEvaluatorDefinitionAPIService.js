(function (appControllers) {

    'use strict';

    DataRecordRuleEvaluatorDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataRecordRuleEvaluatorDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        var controllerName = "DataRecordRuleEvaluatorDefinition";

        function GetDataRecordRuleEvaluatorDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordRuleEvaluatorDefinitionsInfo"), { filter: filter });
        }
        
        return ({
            GetDataRecordRuleEvaluatorDefinitionsInfo: GetDataRecordRuleEvaluatorDefinitionsInfo
        });
    }

    appControllers.service('VR_GenericData_DataRecordRuleEvaluatorDefinitionAPIService', DataRecordRuleEvaluatorDefinitionAPIService);

})(appControllers);
