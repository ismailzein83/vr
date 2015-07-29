'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        GetAssignedCarriers: GetAssignedCarriers,
        GetLinkedOrgChartId: GetLinkedOrgChartId,
        AssignCarriers: AssignCarriers,
        UpdateLinkedOrgChart: UpdateLinkedOrgChart
    });
    
    function GetCarriers(userId, from, to) {
        return BaseAPIService.get('/api/AccountManager/GetCarriers', {
            userId: userId,
            from: from,
            to: to
        });
    }

    function GetAssignedCarriers(managerId, withDescendants, carrierType) {
        return BaseAPIService.get('/api/AccountManager/GetAssignedCarriers', {
            managerId: managerId,
            withDescendants: withDescendants,
            carrierType: carrierType
        });
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