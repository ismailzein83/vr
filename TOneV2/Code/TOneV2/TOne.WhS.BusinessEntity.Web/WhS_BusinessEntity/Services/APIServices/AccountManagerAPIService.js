(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetCarriers(userId)  {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "GetCarriers"), {
                userId: userId
            });
        }

        function  GetAssignedCarriers(managerId, withDescendants, carrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "AccountManager", "GetAssignedCarriers"), {
                managerId: managerId,
                withDescendants: withDescendants,
                carrierType: carrierType
            });

        }
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
            GetCarriers: GetCarriers,
            GetAssignedCarriers: GetAssignedCarriers,
            GetAssignedCarriersFromTempTable: GetFilteredAssignedCarriers,
            GetLinkedOrgChartId: GetLinkedOrgChartId,
            AssignCarriers: AssignCarriers,
            UpdateLinkedOrgChart: UpdateLinkedOrgChart,
            GetAssignedCarriersDetail: GetAssignedCarriersDetail
        });
    }

    appControllers.service('WhS_BE_AccountManagerAPIService', carrierAccountAPIService);

})(appControllers);