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
                var ctor = new DirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/ProcessInput/Scheduled/Templates/RouteSyncProcessScheduledTemplate.html"
        };

        function DirectiveCtor($scope, ctrl) {
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

                api.load = function (payload) {

                    var routeSyncDefinitionId;

                    if (payload != undefined) {
                        if(payload.data != undefined){
                            routeSyncDefinitionId = payload.data.RouteSyncDefinitionId;
                        }
                    }

                    var promises = [];

                    var loadRouteSyncDefinitionSelectorPromise = loadRouteSyncDefinitionSelector();
                    promises.push(loadRouteSyncDefinitionSelectorPromise);

                    function loadRouteSyncDefinitionSelector() {
                        var routeSyncDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        routeSyncDefinitionSelectorAPIReadyDeferred.promise.then(function () {

                            var routeSyncDefinitionSelectorPayload = {
                                selectedIds: routeSyncDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(routeSyncDefinitionSelectorAPI, routeSyncDefinitionSelectorPayload, routeSyncDefinitionSelectorLoadDeferred);
                        });

                        return routeSyncDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.RouteSync.BP.Arguments.RouteSyncProcessInput, TOne.WhS.RouteSync.BP.Arguments",
                        RouteSyncDefinitionId: routeSyncDefinitionSelectorAPI.getSelectedIds()
                    };
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
