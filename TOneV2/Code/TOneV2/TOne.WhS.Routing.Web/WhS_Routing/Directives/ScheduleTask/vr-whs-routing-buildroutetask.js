"use strict";

app.directive("vrWhsRoutingBuildroutetask", ['UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'VRUIUtilsService',
    function (UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, VRUIUtilsService) {
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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ScheduleTask/Templates/BuildRouteTaskTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var switchSelectorAPI;
            var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.effectiveAfterInMinutes = 0;

                $scope.onSwitchSelectorReady = function (api) {
                    switchSelectorAPI = api;
                    switchSelectorReadyDeferred.resolve();
                };

                $scope.onRoutingDatabaseTypeSelectionChanged = function () {
                    $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
                };

                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        IsFuture: $scope.isFuture,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                        Switches: !$scope.isFuture ? switchSelectorAPI.getSelectedIds() : null,
                        EffectiveAfterInMinutes: $scope.effectiveAfterInMinutes
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                api.load = function (payload) {
                    $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);

                    if (payload != undefined && payload.data != undefined) {
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', payload.data.RoutingDatabaseType);
                        $scope.effectiveAfterInMinutes = payload.data.EffectiveAfterInMinutes;
                    }
                    else {
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
                    }

                    var promises = [];
                    var selectedSwitches = undefined;
                    if (payload != undefined && payload.data != undefined) {
                        selectedSwitches = payload.data.Switches;
                    }
                        
                    promises.push(loadSwitchSelector(selectedSwitches));
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadSwitchSelector(selectedIds) {
                var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                switchSelectorReadyDeferred.promise.then(function () {

                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.BusinessEntity.Business.SyncWithinRouteBuildSwitchFilter, TOne.WhS.BusinessEntity.Business"
                            }]
                        }
                    };
                    if (selectedIds != undefined) {
                        payload.selectedIds = selectedIds;
                    }
                    VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
                });

                return switchSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);
