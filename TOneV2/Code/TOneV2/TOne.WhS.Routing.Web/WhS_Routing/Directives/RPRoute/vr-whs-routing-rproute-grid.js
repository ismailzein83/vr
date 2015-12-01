"use strict";

app.directive('vrWhsRoutingRprouteGrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteRuleService',
function (VRNotificationService, VRUIUtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RouteRuleService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new customerRouteGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteGridTemplate.html"

    };

    function customerRouteGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var gridDrillDownTabsObj;

        function initializeController() {
            $scope.rpRoutes = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        var drillDownDefinitions = initDrillDownDefinitions(query.RoutingDatabaseId);
                        gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                        return gridAPI.retrieveData(query);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_RPRouteAPIService.GetFilteredRPRoutes(dataRetrievalInput)
                   .then(function (response) {

                       if (response.Data != undefined) {
                           for (var i = 0; i < response.Data.length; i++) {
                               gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                           }
                       }

                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Rule",
                clicked: openRouteRuleEditor,
            }
            ];
        }

        function openRouteRuleEditor(dataItem) {
            WhS_Routing_RouteRuleService.editRouteRule(dataItem.Entity.ExecutedRuleId);
        }

        function initDrillDownDefinitions(routingDatabaseId) {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Details";
            drillDownDefinition.directive = "vr-whs-routing-rproute-details";

            drillDownDefinition.loadDirective = function (directiveAPI, rpRouteDetail) {
                var payload = {
                    rpRouteDetail: rpRouteDetail,
                    routingDatabaseId: routingDatabaseId
                };

                return directiveAPI.load(payload);
            };

            return [drillDownDefinition];
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
