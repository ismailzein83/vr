(function (appControllers) {

    'use strict';

    DataSourceImportedBatchAPIService.$inject = ['BaseAPIService', 'VR_Integration_ModuleConfig', 'UtilsService'];

    function DataSourceImportedBatchAPIService(BaseAPIService, VR_Integration_ModuleConfig, UtilsService) {
        return ({
            GetFilteredDataSourceImportedBatches: GetFilteredDataSourceImportedBatches,
        });

        function GetFilteredDataSourceImportedBatches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSourceImportedBatch', 'GetFilteredDataSourceImportedBatches'), input);
        }
    }

    appControllers.service('VR_Integration_DataSourceImportedBatchAPIService', DataSourceImportedBatchAPIService);

})(appControllers);
