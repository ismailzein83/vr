"use strict";

app.directive("whsRoutesyncProcessScheduled", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
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
                }
            },
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/ProcessInput/Scheduled/Templates/RouteSyncProcessManualTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;
            var routeSyncDefinitionSelectorAPI;
            var routeSyncDefinitionSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onRouteSyncDefintionSelectorReady = function (api) {
                    routeSyncDefinitionSelectorAPI = api;
                    routeSyncDefinitionSelectorAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "TOne.WhS.RouteSync.BP.Arguments.RouteSyncProcessInput, TOne.WhS.RouteSync.BP.Arguments",
                            RouteSyncDefinitionId: routeSyncDefinitionSelectorAPI.getSelectedIds()
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadRouteSyncDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRouteSyncDefinitionSelector() {
                var routeSyncDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                routeSyncDefinitionSelectorAPIReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(routeSyncDefinitionSelectorAPI, undefined, routeSyncDefinitionSelectorLoadDeferred);
                });

                return routeSyncDefinitionSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);
