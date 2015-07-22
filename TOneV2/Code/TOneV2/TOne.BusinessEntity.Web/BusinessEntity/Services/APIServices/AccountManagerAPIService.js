'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        GetAssignedCarriers: GetAssignedCarriers,
        GetLinkedOrgChartId: GetLinkedOrgChartId,
        AssignCarriers: AssignCarriers
    });
    
    function GetCarriers(userId, from, to) {
        return BaseAPIService.get('/api/AccountManager/GetCarriers', {
            userId: userId,
            from: from,
            to: to
        });
    }

    function GetAssignedCarriers(parameters) {
        return BaseAPIService.post('/api/AccountManager/GetAssignedCarriers', parameters);
    }

    function GetLinkedOrgChartId() {
        return BaseAPIService.get('/api/AccountManager/GetLinkedOrgChartId');
    }

    function AssignCarriers(updatedCarriers) {
        return BaseAPIService.post('/api/AccountManager/AssignCarriers', updatedCarriers);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('AccountManagerAPIService', serviceObj);