"use strict";

app.directive("vrWhsSalesDefaultroutingproduct", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var constructor = new Constructor(ctrl, $scope);
                constructor.initializeController();

            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/DefaultRoutingProductTemplate.html"
        };

        function Constructor(ctrl, $scope) {

            function initializeController() {
                var currentBED;
                var currentEED;

                var readyPromises = [];

                var currentSelectorAPI;
                var currentSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                readyPromises.push(currentSelectorReadyPromiseDeferred.promise);

                var newSelectorAPI;
                var newSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                readyPromises.push(newSelectorReadyPromiseDeferred.promise);

                ctrl.onCurrentSelectorReady = function (api) {
                    currentSelectorAPI = api;
                    currentSelectorReadyPromiseDeferred.resolve();
                };

                ctrl.onNewSelectorReady = function (api) {
                    newSelectorAPI = api;
                    newSelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(readyPromises).then(function () {
                    buildAPI();
                });

                function buildAPI() {
                    var api = {};

                    api.load = function (payload) {
                        var currentRoutingProductId;

                        if (payload != undefined) {
                            currentRoutingProductId = payload.CurrentRoutingProductId;
                            currentBED = payload.CurrentBED;
                            ctrl.beginEffectiveDate = payload.CurrentBED;
                            currentEED = payload.CurrentEED;
                            ctrl.endEffectiveDate = payload.CurrentEED;
                        }

                        var currentSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var currentSelectorPayload = {
                            filter: null,
                            selectedIds: currentRoutingProductId
                        };

                        VRUIUtilsService.callDirectiveLoad(currentSelectorAPI, currentSelectorPayload, currentSelectorLoadPromiseDeferred);

                        return currentSelectorLoadPromiseDeferred.promise.then(function () {
                            var newSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                            var newSelectorPayloadFilter = (currentRoutingProductId != undefined && currentRoutingProductId != null) ?
                                    { ExcludedRoutingProductId: currentRoutingProductId } : null;

                            var newSelectorPayload = {
                                filter: newSelectorPayloadFilter,
                                selectedIds: null
                            };

                            VRUIUtilsService.callDirectiveLoad(newSelectorAPI, newSelectorPayload, newSelectorLoadPromiseDeferred);

                            return newSelectorLoadPromiseDeferred.promise;
                        });
                    };

                    api.getDefaultChanges = function () {
                        var newRoutingProduct = buildNewRoutingProduct();
                        var routingProductChange = (newRoutingProduct == null) ? buildRoutingProductChange() : null;

                        return {
                            $type: "TOne.WhS.Sales.Entities.RatePlanning.DefaultChanges, TOne.WhS.Sales.Entities",
                            DefaultChanges: {
                                NewRoutingProduct: newRoutingProduct,
                                RoutingProductChange: routingProductChange
                            }
                        };

                        function buildNewRoutingProduct() {
                            var newRoutingProduct = null;
                            var newRoutingProductId = selectorAPI.getSelectedIds();

                            if (currentRoutingProductId != newRoutingProductId) {
                                newRoutingProduct = {
                                    RoutingProductId: newRoutingProductId,
                                    BED: $scope.bed,
                                    EED: $scope.eed
                                };
                            }

                            return newRoutingProduct;
                        }

                        function buildRoutingProductChange() {
                            var routingProductChange = null;
                            var newRoutingProductId = selectorAPI.getSelectedIds()

                            if (currentRoutingProductId == newRoutingProductId && $scope.eed != currentEED) {
                                routingProductChange = {
                                    DefaultRoutingProductId: null,
                                    EED: $scope.eed
                                };
                            }

                            return routingProductChange;
                        }
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);
