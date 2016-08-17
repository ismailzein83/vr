(function (appControllers) {

    'use strict';

    DataSourceImportedBatchAPIService.$inject = ['BaseAPIService', 'VR_Integration_ModuleConfig', 'UtilsService', 'SecurityService'];

    function DataSourceImportedBatchAPIService(BaseAPIService, VR_Integration_ModuleConfig, UtilsService, SecurityService) {
        return ({
            GetFilteredDataSourceImportedBatches: GetFilteredDataSourceImportedBatches,
            HasViewImportedBatchesPermission: HasViewImportedBatchesPermission
        });

        function GetFilteredDataSourceImportedBatches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSourceImportedBatch', 'GetFilteredDataSourceImportedBatches'), input);
        }

        function HasViewImportedBatchesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Integration_ModuleConfig.moduleName, 'DataSourceImportedBatch', ['GetFilteredDataSourceImportedBatches']));
        }

    }

    appControllers.service('VR_Integration_DataSourceImportedBatchAPIService', DataSourceImportedBatchAPIService);

})(appControllers);
