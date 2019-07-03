(function (appControllers) {

    'use strict';

    DataRecordFieldAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordFieldAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = "DataRecordFields";

        function GetDataRecordFieldFormulaExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordFieldFormulaExtensionConfigs"));
        }

        function GetDataRecordFieldsInfo(dataRecordTypeId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordFieldsInfo"), {
                dataRecordTypeId: dataRecordTypeId,
                serializedFilter: serializedFilter
            });
        }

        function GetDataRecordAttributes(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordAttributes"), {
                dataRecordTypeId: dataRecordTypeId
            });
        }

        function GetDataRecordFieldTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetDataRecordFieldTypeConfigs"));
        }
        function GetListRecordRuntimeViewTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetListRecordRuntimeViewTypeConfigs"));
        }
        function TryResolveDifferences(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "TryResolveDifferences"), input);
        }

        function GetFieldCustomObjectTypeSettingsConfig() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetFieldCustomObjectTypeSettingsConfig"));
        }

        function GetFieldTypeDescription(fieldType, fieldValue) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetFieldTypeDescription"),
                {
                    FieldType: fieldType,
                    FieldValue: fieldValue
                });
        }
        function GetFieldTypeListDescription(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetFieldTypeListDescription"), input);
        }
        function GetListDataRecordTypeGridViewColumnAtts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetListDataRecordTypeGridViewColumnAtts"), input);
        }
        function GetFieldsDescription(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetFieldsDescription"), input);
        }
        return ({
            GetDataRecordFieldsInfo: GetDataRecordFieldsInfo,
            GetDataRecordAttributes: GetDataRecordAttributes,
            GetDataRecordFieldFormulaExtensionConfigs: GetDataRecordFieldFormulaExtensionConfigs,
            GetDataRecordFieldTypeConfigs: GetDataRecordFieldTypeConfigs,
            TryResolveDifferences: TryResolveDifferences,
            GetFieldCustomObjectTypeSettingsConfig: GetFieldCustomObjectTypeSettingsConfig,
            GetListRecordRuntimeViewTypeConfigs: GetListRecordRuntimeViewTypeConfigs,
            GetFieldTypeDescription: GetFieldTypeDescription,
            GetFieldTypeListDescription: GetFieldTypeListDescription,
            GetListDataRecordTypeGridViewColumnAtts: GetListDataRecordTypeGridViewColumnAtts,
            GetFieldsDescription: GetFieldsDescription
        });

    }

    appControllers.service('VR_GenericData_DataRecordFieldAPIService', DataRecordFieldAPIService);

})(appControllers);
