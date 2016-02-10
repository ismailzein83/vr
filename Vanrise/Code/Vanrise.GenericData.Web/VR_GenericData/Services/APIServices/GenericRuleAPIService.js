(function (appControllers) {

    'use strict';

    GenericRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRules: GetFilteredGenericRules,
            GetGenericRule: GetGenericRule,
            AddGenericRule: AddGenericRule,
            UpdateGenericRule: UpdateGenericRule
        };

        function GetFilteredGenericRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetFilteredGenericRules'), input);
        }

        function GetGenericRule(ruleDefinitionId, ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetGenericRule'), {
                ruleDefinitionId: ruleDefinitionId,
                ruleId: ruleId
            });
        }

        function AddGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'AddGenericRule'), genericRule);
        }

        function UpdateGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'UpdateGenericRule'), genericRule);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleAPIService', GenericRuleAPIService);

})(appControllers);