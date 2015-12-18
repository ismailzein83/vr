(function (appControllers) {

    "use strict";
    dataRecordFieldAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function dataRecordFieldAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        function GetFilteredDataRecordFields(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "GetFilteredDataRecordFields"), input);
        }

        function UpdateDataRecordField(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "UpdateDataRecordField"), input);
        }

        function AddDataRecordField(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "AddDataRecordField"), input);
        }

        function GetDataRecordFieldTypeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "GetDataRecordFieldTypeTemplates"));
        }

        function GetDataRecordField(dataRecordFieldId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "GetDataRecordField"), {
                dataRecordFieldId: dataRecordFieldId
            });
        }

        function DeleteDataRecordField(dataRecordFieldId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordField", "DeleteDataRecordField"), {
                dataRecordFieldId: dataRecordFieldId
            });
        }

        return ({
            GetFilteredDataRecordFields: GetFilteredDataRecordFields,
            UpdateDataRecordField: UpdateDataRecordField,
            AddDataRecordField: AddDataRecordField,
            GetDataRecordFieldTypeTemplates: GetDataRecordFieldTypeTemplates,
            GetDataRecordField: GetDataRecordField,
            DeleteDataRecordField: DeleteDataRecordField
        });
    }

    appControllers.service('VRCommon_DataRecordFieldAPIService', dataRecordFieldAPIService);
})(appControllers);