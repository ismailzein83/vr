(function (appControllers) {

    "use strict";
    dataRecordTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function dataRecordTypeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        function GetFilteredDataRecordTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordType", "GetFilteredDataRecordTypes"), input);
        }

        function GetDataRecordType(dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "DataRecordType", "GetDataRecordType"), {
                dataRecordTypeId: dataRecordTypeId
            });
        }

        return ({
            GetFilteredDataRecordTypes: GetFilteredDataRecordTypes,
            GetDataRecordType: GetDataRecordType,
        });
    }

    appControllers.service('VRCommon_DataRecordTypeAPIService', dataRecordTypeAPIService);
})(appControllers);