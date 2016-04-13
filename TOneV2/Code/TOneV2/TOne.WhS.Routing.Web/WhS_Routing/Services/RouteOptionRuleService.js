(function (appControllers) {

    'use strict';

    RouteOptionRuleService.$inject = ['WhS_Routing_RouteOptionRuleAPIService', 'VRModalService', 'VRNotificationService'];

    function RouteOptionRuleService(WhS_Routing_RouteOptionRuleAPIService, VRModalService, VRNotificationService) {
        return ({
            addRouteOptionRule: addRouteOptionRule,
            editRouteOptionRule: editRouteOptionRule,
            deleteRouteOptionRule: deleteRouteOptionRule
        });

        function addRouteOptionRule(onRouteOptionRuleAdded, routingProductId, sellingNumberPlanId) {
            var settings = {
            };

            var parameters = {
                routingProductId: routingProductId,
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleAdded = onRouteOptionRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, settings);
        }

        function editRouteOptionRule(routeOptionRuleId, onRouteOptionRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                routeOptionRuleId: routeOptionRuleId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleUpdated = onRouteOptionRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, modalSettings);
        }

        function deleteRouteOptionRule(scope, routeRuleObj, onRouteOptionRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_Routing_RouteOptionRuleAPIService.DeleteRule(routeRuleObj.Entity.OptionRuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Route Option Rule", deletionResponse);
                                onRouteOptionRuleDeleted(routeRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_Routing_RouteOptionRuleService', RouteOptionRuleService);

})(appControllers);
