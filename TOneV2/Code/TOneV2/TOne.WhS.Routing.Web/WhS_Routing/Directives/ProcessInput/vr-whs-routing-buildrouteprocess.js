"use strict";

app.directive("vrWhsRoutingBuildrouteprocess", ['UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'VRUIUtilsService', 'VRDateTimeService',
    function (UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, VRUIUtilsService, VRDateTimeService) {
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/BuildRouteProcessTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            var switchSelectorAPI;
            var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);

                $scope.onRoutingDatabaseTypeSelectionChanged = function () {
                    $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
                };

                $scope.onSwitchSelectorReady = function (api) {
                    switchSelectorAPI = api;
                    switchSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (!$scope.isFuture)
                        $scope.effectiveOn = VRDateTimeService.getNowDateTime();

                    var promises = [];
                    promises.push(loadSwitchSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                            EffectiveTime: !$scope.isFuture ? $scope.effectiveOn : null,
                            IsFuture: $scope.isFuture,
                            RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                            RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                            Switches: !$scope.isFuture ? switchSelectorAPI.getSelectedIds() : null,
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

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
        }

        return directiveDefinitionObject;
    }]);