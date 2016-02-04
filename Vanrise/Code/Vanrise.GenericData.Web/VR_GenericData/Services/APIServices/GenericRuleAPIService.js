(function (appControllers) {

    'use strict';

    GenericRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            AddGenericRule: AddGenericRule
        };

        function AddGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'AddGenericRule'), genericRule);
        }

        //function UpdateGenericRuleDefinition(genericRuleDefinition) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'UpdateGenericRuleDefinition'), genericRuleDefinition);
        //}

        //function GetGenericRuleDefinitionsInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetGenericRuleDefinitionsInfo'), {
        //        filter: filter
        //    });
        //}
    }

    appControllers.service('VR_GenericData_GenericRuleAPIService', GenericRuleAPIService);

})(appControllers);