﻿(function (appControllers) {

    'use strict';

    DataRecordTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataRecordTypeAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        function GetDataRecordTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", "GetDataRecordTypeInfo"), { filter: filter });
        }

        function GetRemoteDataRecordTypeInfo(connectionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", "GetRemoteDataRecordTypeInfo"), {
                connectionId: connectionId,
                filter: filter
            });
        }

        function GetFilteredDataRecordTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetFilteredDataRecordTypes'), input);
        }

        function GetDataRecordType(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetDataRecordType'), { dataRecordTypeId: dataRecordTypeId });
        }

        function GetDataRecordTypeToEdit(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetDataRecordTypeToEdit'), { dataRecordTypeId: dataRecordTypeId });
        }

        function AddDataRecordType(dataRecordTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'AddDataRecordType'), dataRecordTypeObject);
        }

        function HasAddDataRecordType() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", ['AddDataRecordType']));
        }

        function UpdateDataRecordType(dataRecordTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'UpdateDataRecordType'), dataRecordTypeObject);
        }

        function HasUpdateDataRecordType() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", ['UpdateDataRecordType']));
        }

        function GetDataRecordTypeExtraFieldsTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataRecordType", "GetDataRecordTypeExtraFieldsTemplates"));
        }

        return ({
            GetDataRecordType: GetDataRecordType,
            GetDataRecordTypeToEdit: GetDataRecordTypeToEdit,
            GetFilteredDataRecordTypes: GetFilteredDataRecordTypes,
            AddDataRecordType: AddDataRecordType,
            HasAddDataRecordType: HasAddDataRecordType,
            UpdateDataRecordType: UpdateDataRecordType,
            HasUpdateDataRecordType: HasUpdateDataRecordType,
            GetDataRecordTypeInfo: GetDataRecordTypeInfo,
            GetRemoteDataRecordTypeInfo: GetRemoteDataRecordTypeInfo,
            GetDataRecordTypeExtraFieldsTemplates: GetDataRecordTypeExtraFieldsTemplates
        });
    }

    appControllers.service('VR_GenericData_DataRecordTypeAPIService', DataRecordTypeAPIService);

})(appControllers);
