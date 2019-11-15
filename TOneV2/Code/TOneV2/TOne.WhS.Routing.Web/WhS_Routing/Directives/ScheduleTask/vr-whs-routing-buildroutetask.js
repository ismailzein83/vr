"use strict";

app.directive("vrWhsRoutingBuildroutetask", ['UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'VRUIUtilsService', 'WhS_Routing_RoutingProcessModeEnum',
    function (UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, VRUIUtilsService, WhS_Routing_RoutingProcessModeEnum) {

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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ScheduleTask/Templates/BuildRouteTaskTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var switchSelectorAPI;
            var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.effectiveAfterInMinutes = 0;
                $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                $scope.routingProcessModes = UtilsService.getArrayEnum(WhS_Routing_RoutingProcessModeEnum);
                $scope.routeAnalysisModeValue = WhS_Routing_RoutingProcessModeEnum.RouteAnalysis.value;
                $scope.routeBuildWithAnalysisModeValue = WhS_Routing_RoutingProcessModeEnum.RouteBuildWithAnalysis.value;

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

                    var selectedSwitches;
                    var selectedRiskyMarginCategories;

                    if (payload != undefined && payload.data != undefined) {
                        $scope.effectiveAfterInMinutes = payload.data.EffectiveAfterInMinutes;
                        $scope.selectedRoutingProcessMode = UtilsService.getEnum(WhS_Routing_RoutingProcessModeEnum, 'value', payload.data.RoutingProcessMode);
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', payload.data.RoutingDatabaseType);

                        selectedSwitches = payload.data.Switches;
                        selectedRiskyMarginCategories = payload.data.RiskyMarginCategories;
                    }
                    else {
                        $scope.selectedRoutingProcessMode = WhS_Routing_RoutingProcessModeEnum.RouteBuild;
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
                    }


                    var promises = [];

                    var loadSwitchSelectorPromise = loadSwitchSelector();
                    promises.push(loadSwitchSelectorPromise);

                    var loadStatusDefinitionSelectorPromise = loadStatusDefinitionSelector();
                    promises.push(loadStatusDefinitionSelectorPromise);

                    function loadSwitchSelector() {
                        var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        switchSelectorReadyDeferred.promise.then(function () {

                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.BusinessEntity.Business.SyncWithinRouteBuildSwitchFilter, TOne.WhS.BusinessEntity.Business"
                                    }]
                                }
                            };
                            if (selectedSwitches != undefined) {
                                payload.selectedIds = selectedSwitches;
                            }
                            VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
                        });

                        return switchSelectorLoadDeferred.promise;
                    }

                    function loadStatusDefinitionSelector() {
                        var loadStatusDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        statusDefinitionSelectorReadyDeferred.promise.then(function () {

                            var statusDefinitionSelectorPayload = {
                                businessEntityDefinitionId: 'eacc1749-481c-4b14-9d2d-843f1ca5c723',
                                selectedIds: selectedRiskyMarginCategories
                            };
                            VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, loadStatusDefinitionSelectorPromiseDeferred);
                        });

                        return loadStatusDefinitionSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var switches;
                    if (!$scope.isFuture && $scope.selectedRoutingProcessMode.value != $scope.routeAnalysisModeValue) {
                        switches = switchSelectorAPI.getSelectedIds();
                    }

                    var riskyMarginCategories;
                    if ($scope.selectedRoutingProcessMode.value == $scope.routeAnalysisModeValue ||
                        $scope.selectedRoutingProcessMode.value == $scope.routeBuildWithAnalysisModeValue) {
                        riskyMarginCategories = statusDefinitionSelectorAPI.getSelectedIds();
                    }

                    return {
                        $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        IsFuture: $scope.isFuture,
                        RoutingProcessMode: $scope.selectedRoutingProcessMode.value,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                        EffectiveAfterInMinutes: $scope.effectiveAfterInMinutes,
                        Switches: switches,
                        RiskyMarginCategories: riskyMarginCategories
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }]);