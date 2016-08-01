(function (appControllers) {

    'use strict';

    DataRecordObjectTypeConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordObjectTypeConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataRecordObjectTypes: GetDataRecordObjectTypes,
            GetDataRecordObjectTypeConfig: GetDataRecordObjectTypeConfig
        });

        function GetDataRecordObjectTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordObjectTypeConfig", "GetDataRecordObjectTypes"));
        }

        function GetDataRecordObjectTypeConfig(configId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordObjectTypeConfig", "GetDataRecordObjectTypeConfig"),
                {
                    configId: configId
                });
        }

    }

    appControllers.service('VR_GenericData_DataRecordObjectTypeConfigAPIService', DataRecordObjectTypeConfigAPIService);

})(appControllers);
