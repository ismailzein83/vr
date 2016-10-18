(function (appControllers) {

    //'use strict';

    //DataRecordFieldTypeConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    ////function DataRecordFieldTypeConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
    ////    return ({
    ////        GetDataRecordFieldTypes: GetDataRecordFieldTypes,
    ////        GetDataRecordFieldTypeConfig: GetDataRecordFieldTypeConfig
    ////    });
    ////    function GetDataRecordFieldTypes() {
    ////        return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFieldTypeConfig", "GetDataRecordFieldTypes"));
    ////    }

    ////    function GetDataRecordFieldTypeConfig(configId) {
    ////        return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFieldTypeConfig", "GetDataRecordFieldTypeConfig"),
    ////            {
    ////                configId: configId
    ////            });
    ////    }
       
    //}

    appControllers.service('VR_GenericData_DataRecordFieldTypeConfigAPIService', DataRecordFieldTypeConfigAPIService);

})(appControllers);
