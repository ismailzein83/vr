(function (appControllers) {

    "use strict";

    CompositeRecordConditionDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function CompositeRecordConditionDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = "CompositeRecordConditionDefinition";

        function GetCompositeRecordConditionDefinitionSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetCompositeRecordConditionDefinitionSettingConfigs"));
        }

        return ({
            GetCompositeRecordConditionDefinitionSettingConfigs: GetCompositeRecordConditionDefinitionSettingConfigs
        });
    }

    appControllers.service('VR_GenericData_CompositeRecordConditionDefinitionAPIService', CompositeRecordConditionDefinitionAPIService);
})(appControllers);