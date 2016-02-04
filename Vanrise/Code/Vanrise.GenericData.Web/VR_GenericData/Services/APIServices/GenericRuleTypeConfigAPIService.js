(function (appControllers) {

    'use strict';

    GenericRuleTypeConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleTypeConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetGenericRuleTypes: GetGenericRuleTypes,
            GetGenericRuleTypeByName: GetGenericRuleTypeByName
        });
        function GetGenericRuleTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "GenericRuleTypeConfig", "GetGenericRuleTypes"));
        }

        function GetGenericRuleTypeByName(ruleTypeName) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "GenericRuleTypeConfig", "GetGenericRuleTypeByName"), { ruleTypeName: ruleTypeName });
        }

    }

    appControllers.service('VR_GenericData_GenericRuleTypeConfigAPIService', GenericRuleTypeConfigAPIService);

})(appControllers);
