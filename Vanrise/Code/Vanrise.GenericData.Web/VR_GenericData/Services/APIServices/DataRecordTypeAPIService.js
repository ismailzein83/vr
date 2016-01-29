(function (appControllers) {

    'use strict';

    DataRecordTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordTypeAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataRecordType: GetDataRecordType,
            GetFilteredDataRecordTypes: GetFilteredDataRecordTypes,
        });

        function GetFilteredDataRecordTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetFilteredDataRecordTypes'), input);
        }

        function GetDataRecordType() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordType', 'GetDataRecordType'));
        }

    }

    appControllers.service('VR_GenericData_DataRecordTypeAPIService', DataRecordTypeAPIService);

})(appControllers);
