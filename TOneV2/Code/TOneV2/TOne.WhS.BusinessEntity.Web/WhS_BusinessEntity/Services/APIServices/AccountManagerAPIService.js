(function (appControllers) {

    'use strict';

    CarrierAccountAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function CarrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "AccountManager";

        function GetLinkedOrgChartId() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetLinkedOrgChartId"));
        }

        function GetFilteredAssignedCarriers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredAssignedCarriers"), input);
        }

        function GetAssignedCarrierDetails(managerId, withDescendants, carrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAssignedCarriersDetail"), {
                managerId: managerId,
                withDescendants: withDescendants,
                carrierType: carrierType
            });
        }

        function UpdateLinkedOrgChart(orgChartId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateLinkedOrgChart"), {
                orgChartId: orgChartId
            });
        }

        function AssignCarriers(updatedCarriers) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AssignCarriers"), updatedCarriers);
        }

        function HasUpdateLinkedOrgChartPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateLinkedOrgChart']));
        }

        function HasAssignCarriersPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AssignCarriers']));
        }

        return ({
            GetLinkedOrgChartId: GetLinkedOrgChartId,
            GetFilteredAssignedCarriers: GetFilteredAssignedCarriers,
            GetAssignedCarrierDetails: GetAssignedCarrierDetails,
            UpdateLinkedOrgChart: UpdateLinkedOrgChart,
            AssignCarriers: AssignCarriers,
            HasUpdateLinkedOrgChartPermission: HasUpdateLinkedOrgChartPermission,
            HasAssignCarriersPermission: HasAssignCarriersPermission
        });

    }

    appControllers.service("WhS_BE_AccountManagerAPIService", CarrierAccountAPIService);

})(appControllers);
