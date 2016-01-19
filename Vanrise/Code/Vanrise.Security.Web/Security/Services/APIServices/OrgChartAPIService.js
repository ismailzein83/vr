(function (appControllers) {

    'use strict';

    OrgChartAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService'];

    function OrgChartAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService) {
        return ({
            GetFilteredOrgCharts: GetFilteredOrgCharts,
            GetOrgChartById: GetOrgChartById,
            AddOrgChart: AddOrgChart,
            UpdateOrgChart: UpdateOrgChart,
            DeleteOrgChart: DeleteOrgChart
        });

        function GetFilteredOrgCharts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'OrgChart', 'GetFilteredOrgCharts'), input);
        }

        function GetOrgChartById(orgChartId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'OrgChart', 'GetOrgChartById'), {
                orgChartId: orgChartId
            });
        }

        function AddOrgChart(addedOrgChart) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'OrgChart', 'AddOrgChart'), addedOrgChart);
        }

        function UpdateOrgChart(updatedOrgChart) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'OrgChart', 'UpdateOrgChart'), updatedOrgChart);
        }

        function DeleteOrgChart(orgChartId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'OrgChart', 'DeleteOrgChart'), {
                orgChartId: orgChartId
            });
        }
    };

    appControllers.service('VR_Sec_OrgChartAPIService', OrgChartAPIService);

})(appControllers);
