"use strict";

app.directive("vrWhsSalesRateplanroutingproduct", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var constructor = new Constructor(ctrl, $scope);
                constructor.initCtrl();

            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RatePlanRoutingProductTemplate.html"
        };

        function Constructor(ctrl, $scope) {
            this.initCtrl = initCtrl;

            function initCtrl() {
                var selectorAPI;
                var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var loadHasBeenCalled = false;
                var isEditable;

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyPromiseDeferred.resolve();
                };

                ctrl.onSelectorChanged = function () {
                    if (loadHasBeenCalled) {
                        if (selectorAPI.getSelectedIds() != null) {
                            ctrl.showNewBED = true;
                            ctrl.showNewEED = true;
                        }
                        else {
                            ctrl.showNewBED = false;
                            ctrl.showNewEED = (isEditable);
                        }
                    }
                };

                selectorReadyPromiseDeferred.promise.then(function () {
                    getAPI();
                });

                function getAPI() {
                    var api = {};

                    api.load = function (payload) {
                        loadHasBeenCalled = true;
                        var currentId;
                        var newId;

                        if (payload != undefined) {
                            console.log(payload);
                            currentId = payload.CurrentRoutingProductId;
                            isEditable = payload.IsCurrentRoutingProductEditable;
                            ctrl.currentName = payload.CurrentRoutingProductName;
                            newId = payload.NewRoutingProductId;
                            ctrl.currentBED = payload.CurrentRoutingProductBED;
                            ctrl.newBED = payload.CurrentRoutingProductBED;
                            ctrl.currentEED = payload.CurrentRoutingProductEED;
                            ctrl.newEED = payload.CurrentRoutingProductEED;
                        }

                        var selectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var selectorPayload = {
                            filter: { ExcludedRoutingProductId: currentId },
                            selectedIds: newId
                        };

                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadPromiseDeferred);

                        return selectorLoadPromiseDeferred.promise;
                    };

                    api.getChanges = function () {
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
                            var newSelectorId = selectorAPI.getSelectedIds();

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
                            var newSelectorId = selectorAPI.getSelectedIds();

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
        }
    }]);
