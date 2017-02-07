(function (appControllers) {

    'use strict';

    RouteRuleService.$inject = ['WhS_Routing_RouteRuleAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService'];

    function RouteRuleService(WhS_Routing_RouteRuleAPIService, VRModalService, VRNotificationService, UtilsService) {
        return ({
            addRouteRule: addRouteRule,
            editRouteRule: editRouteRule,
            deleteRouteRule: deleteRouteRule,
            viewRouteRule: viewRouteRule
        });

        function addRouteRule(onRouteRuleAdded, routingProductId, sellingNumberPlanId) {
            var settings = {
            };

            var parameters = {
                routingProductId: routingProductId,
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteRuleAdded = onRouteRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, settings);
        }

        function editRouteRule(routeRuleId, onRouteRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }
        function viewRouteRule(routeRuleId) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId
            };

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }
        function deleteRouteRule(scope, routeRuleObj, onRouteRuleDeleted) {
            VRNotificationService.showConfirmation('Are you sure you want to delete the route rule ' + routeRuleObj.Entity.Name + '?')
                .then(function (response) {
                    if (response) {
                        return WhS_Routing_RouteRuleAPIService.DeleteRule(routeRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                                onRouteRuleDeleted(routeRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_Routing_RouteRuleService', RouteRuleService);

})(appControllers);
