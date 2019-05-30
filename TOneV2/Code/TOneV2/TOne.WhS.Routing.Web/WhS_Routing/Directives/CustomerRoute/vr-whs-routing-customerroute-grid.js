﻿"use strict";

app.directive('vrWhsRoutingCustomerrouteGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_Routing_CustomerRouteAPIService', 'WhS_Routing_RouteRuleService', 'WhS_Routing_RouteRuleAPIService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BPInstanceStatusEnum', 'WhS_Routing_RouteOptionEvaluatedStatusEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_Routing_CustomerRouteAPIService, WhS_Routing_RouteRuleService, WhS_Routing_RouteRuleAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BPInstanceStatusEnum, WhS_Routing_RouteOptionEvaluatedStatusEnum) {

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
            this.initializeController = initializeController;

            var hidemenuactions = $attrs.hidemenuactions != undefined;
            var isDatabaseTypeCurrent;
            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.customerRoutes = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = initDrillDownDefinitions();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady, retrieveDataContext) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    if (!retrieveDataContext.isDataSorted)
                        dataRetrievalInput.Query.ApplyDefaultSorting = true; //customer + code
                    else
                        dataRetrievalInput.Query.ApplyDefaultSorting = false;

                    WhS_Routing_CustomerRouteAPIService.GetFilteredCustomerRoutes(dataRetrievalInput).then(function (response) {
                        var customerRouteLoadGridPromises = [];

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var customerRoute = response.Data[i];
                                customerRouteLoadGridPromises.push(extendCutomerRouteObject(customerRoute));
                                gridDrillDownTabsObj.setDrillDownExtensionObject(customerRoute);
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

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    $scope.isLoading = true;
                    isDatabaseTypeCurrent = query.isDatabaseTypeCurrent;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendCutomerRouteObject(customerRoute) {

                var promises = [];

                if (customerRoute != undefined && customerRoute.RouteOptionDetails != undefined) {
                    customerRoute.cutomerRouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(customerRoute.cutomerRouteOptionsLoadDeferred.promise);

                    customerRoute.onRouteOptionsReady = function (api) {
                        customerRoute.routeOptionsAPI = api;

                        var routeOptionsPayload;
                        if (customerRoute.RouteOptionDetails != undefined) {
                            routeOptionsPayload = { routeOptionDetails: customerRoute.RouteOptionDetails };
                        }
                        VRUIUtilsService.callDirectiveLoad(customerRoute.routeOptionsAPI, routeOptionsPayload, customerRoute.cutomerRouteOptionsLoadDeferred);
                    };
                }

                customerRoute.cutomerRouteLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(customerRoute.cutomerRouteLoadDeferred.promise);

                customerRoute.onServiceViewerReady = function (api) {
                    customerRoute.serviceViewerAPI = api;

                    var serviceViewerPayload;
                    if (customerRoute.SaleZoneServiceIds != undefined) {
                        serviceViewerPayload = { selectedIds: customerRoute.SaleZoneServiceIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(customerRoute.serviceViewerAPI, serviceViewerPayload, customerRoute.cutomerRouteLoadDeferred);
                };

                return UtilsService.waitMultiplePromises(promises);
            }

            function defineMenuActions() {
                if (!hidemenuactions) {
                    $scope.gridMenuActions = function (dataItem) {
                        var menu = [];

                        if (dataItem.CanEditMatchingRule) {
                            menu.push({
                                name: "Edit Matching Rule",
                                clicked: editLinkedRouteRule,
                                haspermission: hasUpdateRulePermission
                            });
                        }
                        else {
                            menu.push({
                                name: "Matching Rule",
                                clicked: viewRouteRuleEditor,
                            });
                        }

                        if (dataItem.CanAddRuleByCode || dataItem.LinkedRouteRuleIds == undefined || dataItem.LinkedRouteRuleIds.length == 0) {
                            menu.push({
                                name: "Add Rule By Code",
                                clicked: addRouteRuleByCode,
                                haspermission: hasAddRulePermission
                            });
                        }

                        if (dataItem.CanAddRuleByZone) {
                            menu.push({
                                name: "Add Rule By Zone",
                                clicked: addRouteRuleByZone,
                                haspermission: hasAddRulePermission
                            });
                        }

                        if (dataItem.CanAddRuleByCountry) {
                            menu.push({
                                name: "Add Rule By Country",
                                clicked: addRouteRuleByCountry,
                                haspermission: hasAddRulePermission
                            });
                        }

                        if (dataItem.LinkedRouteRuleIds != undefined && dataItem.LinkedRouteRuleIds.length > 1) {
                            menu.push({
                                name: "Linked Rules",
                                clicked: viewLinkedRouteRules
                            });
                        }
                        return menu;
                    };
                }
            }

            function hasUpdateRulePermission() {
                return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
            }

            function hasAddRulePermission() {
                return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
            }

            function viewLinkedRouteRules(dataItem) {
                var customerRouteData = { code: dataItem.Code, SaleZoneServiceIds: dataItem.SaleZoneServiceIds, Rate: dataItem.Rate, CustomerId: dataItem.CustomerId };

                var onRouteRuleUpdated = function (updatedItem) {
                    triggerPartialRoute(updatedItem.Entity.RuleId);
                };
                WhS_Routing_RouteRuleService.viewLinkedRouteRules(dataItem.LinkedRouteRuleIds, customerRouteData, onRouteRuleUpdated);
            }

            function editLinkedRouteRule(dataItem) {
                var customerRouteData = { code: dataItem.Code, SaleZoneServiceIds: dataItem.SaleZoneServiceIds, Rate: dataItem.Rate, CustomerId: dataItem.CustomerId };

                var onRouteRuleUpdated = function (updatedItem) {
                    triggerPartialRoute(updatedItem.Entity.RuleId);
                };
                WhS_Routing_RouteRuleService.editLinkedRouteRule(dataItem.ExecutedRuleId, customerRouteData, onRouteRuleUpdated);
            }

            function viewRouteRuleEditor(dataItem) {
                var customerRouteData = { code: dataItem.Code, SaleZoneServiceIds: dataItem.SaleZoneServiceIds, Rate: dataItem.Rate, CustomerId: dataItem.CustomerId };
                WhS_Routing_RouteRuleService.viewRouteRule(dataItem.ExecutedRuleId, customerRouteData);
            }

            function addRouteRuleByCode(dataItem) {
                var linkedRouteRuleInput = { CustomerId: dataItem.CustomerId, Code: dataItem.Code, RuleId: dataItem.ExecutedRuleId, RouteOptions: dataItem.Options };
                addRouteRule(dataItem, linkedRouteRuleInput);
            }

            function addRouteRuleByZone(dataItem) {
                var linkedRouteRuleInput = {
                    CustomerId: dataItem.CustomerId, Code: dataItem.Code, SaleZoneId: dataItem.SaleZoneId, RuleId: dataItem.ExecutedRuleId, RouteOptions: dataItem.Options
                };
                addRouteRule(dataItem, linkedRouteRuleInput);
            }

            function addRouteRuleByCountry(dataItem) {
                var linkedRouteRuleInput = {
                    CustomerId: dataItem.CustomerId, Code: dataItem.Code, SaleZoneId: dataItem.SaleZoneId, RuleId: dataItem.ExecutedRuleId, RouteOptions: dataItem.Options, RuleByCountry: true
                };
                addRouteRule(dataItem, linkedRouteRuleInput);
            }

            function addRouteRule(dataItem, linkedRouteRuleInput) {
                var onRouteRuleAdded = function (addedItem) {
                    triggerPartialRoute(addedItem.Entity.RuleId);
                };
                var customerRouteData = { code: dataItem.Code, SaleZoneServiceIds: dataItem.SaleZoneServiceIds, Rate: dataItem.Rate, CustomerId: dataItem.CustomerId };
                WhS_Routing_RouteRuleService.addLinkedRouteRule(linkedRouteRuleInput, customerRouteData, onRouteRuleAdded);
            }

            function triggerPartialRoute(ruleId) {
                if (isDatabaseTypeCurrent) {
                    var inputArguments = {
                        $type: 'TOne.WhS.Routing.BP.Arguments.PartialRoutingProcessInput, TOne.WhS.Routing.BP.Arguments',
                        RouteRuleId: ruleId
                    };

                    var input = {
                        InputArguments: inputArguments
                    };
                    $scope.isLoading = true;
                    BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                        if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                            var processTrackingContext = {
                                automaticCloseWhenCompleted: true,
                                onClose: function (bpInstanceClosureContext) {

                                    if (bpInstanceClosureContext != undefined && bpInstanceClosureContext.bpInstanceStatusValue === BPInstanceStatusEnum.Completed.value) {
                                        $scope.isLoading = true;
                                        gridAPI.refreshGrid().then(function () {
                                            $scope.isLoading = false;
                                        }).catch(function (error) {
                                            $scope.isLoading = false;
                                        });
                                    }
                                    else {
                                        $scope.isLoading = false;
                                    }
                                }
                            };

                            BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                        }
                    }).catch(function (error) {
                        $scope.isLoading = false;
                    });
                }
            }

            function initDrillDownDefinitions() {
                var drillDownDefinition = {};

                drillDownDefinition.title = "Details";
                drillDownDefinition.directive = "vr-whs-routing-customerroute-details";

                drillDownDefinition.loadDirective = function (directiveAPI, customerRoute) {
                    var payload = {
                        customerRoute: customerRoute,
                        context: buildContext()
                    };

                    return directiveAPI.load(payload);
                };
                return [drillDownDefinition];
            }

            function buildContext() {
                return {
                    isDatabaseTypeCurrent: isDatabaseTypeCurrent,
                    refreshGrid: function () { return gridAPI.refreshGrid(); }
                };
            }
        }

        return directiveDefinitionObject;
    }]);