(function (appControllers) {
    'use strict';

    BusinessObjectDataRecordStorageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function BusinessObjectDataRecordStorageAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = "BusinessObjectDataRecordStorage";

        function GetDataRecordStorageTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordStorageTemplateConfigs"));
        }

        return ({
            GetDataRecordStorageTemplateConfigs: GetDataRecordStorageTemplateConfigs,

        });
    }

    appControllers.service('VR_GenericData_BusinessObjectDataRecordStorageAPIService', BusinessObjectDataRecordStorageAPIService);

})(appControllers);