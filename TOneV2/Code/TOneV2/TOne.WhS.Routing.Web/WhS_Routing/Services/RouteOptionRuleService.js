(function (appControllers) {

    'use strict';

    RouteOptionRuleService.$inject = ['WhS_Routing_RouteOptionRuleAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService'];

    function RouteOptionRuleService(WhS_Routing_RouteOptionRuleAPIService, VRModalService, VRNotificationService, UtilsService) {
        return ({
            addRouteOptionRule: addRouteOptionRule,
            editRouteOptionRule: editRouteOptionRule,
            deleteRouteOptionRule: deleteRouteOptionRule,
            viewRouteOptionRule: viewRouteOptionRule,
            addLinkedRouteOptionRule: addLinkedRouteOptionRule,
            viewLinkedRouteOptionRules: viewLinkedRouteOptionRules,
            editLinkedRouteOptionRule: editLinkedRouteOptionRule
        });

        function addRouteOptionRule(onRouteOptionRuleAdded, routingProductId, sellingNumberPlanId) {
            var settings = {
            };

            var parameters = {
                routingProductId: routingProductId,
                sellingNumberPlanId: sellingNumberPlanId,
                isLinkedRouteRule: false
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleAdded = onRouteOptionRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, settings);
        };

        function editRouteOptionRule(routeRuleId, onRouteOptionRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId,
                isLinkedRouteRule: false
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleUpdated = onRouteOptionRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, modalSettings);
        };

        function deleteRouteOptionRule(scope, routeRuleObj, onRouteOptionRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_Routing_RouteOptionRuleAPIService.DeleteRule(routeRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Route Option Rule", deletionResponse);
                                onRouteOptionRuleDeleted(routeRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        };

        function viewRouteOptionRule(routeRuleId) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId,
                isLinkedRouteRule: false
            };

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, modalSettings);
        };

        function addLinkedRouteOptionRule(linkedRouteOptionRuleInput, linkedCode, onRouteOptionRuleAdded) {
            var modalSettings = {
            };
            var parameters = {
                linkedRouteOptionRuleInput: linkedRouteOptionRuleInput,
                linkedCode: linkedCode,
                isLinkedRouteRule: true
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleAdded = onRouteOptionRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, modalSettings);
        };

        function viewLinkedRouteOptionRules(linkedRouteOptionRuleIds, linkedCode) {
            var settings = {
            };

            var parameters = {
                linkedRouteOptionRuleIds: linkedRouteOptionRuleIds,
                linkedCode: linkedCode
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/LinkedRouteOptionRuleEditor.html', parameters, settings);
        };

        function editLinkedRouteOptionRule(routeRuleId, linkedCode, onRouteOptionRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId,
                linkedCode: linkedCode,
                isLinkedRouteRule: true
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteOptionRuleUpdated = onRouteOptionRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', parameters, modalSettings);
        };
    };

    appControllers.service('WhS_Routing_RouteOptionRuleService', RouteOptionRuleService);

})(appControllers);
