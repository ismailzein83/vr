(function (appControllers) {

    'use strict';

    DataRecordTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordTypeAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataRecordType: GetDataRecordType,
            GetFilteredDataRecordTypes: GetFilteredDataRecordTypes,
            AddDataRecordType: AddDataRecordType,
            UpdateDataRecordType: UpdateDataRecordType,
            GetDataRecordFieldTypeTemplates: GetDataRecordFieldTypeTemplates,
            GetDataRecordTypeInfo: GetDataRecordTypeInfo
        });
        function GetDataRecordTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "DataRecordType", "GetDataRecordTypeInfo"), { filter: filter });
        }
        function GetFilteredDataRecordTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetFilteredDataRecordTypes'), input);
        }

        function GetDataRecordType(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetDataRecordType'), { dataRecordTypeId: dataRecordTypeId });
        }
        function AddDataRecordType(dataRecordTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'AddDataRecordType'), dataRecordTypeObject);
        }
        function UpdateDataRecordType(dataRecordTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'UpdateDataRecordType'), dataRecordTypeObject);
        }
        function GetDataRecordFieldTypeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", "GetDataRecordFieldTypeTemplates"));
        }
    }

    appControllers.service('VR_GenericData_DataRecordTypeAPIService', DataRecordTypeAPIService);

})(appControllers);
