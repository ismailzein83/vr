﻿"use strict";

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

            var gridAPI;
            this.initializeController = initializeController;

            var switchSelectorAPI;
            var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                defineAPI();
            }

            function loadSwitchSelector() {
                var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                switchSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, undefined, switchSelectorLoadDeferred);
                });

                return switchSelectorLoadDeferred.promise;
            }

            function defineAPI() {

                $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);

                $scope.onRoutingDatabaseTypeSelectionChanged = function () {
                    $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
                };

                $scope.onSwitchSelectorReady = function (api) {
                    switchSelectorAPI = api;
                    switchSelectorReadyDeferred.resolve();
                };
                /* directive API definition */

                var api = {};
                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        IsFuture: $scope.isFuture,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                        Switches: !$scope.isFuture ? switchSelectorAPI.getSelectedIds() : null,
                        StoreCodeMatches: $scope.storeCodeMatches
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadSwitchSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
