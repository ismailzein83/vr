(function (appControllers) {

    'use strict';

    GenericRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            AddGenericRule: AddGenericRule,
            GetFilteredGenericRules: GetFilteredGenericRules
        };

        function GetFilteredGenericRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetFilteredGenericRules'), input);
        }

        function AddGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'AddGenericRule'), genericRule);
        }       
    }

    appControllers.service('VR_GenericData_GenericRuleAPIService', GenericRuleAPIService);

})(appControllers);