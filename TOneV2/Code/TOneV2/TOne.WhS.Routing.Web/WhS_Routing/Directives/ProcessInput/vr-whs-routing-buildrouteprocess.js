"use strict";

app.directive("vrWhsRoutingBuildrouteprocess", ['UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'VRUIUtilsService', 'VRDateTimeService', 'WhS_Routing_RoutingProcessModeEnum',
    function (UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, VRUIUtilsService, VRDateTimeService, WhS_Routing_RoutingProcessModeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/BuildRouteProcessTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var switchSelectorAPI;
            var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                $scope.routingProcessModes = UtilsService.getArrayEnum(WhS_Routing_RoutingProcessModeEnum);
                $scope.selectedRoutingProcessMode = WhS_Routing_RoutingProcessModeEnum.RouteBuild;
                $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
                $scope.routingProcessModeAnalysisValue = WhS_Routing_RoutingProcessModeEnum.Analysis.value;
                $scope.routingProcessModeRouteBuildWithAnalysisValue = WhS_Routing_RoutingProcessModeEnum.RouteBuildWithAnalysis.value;

                $scope.onSwitchSelectorReady = function (api) {
                    switchSelectorAPI = api;
                    switchSelectorReadyDeferred.resolve();
                };

                $scope.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.onRoutingDatabaseTypeSelectionChanged = function () {
                    $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (!$scope.isFuture) {
                        $scope.effectiveOn = VRDateTimeService.getNowDateTime();
                    }

                    var promises = [];

                    var loadSwitchSelectorPromise = loadSwitchSelector();
                    promises.push(loadSwitchSelectorPromise);

                    var loadStatusDefinitionSelectorPromise = loadStatusDefinitionSelector();
                    promises.push(loadStatusDefinitionSelectorPromise);

                    function loadSwitchSelector() {
                        var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        switchSelectorReadyDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.BusinessEntity.Business.SyncWithinRouteBuildSwitchFilter, TOne.WhS.BusinessEntity.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, selectorPayload, switchSelectorLoadDeferred);
                        });

                        return switchSelectorLoadDeferred.promise;
                    }

                    function loadStatusDefinitionSelector() {
                        var loadStatusDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        statusDefinitionSelectorReadyDeferred.promise.then(function () {

                            var statusDefinitionSelectorPayload = {
                                businessEntityDefinitionId: 'eacc1749-481c-4b14-9d2d-843f1ca5c723'
                            };
                            VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, loadStatusDefinitionSelectorPromiseDeferred);
                        });

                        return loadStatusDefinitionSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var switches;
                    if (!$scope.isFuture && $scope.selectedRoutingProcessMode.value != $scope.routingProcessModeAnalysisValue) {
                        switches = switchSelectorAPI.getSelectedIds();
                    }

                    var riskyMarginCategories;
                    if ($scope.selectedRoutingProcessMode.value == $scope.routingProcessModeAnalysisValue ||
                        $scope.selectedRoutingProcessMode.value == $scope.routingProcessModeRouteBuildWithAnalysisValue) {
                        riskyMarginCategories = statusDefinitionSelectorAPI.getSelectedIds();
                    }

                    return {
                        InputArguments: {
                            $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                            EffectiveTime: !$scope.isFuture ? $scope.effectiveOn : null,
                            IsFuture: $scope.isFuture,
                            RoutingProcessMode: $scope.selectedRoutingProcessMode.value,
                            RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                            RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                            Switches: switches,
                            RiskyMarginCategories: riskyMarginCategories
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);