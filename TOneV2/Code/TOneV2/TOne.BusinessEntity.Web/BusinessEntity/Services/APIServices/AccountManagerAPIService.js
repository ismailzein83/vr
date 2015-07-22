'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        GetAssignedCarriers: GetAssignedCarriers,
        AssignCarriers: AssignCarriers
    });
    
    function GetCarriers(from, to) {
        return BaseAPIService.get('/api/AccountManager/GetCarriers', {
            from: from,
            to: to
        });
    }

    function GetAssignedCarriers(parameters) {
        return BaseAPIService.post('/api/AccountManager/GetAssignedCarriers', parameters);
    }

    function AssignCarriers(updatedCarriers) {
        return BaseAPIService.post('/api/AccountManager/AssignCarriers', updatedCarriers);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('AccountManagerAPIService', serviceObj);