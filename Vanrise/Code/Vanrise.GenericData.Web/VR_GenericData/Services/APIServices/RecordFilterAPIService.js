(function (appControllers) {

    'use strict';

    RecordFilterAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function RecordFilterAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        var controllerName = 'RecordFilter';
        return {
            BuildRecordFilterGroupExpression: BuildRecordFilterGroupExpression,
        };

        function BuildRecordFilterGroupExpression(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'BuildRecordFilterGroupExpression'), input);
        }
    }
    appControllers.service('VR_GenericData_RecordFilterAPIService', RecordFilterAPIService);

})(appControllers);