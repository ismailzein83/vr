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

                    var currentBED;
                    var currentEED;

                    api.load = function (payload) {
                        var currentRoutingProductId;

                        if (payload != undefined) {
                            currentRoutingProductId = payload.CurrentRoutingProductId;

                            ctrl.beginEffectiveDate = payload.CurrentBED;
                            currentBED = new Date(UtilsService.cloneObject(ctrl.beginEffectiveDate));
                            
                            ctrl.endEffectiveDate = payload.CurrentEED;
                            currentEED = new Date(UtilsService.cloneObject(ctrl.endEffectiveDate));
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
                            var currentSelectorId = currentSelectorAPI.getSelectedIds();
                            var newSelectorId = newSelectorAPI.getSelectedIds();

                            if (newSelectorId != null && currentSelectorId != newSelectorId) {
                                newRoutingProduct = {
                                    DefaultRoutingProductId: newSelectorId,
                                    BED: ctrl.beginEffectiveDate,
                                    EED: ctrl.endEffectiveDate
                                };
                            }

                            return newRoutingProduct;
                        }

                        function buildRoutingProductChange() {
                            var routingProductChange = null;
                            var currentSelectorId = currentSelectorAPI.getSelectedIds();
                            var newSelectorId = newSelectorAPI.getSelectedIds();

                            if (newSelectorId == null && !compareDates(ctrl.endEffectiveDate, currentEED)) {
                                routingProductChange = {
                                    DefaultRoutingProductId: currentSelectorId,
                                    EED: ctrl.endEffectiveDate
                                };
                            }

                            return routingProductChange;

                            function compareDates(date1, date2) {
                                if (!isEmpty(date1) && !isEmpty(date2))
                                    return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
                                else if (isEmpty(date1) && isEmpty(date2))
                                    return true;
                                else
                                    return false;
                            }
                        }
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);
