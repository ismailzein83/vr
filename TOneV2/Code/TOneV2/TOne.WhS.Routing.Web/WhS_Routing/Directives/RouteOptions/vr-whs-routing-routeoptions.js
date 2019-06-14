"use strict";

app.directive('vrWhsRoutingRouteoptions', ['UtilsService', 'WhS_Routing_RouteOptionEvaluatedStatusEnum',
    function (UtilsService, WhS_Routing_RouteOptionEvaluatedStatusEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new routeOptions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteOptions/Templates/RouteOptionsTemplate.html"
        };

        function routeOptions($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var hasViewRatesPermission;

            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.RouteOptionDetails = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        hasViewRatesPermission = payload.hasViewRatesPermission;
                        $scope.scopeModel.routeOptionDetails = payload.routeOptionDetails;
                        extendRouteOptionDetails($scope.scopeModel.routeOptionDetails);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendRouteOptionDetails(routeOptionDetails) {
                if (routeOptionDetails != undefined) {
                    for (var i = 0; i < routeOptionDetails.length; i++) {
                        var currentRouteOptionDetail = routeOptionDetails[i];

                        if (hasViewRatesPermission) 
                            currentRouteOptionDetail.title = "Rate: " + currentRouteOptionDetail.SupplierRate + ", Services: " + currentRouteOptionDetail.ExactSupplierServiceSymbols;
                       
                        else 
                            currentRouteOptionDetail.title = "Services: " + currentRouteOptionDetail.ExactSupplierServiceSymbols;
                        
                        var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, currentRouteOptionDetail.EvaluatedStatus, "value");
                        if (evaluatedStatus != undefined) {
                            currentRouteOptionDetail.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                        }

                        if (currentRouteOptionDetail.Backups) {
                            for (var j = 0; j < currentRouteOptionDetail.Backups.length; j++) {
                                var currentRouteBackupOptionDetail = currentRouteOptionDetail.Backups[j];

                                if (hasViewRatesPermission) 
                                    currentRouteBackupOptionDetail.title = "Rate: " + currentRouteBackupOptionDetail.SupplierRate + ", Services: " + currentRouteBackupOptionDetail.ExactSupplierServiceSymbols;
                                else
                                    currentRouteBackupOptionDetail.title = "Services: " + currentRouteBackupOptionDetail.ExactSupplierServiceSymbols;

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