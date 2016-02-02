﻿(function (appControllers) {

    'use strict';

    DataRecordFieldTypeConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordFieldTypeConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataRecordFieldTypes: GetDataRecordFieldTypes,
            GetDataRecordFieldType: GetDataRecordFieldType
        });
        function GetDataRecordFieldTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFieldTypeConfig", "GetDataRecordFieldTypes"));
        }

        function GetDataRecordFieldType() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFieldTypeConfig", "GetDataRecordFieldType"));
        }
       
    }

    appControllers.service('VR_GenericData_DataRecordFieldTypeConfigAPIService', DataRecordFieldTypeConfigAPIService);

})(appControllers);
