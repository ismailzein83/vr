﻿(function (appControllers) {

    'use strict';

    RouteRuleService.$inject = ['WhS_Routing_RouteRuleAPIService', 'VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteRuleService(WhS_Routing_RouteRuleAPIService, VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {

        var drillDownDefinitions = [];

        function addRouteRule(onRouteRuleAdded, context) {
            var settings = {
            };

            var parameters = {
                routingProductId: context != undefined ? context.routingProductId : undefined,
                sellingNumberPlanId: context != undefined ? context.sellingNumberPlanId : undefined,
                isLinkedRouteRule: false,
                defaultRouteRuleValues: context != undefined ? context.defaultRouteRuleValues : undefined
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
                routeRuleId: routeRuleId,
                isLinkedRouteRule: false
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }

        function setRouteRuleDeleted(scope, routeRuleObj, onRouteRuleDeleted) {
            VRNotificationService.showConfirmation('Are you sure you want to delete this route rule?').then(function (response) {
                if (response) {
                    return WhS_Routing_RouteRuleAPIService.SetRouteRuleDeleted(routeRuleObj.Entity.RuleId).then(function (deletionResponse) {
                        VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                        onRouteRuleDeleted(routeRuleObj);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        function setRouteRulesDeleted(routeRulesIds, onSetRouteRulesDeleted) {
            VRNotificationService.showConfirmation('Are you sure you want to delete selected items?').then(function (response) {
                if (response) {
                    return WhS_Routing_RouteRuleAPIService.SetRouteRulesDeleted({ RuleIds: routeRulesIds }).then(function (deletionResponse) {
                        VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                        onSetRouteRulesDeleted();
                    });
                }
            });
        }

        function viewRouteRule(routeRuleId, customerRouteData) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId,
                isLinkedRouteRule: false,
                customerRouteData: customerRouteData
            };

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }

        function addLinkedRouteRule(linkedRouteRuleInput, customerRouteData, onRouteRuleAdded) {
            var modalSettings = {
            };
            var parameters = {
                linkedRouteRuleInput: linkedRouteRuleInput,
                customerRouteData: customerRouteData,
                isLinkedRouteRule: true
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteRuleAdded = onRouteRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }

        function viewLinkedRouteRules(linkedRouteRuleIds, customerRouteData, onRouteRuleUpdated) {
            var settings = {
            };

            var parameters = {
                linkedRouteRuleIds: linkedRouteRuleIds,
                customerRouteData: customerRouteData,
                onRouteRuleUpdated: onRouteRuleUpdated
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/LinkedRouteRuleEditor.html', parameters, settings);
        }

        function editLinkedRouteRule(routeRuleId, customerRouteData, onRouteRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                routeRuleId: routeRuleId,
                customerRouteData: customerRouteData,
                isLinkedRouteRule: true
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
        }

        function viewRouteRules(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteRule/RouteRuleEditor.html', modalParameters, modalSettings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_Routing_RouteRules_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewRouteRules(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function getEntityUniqueName() {
            return "WhS_Routing_RouteRules";
        }

        function registerObjectTrackingDrillDownToRouteRules() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, routeRulesItem) {
                routeRulesItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: routeRulesItem.Entity.RuleId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return routeRulesItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }


        return ({
            addRouteRule: addRouteRule,
            editRouteRule: editRouteRule,
            setRouteRuleDeleted: setRouteRuleDeleted,
            setRouteRulesDeleted: setRouteRulesDeleted,
            viewRouteRule: viewRouteRule,
            addLinkedRouteRule: addLinkedRouteRule,
            viewLinkedRouteRules: viewLinkedRouteRules,
            editLinkedRouteRule: editLinkedRouteRule,
            registerObjectTrackingDrillDownToRouteRules: registerObjectTrackingDrillDownToRouteRules,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        });
    }

    appControllers.service('WhS_Routing_RouteRuleService', RouteRuleService);

})(appControllers);