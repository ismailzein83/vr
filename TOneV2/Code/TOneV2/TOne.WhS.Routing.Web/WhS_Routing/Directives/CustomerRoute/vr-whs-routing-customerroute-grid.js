"use strict";

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

            var isDatabaseTypeCurrent;
            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.showGrid = false;
                $scope.customerRoutes = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = initDrillDownDefinitions();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Routing_CustomerRouteAPIService.GetFilteredCustomerRoutes(dataRetrievalInput).then(function (response) {
                        var customerRouteLoadGridPromises = [];

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var customerRoute = response.Data[i];
                                extendCutomerRouteObject(customerRoute);
                                customerRouteLoadGridPromises.push(customerRoute.cutomerRouteLoadDeferred.promise);
                                gridDrillDownTabsObj.setDrillDownExtensionObject(customerRoute);
                            }
                        }
                        onResponseReady(response);

                        //showGrid
                        UtilsService.waitMultiplePromises(customerRouteLoadGridPromises).then(function () {
                            $scope.showGrid = true;
                            loadGridPromiseDeffered.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            loadGridPromiseDeffered.reject();
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        loadGridPromiseDeffered.reject();
                    });

                    return loadGridPromiseDeffered.promise;
                };

                //$scope.getRowStyle = function (dataItem) {
                //    var rowStyle;
                //    if (dataItem.IsBlocked)
                //        rowStyle = { CssClass: "bg-danger" };
                //    return rowStyle;
                //};

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    isDatabaseTypeCurrent = query.isDatabaseTypeCurrent;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendCutomerRouteObject(customerRoute) {

                if (customerRoute != undefined && customerRoute.RouteOptionDetails != undefined) {
                    for (var i = 0; i < customerRoute.RouteOptionDetails.length; i++) {
                        var currentRouteOptionDetail = customerRoute.RouteOptionDetails[i];

                        var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, currentRouteOptionDetail.EvaluatedStatus, "value");
                        if (evaluatedStatus != undefined) {
                            currentRouteOptionDetail.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                        }

                        if (currentRouteOptionDetail.Backups) {
                            for (var j = 0; j < currentRouteOptionDetail.Backups.length; j++) {
                                var currentRouteBackupOptionDetail = currentRouteOptionDetail.Backups[j];

                                var backupEvaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, currentRouteBackupOptionDetail.EvaluatedStatus, "value");
                                if (backupEvaluatedStatus != undefined) {
                                    currentRouteBackupOptionDetail.EvaluatedStatusCssClass = backupEvaluatedStatus.cssclass;
                                }
                            }
                        }
                    }
                }

                customerRoute.cutomerRouteLoadDeferred = UtilsService.createPromiseDeferred();
                customerRoute.onServiceViewerReady = function (api) {
                    customerRoute.serviceViewerAPI = api;

                    var serviceViewerPayload;
                    if (customerRoute.SaleZoneServiceIds != undefined) {
                        serviceViewerPayload = { selectedIds: customerRoute.SaleZoneServiceIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(customerRoute.serviceViewerAPI, serviceViewerPayload, customerRoute.cutomerRouteLoadDeferred);
                };
            }

            function defineMenuActions() {
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

                    if (!dataItem.LinkedRouteRuleIds || dataItem.LinkedRouteRuleIds.length == 0) {
                        menu.push({
                            name: "Add Rule By Code",
                            clicked: addRouteRuleByCode,
                            haspermission: hasAddRulePermission
                        });

                        menu.push({
                            name: "Add Rule By Zone",
                            clicked: addRouteRuleByZone,
                            haspermission: hasAddRulePermission
                        });
                    }
                    else {
                        if (dataItem.LinkedRouteRuleIds.length > 1) {
                            menu.push({
                                name: "Linked Rules",
                                clicked: viewLinkedRouteRules
                            });
                        }
                    }
                    return menu;
                };
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

            function addRouteRule(dataItem, linkedRouteRuleInput) {
                var onRouteRuleAdded = function (addedItem) {
                    //var newDataItem = {
                    //    CustomerRouteDetailId: dataItem.CustomerRouteDetailId,
                    //    CustomerId:dataItem.CustomerId,
                    //    CustomerName: dataItem.CustomerName,
                    //    SaleZoneName: dataItem.SaleZoneName,
                    //    Code: dataItem.Code,
                    //    Rate: dataItem.Rate,
                    //    IsBlocked: dataItem.IsBlocked,
                    //    SaleZoneServiceIds: dataItem.SaleZoneServiceIds,
                    //    ExecutedRuleId: dataItem.ExecutedRuleId,
                    //    Options: dataItem.Options,
                    //    ExecutedRouteRuleName: dataItem.ExecutedRouteRuleName,
                    //    ExecutedRouteRuleSettingsTypeName: dataItem.ExecutedRouteRuleSettingsTypeName,
                    //    RouteOptionDetails: dataItem.RouteOptionDetails,
                    //    LinkedRouteRuleCount: 1,
                    //    LinkedRouteRuleIds: [],
                    //    CanEditMatchingRule: true
                    //};
                    //newDataItem.LinkedRouteRuleIds.push(addedItem.Entity.RuleId);

                    //extendCutomerRouteObject(newDataItem);
                    //gridDrillDownTabsObj.setDrillDownExtensionObject(newDataItem);
                    //gridAPI.itemUpdated(newDataItem);

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
                        customerRoute: customerRoute
                    };

                    return directiveAPI.load(payload);
                };
                return [drillDownDefinition];
            }
        }

        return directiveDefinitionObject;
    }]);