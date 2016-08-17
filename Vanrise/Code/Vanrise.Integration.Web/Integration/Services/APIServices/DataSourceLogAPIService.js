(function (appControllers) {

    'use strict';

    DataSourceLogAPIService.$inject = ['BaseAPIService', 'VR_Integration_ModuleConfig', 'UtilsService', 'SecurityService'];

    function DataSourceLogAPIService(BaseAPIService, VR_Integration_ModuleConfig, UtilsService, SecurityService) {
        return ({
            GetFilteredDataSourceLogs: GetFilteredDataSourceLogs,
            HasViewFilteredDataSourcePermission: HasViewFilteredDataSourcePermission
        });

        function GetFilteredDataSourceLogs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName,'DataSourceLog','GetFilteredDataSourceLogs'), input);
        }
        function HasViewFilteredDataSourcePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Integration_ModuleConfig.moduleName, 'DataSourceLog', ['GetFilteredDataSourceLogs']));
        }
    }

    appControllers.service('VR_Integration_DataSourceLogAPIService', DataSourceLogAPIService);

})(appControllers);
