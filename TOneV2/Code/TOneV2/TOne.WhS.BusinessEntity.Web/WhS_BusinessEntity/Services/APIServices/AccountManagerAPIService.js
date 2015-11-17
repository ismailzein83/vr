(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        function GetAssignedCarriersDetail(managerId, withDescendants, carrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "GetAssignedCarriersDetail"), {
                managerId: managerId,
                withDescendants: withDescendants,
                carrierType: carrierType
            });

        }

        function GetFilteredAssignedCarriers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "GetFilteredAssignedCarriers"), input);
        }

        function GetLinkedOrgChartId() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "GetLinkedOrgChartId"));
        }

        function UpdateLinkedOrgChart(orgChartId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "UpdateLinkedOrgChart"), {
                orgChartId: orgChartId
            });
        }

        function AssignCarriers(updatedCarriers) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "AssignCarriers"), updatedCarriers);
        }

        return ({
            GetFilteredAssignedCarriers: GetFilteredAssignedCarriers,
            GetLinkedOrgChartId: GetLinkedOrgChartId,
            AssignCarriers: AssignCarriers,
            UpdateLinkedOrgChart: UpdateLinkedOrgChart,
            GetAssignedCarriersDetail: GetAssignedCarriersDetail
        });
    }

    appControllers.service('WhS_BE_AccountManagerAPIService', carrierAccountAPIService);

})(appControllers);