'use strict';

app.directive('vrWhsRoutingCustomerrouteApprovalStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RoutingApprovalTypeEnum', 'WhS_Routing_UtilsService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RoutingApprovalTypeEnum, WhS_Routing_UtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new customerRouteApprovalStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteApprovalStaticEditorTemplate.html"
        };

        function customerRouteApprovalStaticEditor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var selectedValues;
            var routingDatabaseId;
            var routingDatabaseType;

            var gridFiltersAPI;
            var gridFiltersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.legendHeader = "Legend";
                $scope.scopeModel.legendContent = WhS_Routing_UtilsService.getLegendContent();

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGridFiltersReady = function (api) {
                    gridFiltersAPI = api;
                    gridFiltersReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.searchClicked = function () {
                    if (gridAPI != undefined)
                        return gridAPI.loadGrid(gridFiltersAPI.getData());
                };

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            routingDatabaseId = selectedValues.RoutingDatabaseId;
                            routingDatabaseType = selectedValues.RoutingDatabaseType;
                        }
                    }

                    $scope.scopeModel.approvalType = UtilsService.getArrayEnum(WhS_Routing_RoutingApprovalTypeEnum);

                    promises.push(loadGridFilters());

                    UtilsService.waitMultiplePromises(promises).then(function () {

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });

                    function loadGridFilters() {
                        var loadGridFiltersPromiseDeferred = UtilsService.createPromiseDeferred();

                        var payload = {
                            routingDatabase: {
                                routingDatabaseId: routingDatabaseId,
                                routingDatabaseType: routingDatabaseType
                            }
                        };


                        gridFiltersReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(gridFiltersAPI, payload, loadGridFiltersPromiseDeferred);
                        });

                        return loadGridFiltersPromiseDeferred.promise;
                    }
                };

                api.setData = function (approvalObject) {
                    approvalObject["Decision"] = $scope.scopeModel.selectedApprovalType.value;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);