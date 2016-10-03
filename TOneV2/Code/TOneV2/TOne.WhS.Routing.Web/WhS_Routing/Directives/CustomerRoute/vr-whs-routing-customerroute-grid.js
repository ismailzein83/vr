"use strict";

app.directive('vrWhsRoutingCustomerrouteGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_Routing_CustomerRouteAPIService', 'WhS_Routing_RouteRuleService',
function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_Routing_CustomerRouteAPIService, WhS_Routing_RouteRuleService) {

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
        templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteGridTemplate.html"

    };

    function customerRouteGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var gridDrillDownTabsObj;

        function initializeController() {
            $scope.showGrid = false;
            $scope.customerRoutes = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = initDrillDownDefinitions();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_CustomerRouteAPIService.GetFilteredCustomerRoutes(dataRetrievalInput)
                   .then(function (response) {
                       var _promises = [];

                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               var customerRoute = response.Data[i];
                               extendCutomerRouteObject(customerRoute);
                               _promises.push(customerRoute.cutomerRouteLoadDeferred.promise);
                               gridDrillDownTabsObj.setDrillDownExtensionObject(customerRoute);
                           }
                       }
                       onResponseReady(response);

                       //showGrid
                       UtilsService.waitMultiplePromises(_promises).then(function () {
                           $scope.showGrid = true;
                       }).catch(function (error) {
                           VRNotificationService.notifyExceptionWithClose(error, $scope);
                       });
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function extendCutomerRouteObject(customerRoute)
        {
            console.log(customerRoute);

            customerRoute.cutomerRouteLoadDeferred = UtilsService.createPromiseDeferred();
            customerRoute.onServiceViewerReady = function (api) {
                customerRoute.serviceViewerAPI = api;

                var serviceViewerPayload;
                if (customerRoute.Entity != undefined) {
                    serviceViewerPayload = {
                        selectedIds: customerRoute.Entity.CustomerServiceIds 
                    };
                }

                VRUIUtilsService.callDirectiveLoad(customerRoute.serviceViewerAPI, serviceViewerPayload, customerRoute.cutomerRouteLoadDeferred);
            };
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Rule",
                clicked: openRouteRuleEditor,
            }];
        }
        function openRouteRuleEditor(dataItem) {
            WhS_Routing_RouteRuleService.editRouteRule(dataItem.Entity.ExecutedRuleId);
        }

        function initDrillDownDefinitions()
        {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Details";
            drillDownDefinition.directive = "vr-whs-routing-customerroute-details";

            drillDownDefinition.loadDirective = function (directiveAPI, customerRoute) {
                var payload = {
                    customerRoute: customerRoute
                };

                return directiveAPI.load(payload);
            };

            return [drillDownDefinition];
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
