app.service('WhS_Sales_MainService', ["VRModalService", function (VRModalService) {

    return ({
        editRatePlan: editRatePlan,
    });

    function editRatePlan(customerId, onRatePlanUpdated) {

        var modalSettings = {
        };

        var parameters = {
            CustomerId: customerId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onRatePlanUpdated = onRatePlanUpdated;
        };

        VRModalService.showModal('/Client/Modules/WhS_Sales/Views/RatePlanEditor.html', parameters, modalSettings);
    }
}]);
