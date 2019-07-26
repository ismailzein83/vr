"use strict";

app.directive('vrWhsRoutingPartialrouteModifiedcustomerroutesGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_Routing_ModifiedCustomerRoutePreviewAPIService', 'WhS_Routing_RouteRuleService', 'WhS_Routing_RouteRuleAPIService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BPInstanceStatusEnum', 'WhS_Routing_RouteOptionEvaluatedStatusEnum', 'WhS_Routing_CustomerRouteAPIService',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_Routing_ModifiedCustomerRoutePreviewAPIService, WhS_Routing_RouteRuleService, WhS_Routing_RouteRuleAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BPInstanceStatusEnum, WhS_Routing_RouteOptionEvaluatedStatusEnum, WhS_Routing_CustomerRouteAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new partialrouteModifiedcustomerroutesGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/PartialRoute/Templates/ModifiedCustomerRouteGridTemplate.html"
        };

        function partialrouteModifiedcustomerroutesGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var hasViewRatesPermission;

            function initializeController() {
                $scope.customerRoutes = [];
                $scope.showGrid = false;
                $scope.isLoading = true;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady, retrieveDataContext) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Routing_ModifiedCustomerRoutePreviewAPIService.GetAllModifiedCustomerRoutes(dataRetrievalInput).then(function (response) {
                        var customerRouteLoadGridPromises = [];

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var customerRoute = response.Data[i];
                                if (customerRoute != undefined) {
                                    customerRouteLoadGridPromises.push(extendCutomerRouteObject(customerRoute));
                                }
                            }
                        }
                        onResponseReady(response);

                        UtilsService.waitMultiplePromises(customerRouteLoadGridPromises).then(function () {
                            loadGridPromiseDeffered.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            loadGridPromiseDeffered.reject();
                        }).finally(function () {
                            $scope.isLoading = false;
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        loadGridPromiseDeffered.reject();
                    });

                    return loadGridPromiseDeffered.promise;
                };

                $scope.getColor = function (dataItem) {
                    var cssClass = 'span-summary bold-label';
                    if (dataItem.IsBlocked)
                        cssClass += ' danger-font';

                    return cssClass;
                };

                WhS_Routing_CustomerRouteAPIService.HasViewCustomerRouteRatesPermission().then(function (response) {
                    hasViewRatesPermission = response;
                    defineMenuActions();
                    $scope.showGrid = true;
                });
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    $scope.isLoading = true;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendCutomerRouteObject(customerRoute) {
                var promises = [];
                if (customerRoute != undefined) {
                    if (customerRoute.OrigRouteOptionDetails != undefined) {
                        customerRoute.origCutomerRouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(customerRoute.origCutomerRouteOptionsLoadDeferred.promise);

                        customerRoute.onOrigRouteOptionsReady = function (api) {
                            customerRoute.origRouteOptionsAPI = api;

                            var origRouteOptionsPayload = {
                                routeOptionDetails: customerRoute.OrigRouteOptionDetails,
                                hasViewRatesPermission: hasViewRatesPermission
                            };
                            VRUIUtilsService.callDirectiveLoad(customerRoute.origRouteOptionsAPI, origRouteOptionsPayload, customerRoute.origCutomerRouteOptionsLoadDeferred);
                        };
                    }

                    if (customerRoute.RouteOptionDetails != undefined) {
                        customerRoute.cutomerRouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(customerRoute.cutomerRouteOptionsLoadDeferred.promise);

                        customerRoute.onRouteOptionsReady = function (api) {
                            customerRoute.routeOptionsAPI = api;
                            var routeOptionsPayload = {
                                hasViewRatesPermission: hasViewRatesPermission,
                                routeOptionDetails: customerRoute.RouteOptionDetails
                            };

                            VRUIUtilsService.callDirectiveLoad(customerRoute.routeOptionsAPI, routeOptionsPayload, customerRoute.cutomerRouteOptionsLoadDeferred);
                        };
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = function (dataItem) {
                    if (hasViewRatesPermission) {
                        var menu = [];
                        menu.push({
                            name: "Matching Rule",
                            clicked: viewRouteRuleEditor
                        });
                        return menu;
                    }
                };
            }

            function viewRouteRuleEditor(dataItem) {
                var customerRouteData = { code: dataItem.Code, SaleZoneServiceIds: dataItem.SaleZoneServiceIds, Rate: dataItem.Rate, CustomerId: dataItem.CustomerId };
                WhS_Routing_RouteRuleService.viewRouteRule(dataItem.ExecutedRuleId, customerRouteData);
            }
        }

        return directiveDefinitionObject;
    }]);