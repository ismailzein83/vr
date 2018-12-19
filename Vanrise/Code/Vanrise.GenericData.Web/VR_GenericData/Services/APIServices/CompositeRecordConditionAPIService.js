(function (appControllers) {

    "use strict";

    CompositeRecordConditionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function CompositeRecordConditionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = "CompositeRecordCondition";

        function GetCompositeRecordConditionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetCompositeRecordConditionConfigs"));
        }

        return ({
            GetCompositeRecordConditionConfigs: GetCompositeRecordConditionConfigs
        });
    }

    appControllers.service('VR_GenericData_CompositeRecordConditionAPIService', CompositeRecordConditionAPIService);
})(appControllers);



