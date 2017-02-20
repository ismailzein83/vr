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
                    };

                    return directiveAPI;
                };
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_CustomerRouteAPIService.GetFilteredCustomerRoutes(dataRetrievalInput)
                   .then(function (response) {
                       var _customerRouteServiceViewerPromises = [];

                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               var customerRoute = response.Data[i];
                               extendCutomerRouteObject(customerRoute);
                               _customerRouteServiceViewerPromises.push(customerRoute.cutomerRouteLoadDeferred.promise);
                               gridDrillDownTabsObj.setDrillDownExtensionObject(customerRoute);
                           }
                       }
                       onResponseReady(response);

                       //showGrid
                       UtilsService.waitMultiplePromises(_customerRouteServiceViewerPromises).then(function () {
                           $scope.showGrid = true;
                       }).catch(function (error) {
                           VRNotificationService.notifyExceptionWithClose(error, $scope);
                       });
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            $scope.getRowStyle = function (dataItem) {
                var rowStyle;

                if (dataItem.Entity.IsBlocked)
                    rowStyle = { CssClass: "bg-danger" };

                return rowStyle;
            };

            defineMenuActions();
        };

        function extendCutomerRouteObject(customerRoute) {
            customerRoute.cutomerRouteLoadDeferred = UtilsService.createPromiseDeferred();
            customerRoute.onServiceViewerReady = function (api) {
                customerRoute.serviceViewerAPI = api;

                var serviceViewerPayload;
                if (customerRoute.Entity != undefined) {
                    serviceViewerPayload = {
                        selectedIds: customerRoute.Entity.SaleZoneServiceIds
                    };
                }

                VRUIUtilsService.callDirectiveLoad(customerRoute.serviceViewerAPI, serviceViewerPayload, customerRoute.cutomerRouteLoadDeferred);
            };
        };

        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menu = [];
                if (dataItem.Entity.ExecutedRuleId != undefined) {
                    menu.push({
                        name: "Matching Rule",
                        clicked: viewRouteRuleEditor,
                    });
                }
                if (dataItem.LinkedRouteRuleIds != null && dataItem.LinkedRouteRuleIds.length > 0) {
                    if (dataItem.LinkedRouteRuleIds.length == 1) {
                        menu.push({
                            name: "Edit Rule",
                            clicked: editLinkedRouteRule,
                        });
                    }
                    else {
                        menu.push({
                            name: "Linked Rules",
                            clicked: viewLinkedRouteRules,
                        });
                    }
                }
                else {
                    menu.push({
                        name: "Add Rule",
                        clicked: addRouteRuleEditor,
                    });
                }
                return menu;
            }
        };
        function viewLinkedRouteRules(dataItem) {
            WhS_Routing_RouteRuleService.viewLinkedRouteRules(dataItem.LinkedRouteRuleIds, dataItem.Entity.Code);
        };

        function editLinkedRouteRule(dataItem) {
            WhS_Routing_RouteRuleService.editLinkedRouteRule(dataItem.LinkedRouteRuleIds[0], dataItem.Entity.Code);
        };

        function viewRouteRuleEditor(dataItem) {
            WhS_Routing_RouteRuleService.viewRouteRule(dataItem.Entity.ExecutedRuleId);
        };

        function addRouteRuleEditor(dataItem) {
            var onRouteRuleAdded = function (addedItem) {
                var newDataItem =
                    {
                        CustomerRouteDetailId: dataItem.CustomerRouteDetailId,
                        Entity: dataItem.Entity,
                        CustomerName: dataItem.CustomerName,
                        ZoneName: dataItem.ZoneName,
                        RouteOptionDetails: dataItem.RouteOptionDetails,
                        LinkedRouteRuleIds: []
                    };
                newDataItem.LinkedRouteRuleIds.push(addedItem.Entity.RuleId);
                extendCutomerRouteObject(newDataItem);
                gridDrillDownTabsObj.setDrillDownExtensionObject(newDataItem);
                gridAPI.itemUpdated(newDataItem);
            };

            var linkedRouteRuleInput = { RuleId: dataItem.Entity.ExecutedRuleId, RouteOptions: dataItem.Entity.Options, CustomerId: dataItem.Entity.CustomerId, Code: dataItem.Entity.Code };
            WhS_Routing_RouteRuleService.addLinkedRouteRule(linkedRouteRuleInput,dataItem.Entity.Code, onRouteRuleAdded);
        };

        function initDrillDownDefinitions() {
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
        };

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);