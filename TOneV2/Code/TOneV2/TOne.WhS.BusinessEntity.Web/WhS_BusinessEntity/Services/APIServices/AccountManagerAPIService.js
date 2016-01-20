(function (appControllers) {

    'use strict';

    CarrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function CarrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        return ({
            GetLinkedOrgChartId: GetLinkedOrgChartId,
            GetFilteredAssignedCarriers: GetFilteredAssignedCarriers,
            GetAssignedCarrierDetails: GetAssignedCarrierDetails,
            UpdateLinkedOrgChart: UpdateLinkedOrgChart,
            AssignCarriers: AssignCarriers
        });

        function GetLinkedOrgChartId() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'AccountManager', 'GetLinkedOrgChartId'));
        }

        function GetFilteredAssignedCarriers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'AccountManager', 'GetFilteredAssignedCarriers'), input);
        }

        function GetAssignedCarrierDetails(managerId, withDescendants, carrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'AccountManager', 'GetAssignedCarriersDetail'), {
                managerId: managerId,
                withDescendants: withDescendants,
                carrierType: carrierType
            });
        }

        function UpdateLinkedOrgChart(orgChartId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'AccountManager', 'UpdateLinkedOrgChart'), {
                orgChartId: orgChartId
            });
        }

        function AssignCarriers(updatedCarriers) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'AccountManager', 'AssignCarriers'), updatedCarriers);
        }
    }

    appControllers.service('WhS_BE_AccountManagerAPIService', CarrierAccountAPIService);

})(appControllers);
