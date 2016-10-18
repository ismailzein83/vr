(function (appControllers) {

    'use strict';

    DataRecordFieldAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordFieldAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataRecordFieldsInfo: GetDataRecordFieldsInfo,
            GetDataRecordAttributes: GetDataRecordAttributes,
            GetDataRecordFieldFormulaExtensionConfigs: GetDataRecordFieldFormulaExtensionConfigs,
            GetDataRecordFieldTypeConfigs: GetDataRecordFieldTypeConfigs
        });

        function GetDataRecordFieldFormulaExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFields", "GetDataRecordFieldFormulaExtensionConfigs"));
        }


        function GetDataRecordFieldsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFields", "GetDataRecordFieldsInfo"),
                {
                    serializedFilter: serializedFilter
                });
        }

        function GetDataRecordAttributes(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFields", "GetDataRecordAttributes"),
                {
                    dataRecordTypeId: dataRecordTypeId
                });
        }

        function GetDataRecordFieldTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordFields", "GetDataRecordFieldTypeConfigs"));
        }
        
    }

    appControllers.service('VR_GenericData_DataRecordFieldAPIService', DataRecordFieldAPIService);

})(appControllers);
