'use strict';

app.directive('vrWhsRoutingCustomerrouteDetails', ['WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BPInstanceStatusEnum',
    function (WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BPInstanceStatusEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new customerRouteDetailsCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteDetailTemplate.html"
        };

        function customerRouteDetailsCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var customerRoute;
            var gridAPI;
            var context;

            function initializeController() {
                $scope.routeOptionDetails = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.getMenuActions = function (dataItem) {
                    var menuActions = [];

                    if (dataItem.ExecutedRuleId) {
                        menuActions.push({
                            name: "Matching Rule",
                            clicked: viewRouteOptionRuleEditor,
                        });
                    }

                    if (dataItem.LinkedRouteOptionRuleIds != null && dataItem.LinkedRouteOptionRuleIds.length > 0) {
                        if (dataItem.LinkedRouteOptionRuleIds.length == 1) {
                            menuActions.push({
                                name: "Edit Rule",
                                clicked: editLinkedRouteOptionRule,
                                haspermission: hasUpdateRulePermission
                            });
                        }
                        else {
                            menuActions.push({
                                name: "Linked Rules",
                                clicked: viewLinkedRouteOptionRules,
                            });
                        }
                    }
                    else {
                        menuActions.push({
                            name: "Add Rule",
                            clicked: addRouteOptionRuleEditor,
                            haspermission: hasAddRulePermission
                        });
                    }

                    return menuActions;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        customerRoute = payload.customerRoute;
                        context = payload.context;
                    }

                    if (customerRoute != undefined && customerRoute.RouteOptionDetails != null) {
                        for (var i = 0; i < customerRoute.RouteOptionDetails.length; i++) {
                            var routeOptionDetail = customerRoute.RouteOptionDetails[i];
                            extendRouteOptionDetailObject(routeOptionDetail);
                            promises.push(routeOptionDetail.routeOptionDetailLoadDeferred.promise);
                            $scope.routeOptionDetails.push(routeOptionDetail);

                            if (routeOptionDetail.Backups != undefined) {
                                for (var j = 0; j < routeOptionDetail.Backups.length; j++) {
                                    var routeBackupOptionDetail = routeOptionDetail.Backups[j];
                                    extendRouteBackupOptionDetailObject(routeBackupOptionDetail);
                                    promises.push(routeBackupOptionDetail.routeOptionDetailLoadDeferred.promise);
                                    $scope.routeOptionDetails.push(routeBackupOptionDetail);
                                }
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendRouteOptionDetailObject(routeOptionDetail) {
                routeOptionDetail.routeOptionDetailLoadDeferred = UtilsService.createPromiseDeferred();
                routeOptionDetail.onServiceViewerReady = function (api) {
                    routeOptionDetail.serviceViewerAPI = api;

                    var serviceViewerPayload = {
                        selectedIds: routeOptionDetail.ExactSupplierServiceIds
                    };
                    VRUIUtilsService.callDirectiveLoad(routeOptionDetail.serviceViewerAPI, serviceViewerPayload, routeOptionDetail.routeOptionDetailLoadDeferred);
                };

                routeOptionDetail.IsBackup = false;
            }
            function extendRouteBackupOptionDetailObject(routeBackupOptionDetail) {
                routeBackupOptionDetail.routeOptionDetailLoadDeferred = UtilsService.createPromiseDeferred();
                routeBackupOptionDetail.onServiceViewerReady = function (api) {
                    routeBackupOptionDetail.serviceViewerAPI = api;

                    var serviceViewerPayload = {
                        selectedIds: routeBackupOptionDetail.ExactSupplierServiceIds
                    };
                    VRUIUtilsService.callDirectiveLoad(routeBackupOptionDetail.serviceViewerAPI, serviceViewerPayload, routeBackupOptionDetail.routeOptionDetailLoadDeferred);
                };

                routeBackupOptionDetail.IsBackup = true;
            }

            function addRouteOptionRuleEditor(dataItem) {
                var onRouteOptionRuleAdded = function (addedItem) {
                    triggerPartialRoute(addedItem.Entity.RuleId);
                };

                var linkedRouteOptionRuleInput = { RuleId: dataItem.ExecutedRuleId, CustomerId: customerRoute.CustomerId, Code: customerRoute.Code, SupplierId: dataItem.SupplierId, SupplierZoneId: dataItem.SupplierZoneId };
                WhS_Routing_RouteOptionRuleService.addLinkedRouteOptionRule(linkedRouteOptionRuleInput, customerRoute.Code, onRouteOptionRuleAdded);
            }

            function hasAddRulePermission() {
                return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            }

            function hasUpdateRulePermission() {
                return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
            }

            function viewLinkedRouteOptionRules(dataItem) {
                WhS_Routing_RouteOptionRuleService.viewLinkedRouteOptionRules(dataItem.LinkedRouteOptionRuleIds, customerRoute.Code);
            }

            function editLinkedRouteOptionRule(dataItem) {
                var onRouteRuleUpdated = function (updatedItem) {
                    triggerPartialRoute(updatedItem.Entity.RuleId);
                };
                WhS_Routing_RouteOptionRuleService.editLinkedRouteOptionRule(dataItem.LinkedRouteOptionRuleIds[0], customerRoute.Code, onRouteRuleUpdated);
            }

            function viewRouteOptionRuleEditor(dataItem) {
                WhS_Routing_RouteOptionRuleService.viewRouteOptionRule(dataItem.ExecutedRuleId);
            }

            function triggerPartialRoute(ruleId) {
                if (context != undefined && context.isDatabaseTypeCurrent) {
                    var inputArguments = {
                        $type: 'TOne.WhS.Routing.BP.Arguments.PartialRoutingProcessInput, TOne.WhS.Routing.BP.Arguments',
                        RouteOptionRuleId: ruleId
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
										if (context != undefined && context.refreshGrid != undefined && typeof (context.refreshGrid) == 'function') {
											context.refreshGrid().then(function () {
												$scope.isLoading = false;
											}).catch(function (error) {
												$scope.isLoading = false;
											});
										}
										else {
											$scope.isLoading = false;
										}
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
        }

        return directiveDefinitionObject;
    }]);