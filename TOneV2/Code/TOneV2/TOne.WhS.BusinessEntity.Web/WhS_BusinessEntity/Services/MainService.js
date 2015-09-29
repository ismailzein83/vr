
app.service('WhS_BE_MainService', ['WhS_BE_RouteRuleAPIService', 'VRModalService', 'VRNotificationService', function (WhS_BE_RouteRuleAPIService, VRModalService, VRNotificationService) {

    return ({
        addRouteRule: addRouteRule,
        editRouteRule: editRouteRule,
        deleteRouteRule: deleteRouteRule
    });

    function addRouteRule(onRouteRuleAdded)
    {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onRoutingProductAdded = onRouteRuleAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRuleEditor.html', null, settings);
    }

    function editRouteRule(routeRuleObj, onRouteRuleUpdated) {
        var modalSettings = {
        };
        var parameters = {
            routingProductId: routeRuleObj.RouteRuleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Route Rule";
            modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRuleEditor.html', parameters, modalSettings);
    }

    function deleteRouteRule() {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {

                    return WhS_BE_RouteRuleAPIService.DeleteRouteRule(routeRuleObj.RoutRuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

}]);
