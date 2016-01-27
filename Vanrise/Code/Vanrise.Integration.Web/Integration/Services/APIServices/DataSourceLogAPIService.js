(function (appControllers) {

    'use strict';

    DataSourceLogAPIService.$inject = ['BaseAPIService', 'VR_Integration_ModuleConfig', 'UtilsService'];

    function DataSourceLogAPIService(BaseAPIService, VR_Integration_ModuleConfig, UtilsService) {
        return ({
            GetFilteredDataSourceLogs: GetFilteredDataSourceLogs
        });

        function GetFilteredDataSourceLogs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName,'DataSourceLog','GetFilteredDataSourceLogs'), input);
        }
    }

    appControllers.service('VR_Integration_DataSourceLogAPIService', DataSourceLogAPIService);

})(appControllers);
