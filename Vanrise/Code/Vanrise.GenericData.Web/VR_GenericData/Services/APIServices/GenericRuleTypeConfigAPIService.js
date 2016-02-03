(function (appControllers) {

    'use strict';

    GenericRuleTypeConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleTypeConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetGenericRuleTypes: GetGenericRuleTypes,
            GetGenericRuleType: GetGenericRuleType
        });
        function GetGenericRuleTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "GenericRuleTypeConfig", "GetGenericRuleTypes"));
        }

        function GetGenericRuleType() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "GenericRuleTypeConfig", "GetGenericRuleType"));
        }

    }

    appControllers.service('VR_GenericData_GenericRuleTypeConfigAPIService', GenericRuleTypeConfigAPIService);

})(appControllers);
