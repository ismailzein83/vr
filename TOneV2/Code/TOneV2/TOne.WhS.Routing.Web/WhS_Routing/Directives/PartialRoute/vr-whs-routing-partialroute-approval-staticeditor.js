'use strict';

app.directive('vrWhsRoutingPartialrouteApprovalStaticeditor', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutingApprovalTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RoutingApprovalTypeEnum) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new partialRouteApprovalStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/PartialRoute/Templates/PartialRouteApprovalStaticEditorDefinitionTemplate.html"
        };

        function partialRouteApprovalStaticEditor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var selectedValues;
            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var routingDatabaseId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.legendHeader = "Legend";
                $scope.scopeModel.legendContent = getLegendContent();

                $scope.scopeModel.totalChangedRoutes;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                };

                function getLegendContent() {
                    return '<div style="font-size:12px; margin:10px">' +
                        '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FF0000; margin: 0px 3px"></div> Blocked </div>' +
                        '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FFA500; margin: 0px 3px"></div> Lossy </div>' +
                        '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #0000FF; margin: 0px 3px"></div> Forced </div>' +
                        '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #28A744; margin: 0px 3px"></div> Market Price </div>' +
                        '</div>';
                }

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {

                    $scope.scopeModel.approvalType = UtilsService.getArrayEnum(WhS_Routing_RoutingApprovalTypeEnum);

                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            $scope.scopeModel.totalChangedRoutes = selectedValues.TotalChangedRoutes;
                            routingDatabaseId = selectedValues.RoutingDatabaseId;
                        }
                    }

                    function loadGrid() {
                        var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridReadyPromiseDeferred.promise.then(function () {
                            gridAPI.loadGrid(getFilterObject()).then(function () {
                                gridLoadPromiseDeferred.resolve();
                            });
                        });

                        return gridLoadPromiseDeferred.promise;
                    }

                    promises.push(loadGrid());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (approvalObject) {
                    approvalObject["Decision"] = $scope.scopeModel.selectedApprovalType.value;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getFilterObject() {

                var query = {
                    RoutingDatabaseId: routingDatabaseId
                };
                return query;
            }

        }
        return directiveDefinitionObject;
    }]);