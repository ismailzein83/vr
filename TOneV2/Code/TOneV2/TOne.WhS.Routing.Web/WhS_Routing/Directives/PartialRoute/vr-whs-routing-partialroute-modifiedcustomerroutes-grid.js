"use strict";

app.directive('vrWhsRoutingPartialrouteModifiedcustomerroutesGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_Routing_ModifiedCustomerRoutePreviewAPIService', 'WhS_Routing_RouteRuleService', 'WhS_Routing_RouteRuleAPIService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BPInstanceStatusEnum', 'WhS_Routing_RouteOptionEvaluatedStatusEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_Routing_ModifiedCustomerRoutePreviewAPIService, WhS_Routing_RouteRuleService, WhS_Routing_RouteRuleAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BPInstanceStatusEnum, WhS_Routing_RouteOptionEvaluatedStatusEnum) {

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

            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);
            var gridAPI;

            function initializeController() {
                $scope.customerRoutes = [];

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
                                    extendCutomerRouteObject(customerRoute.RouteOptionDetails);
                                    extendCutomerRouteObject(customerRoute.OrigRouteOptionDetails);
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

            function extendCutomerRouteObject(routeOptionDetails) {

                if (routeOptionDetails != undefined) {
                    for (var i = 0; i < routeOptionDetails.length; i++) {
                        var currentRouteOptionDetail = routeOptionDetails[i];

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
            }
        }

        return directiveDefinitionObject;
    }]);