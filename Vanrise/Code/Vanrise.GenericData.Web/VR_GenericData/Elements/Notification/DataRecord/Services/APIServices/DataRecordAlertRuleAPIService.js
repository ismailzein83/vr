(function (appControllers) {

    'use strict';

    DataRecordAlertRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordAlertRuleAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = 'DataRecordAlertRule';

        function GetDataRecordAlertRuleConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordAlertRuleConfigs"));
        }

        return {
            GetDataRecordAlertRuleConfigs: GetDataRecordAlertRuleConfigs
        };
    }

    appControllers.service('VR_GenericData_DataRecordAlertRuleAPIService', DataRecordAlertRuleAPIService);

})(appControllers);