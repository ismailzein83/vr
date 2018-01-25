(function (appControllers) {

    'use strict';

    RouteOptionRuleService.$inject = ['WhS_Routing_RouteOptionRuleAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteOptionRuleService(WhS_Routing_RouteOptionRuleAPIService, VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addRouteOptionRule: addRouteOptionRule,
            editRouteOptionRule: editRouteOptionRule,
            deleteRouteOptionRule: deleteRouteOptionRule,
            viewRouteOptionRule: viewRouteOptionRule,
            addLinkedRouteOptionRule: addLinkedRouteOptionRule,
            viewLinkedRouteOptionRules: viewLinkedRouteOptionRules,
            editLinkedRouteOptionRule: editLinkedRouteOptionRule,
            registerObjectTrackingDrillDownToRouteOptionRules: registerObjectTrackingDrillDownToRouteOptionRules,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction,
            setRouteOptionsRulesDeleted: setRouteOptionsRulesDeleted
        });
        function viewRouteOptionRules(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleEditor.html', modalParameters, modalSettings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_Routing_RouteOptionRules_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewRouteOptionRules(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }
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

        function setRouteOptionsRulesDeleted(routeOptionsIds, onSetRouteOptionsRulesDeleted) {
            VRNotificationService.showConfirmation('Are you sure you want to delete these route options rules ?')
                .then(function (response) {
                    if (response) {
                        return WhS_Routing_RouteOptionRuleAPIService.SetRouteOptionsRulesDeleted({
                            RouteOptionsIds: routeOptionsIds
                        }).then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Route Option Rule", deletionResponse);
                            onSetRouteOptionsRulesDeleted();
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


        function getEntityUniqueName() {
            return "WhS_Routing_RouteOptionRules";
        }

        function registerObjectTrackingDrillDownToRouteOptionRules() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, routeOptionRulesItem) {
                routeOptionRulesItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: routeOptionRulesItem.Entity.RuleId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return routeOptionRulesItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    };

    appControllers.service('WhS_Routing_RouteOptionRuleService', RouteOptionRuleService);

})(appControllers);
