'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        GetAssignedCarriers: GetAssignedCarriers,
        GetAssignedCarriersFromTempTable: GetAssignedCarriersFromTempTable,
        GetLinkedOrgChartId: GetLinkedOrgChartId,
        AssignCarriers: AssignCarriers,
        UpdateLinkedOrgChart: UpdateLinkedOrgChart
    });
    
    function GetCarriers(userId) {
        return BaseAPIService.get('/api/AccountManager/GetCarriers', {
            userId: userId
        });
    }

    // called from the CarrierAssignmentEditor page
    function GetAssignedCarriers(managerId, withDescendants, carrierType) {
        return BaseAPIService.get('/api/AccountManager/GetAssignedCarriers', {
            managerId: managerId,
            withDescendants: withDescendants,
            carrierType: carrierType
        });
    }

    // called from the AccountManagerManagement page
    function GetAssignedCarriersFromTempTable(input) {
        return BaseAPIService.post('/api/AccountManager/GetAssignedCarriersFromTempTable', input);
    }

    function GetLinkedOrgChartId() {
        return BaseAPIService.get('/api/AccountManager/GetLinkedOrgChartId');
    }

    function UpdateLinkedOrgChart(orgChartId) {
        return BaseAPIService.get('/api/AccountManager/UpdateLinkedOrgChart', {
            orgChartId: orgChartId
        });
    }

    function AssignCarriers(updatedCarriers) {
        return BaseAPIService.post('/api/AccountManager/AssignCarriers', updatedCarriers);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('AccountManagerAPIService', serviceObj);