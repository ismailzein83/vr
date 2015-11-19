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
                var isCurrentDefaultRoutingProductEditable;
                var currentDefaultRoutingProductId;
                var newDefaultRoutingProductId;
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

                ctrl.onNewRoutingProductChanged = function () {
                    if (newSelectorAPI.getSelectedIds() != null) {
                        ctrl.disableBED = false;
                        ctrl.disableEED = false;
                    }
                    else {
                        ctrl.disableBED = true;
                        ctrl.disableEED = (!isCurrentDefaultRoutingProductEditable);
                    }
                };

                UtilsService.waitMultiplePromises(readyPromises).then(function () {
                    buildAPI();
                });

                function buildAPI() {
                    var api = {};

                    api.load = function (payload) {
                        if (payload != undefined) {
                            isCurrentDefaultRoutingProductEditable = payload.IsCurrentDefaultRoutingProductEditable;
                            currentDefaultRoutingProductId = payload.CurrentDefaultRoutingProductId;
                            newDefaultRoutingProductId = payload.NewDefaultRoutingProductId;
                            
                            ctrl.bed = new Date(payload.CurrentBED);
                            currentBED = new Date(payload.CurrentBED);
                            ctrl.disableBED = true;
                            
                            ctrl.eed = (payload.CurrentEED != null) ? new Date(payload.CurrentEED) : null;
                            currentEED = (payload.CurrentEED != null) ? new Date(payload.CurrentEED) : null;
                            ctrl.disableEED = !payload.IsCurrentDefaultRoutingProductEditable;
                        }

                        var currentSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var currentSelectorPayload = {
                            filter: null,
                            selectedIds: currentDefaultRoutingProductId
                        };

                        VRUIUtilsService.callDirectiveLoad(currentSelectorAPI, currentSelectorPayload, currentSelectorLoadPromiseDeferred);

                        return currentSelectorLoadPromiseDeferred.promise.then(function () {
                            var newSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                            newSelectorLoadPromiseDeferred.promise.then(function () {
                                if (newDefaultRoutingProductId != null)
                                    ctrl.selectedNewRoutingProduct = UtilsService.getItemByVal(ctrl.newRoutingProducts, newDefaultRoutingProductId, "RoutingProductId");
                            });

                            var newSelectorPayloadFilter = (currentDefaultRoutingProductId != undefined && currentDefaultRoutingProductId != null) ?
                                    { ExcludedRoutingProductId: currentDefaultRoutingProductId } : null;

                            var newSelectorPayload = {
                                filter: newSelectorPayloadFilter,
                                selectedIds: null
                            };

                            VRUIUtilsService.callDirectiveLoad(newSelectorAPI, newSelectorPayload, newSelectorLoadPromiseDeferred);

                            return newSelectorLoadPromiseDeferred.promise;
                        });
                    };

                    api.getDefaultChanges = function () {
                        var newDefaultRoutingProduct = buildNewDefaultRoutingProduct();
                        var defaultRoutingProductChange = (newDefaultRoutingProduct == null) ? buildDefaultRoutingProductChange() : null;

                        var defaultChanges = (newDefaultRoutingProduct != null || defaultRoutingProductChange != null) ? {
                            NewDefaultRoutingProduct: newDefaultRoutingProduct,
                            DefaultRoutingProductChange: defaultRoutingProductChange
                        } : null;

                        return defaultChanges;

                        function buildNewDefaultRoutingProduct() {
                            var newDefaultRoutingProduct = null;
                            var currentSelectorId = currentSelectorAPI.getSelectedIds();
                            var newSelectorId = newSelectorAPI.getSelectedIds();

                            if (newSelectorId != null && currentSelectorId != newSelectorId) {
                                newDefaultRoutingProduct = {
                                    $type: "TOne.WhS.Sales.Entities.RatePlanning.NewDefaultRoutingProduct, TOne.WhS.Sales.Entities",
                                    DefaultRoutingProductId: newSelectorId,
                                    BED: ctrl.bed,
                                    EED: ctrl.eed
                                };
                            }

                            return newDefaultRoutingProduct;
                        }

                        function buildDefaultRoutingProductChange() {
                            var defaultRoutingProductChange = null;
                            var currentSelectorId = currentSelectorAPI.getSelectedIds();
                            var newSelectorId = newSelectorAPI.getSelectedIds();

                            if (newSelectorId == null && !compareDates(ctrl.eed, currentEED)) {
                                defaultRoutingProductChange = {
                                    $type: "TOne.WhS.Sales.Entities.RatePlanning.DefaultRoutingProductChange, TOne.WhS.Sales.Entities",
                                    DefaultRoutingProductId: currentSelectorId,
                                    EED: ctrl.eed
                                };
                            }

                            return defaultRoutingProductChange;

                            function compareDates(date1, date2) {
                                if (!isEmpty(date1) && !isEmpty(date2))
                                    return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
                                else if (isEmpty(date1) && isEmpty(date2))
                                    return true;
                                else
                                    return false;
                            }

                            function isEmpty(value) {
                                return (value == undefined || value == null);
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
