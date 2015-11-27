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

                var ratePlanRoutingProduct = new RatePlanRoutingProduct(ctrl, $scope);
                ratePlanRoutingProduct.initCtrl();

            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/DefaultRoutingProductTemplate.html"
        };

        function RatePlanRoutingProduct(ctrl, $scope) {
            this.initCtrl = initCtrl;

            function initCtrl() {
                var counter = 0;
                var isDirty = false;
                var currentId;
                var selectorAPI;
                var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                ctrl.newRoutingProduct = undefined;

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyPromiseDeferred.resolve();
                };

                ctrl.onSelectionChange = function (item) {
                    counter++;
                    
                    if (counter > 1)
                        isDirty = true;

                    if (item != undefined) {
                        ctrl.ShowNewBED = true;
                        ctrl.ShowNewEED = true;
                    }
                    else {
                        ctrl.ShowNewBED = false;
                        ctrl.ShowNewEED = (ctrl.IsEditable);
                    }
                };

                ctrl.onNewEEDChange = function () {
                    isDirty = true;
                };

                selectorReadyPromiseDeferred.promise.then(function () {
                    getAPI();
                });

                function getAPI() {
                    var api = {};

                    api.load = function (payload) {
                        var newId;
                        
                        if (payload != undefined) {
                            currentId = payload.CurrentRoutingProductId;
                            ctrl.IsEditable = payload.IsCurrentRoutingProductEditable;
                            ctrl.CurrentName = payload.CurrentRoutingProductName;
                            ctrl.CurrentBED = (payload.CurrentRoutingProductBED != null) ? new Date(payload.CurrentRoutingProductBED) : null;
                            ctrl.CurrentEED = (payload.CurrentRoutingProductEED != null) ? new Date(payload.CurrentRoutingProductEED) : null;
                            newId = payload.NewRoutingProductId;
                            ctrl.NewBED = (payload.NewRoutingProductBED != null) ? new Date(payload.NewRoutingProductBED) : null;
                            ctrl.NewEED = (payload.NewRoutingProductEED != null) ? new Date(payload.NewRoutingProductEED) : null;
                        }

                        if (ctrl.IsEditable == null)
                            ctrl.IsEditable = false;

                        if (newId != undefined) {
                            ctrl.ShowNewBED = true;
                            ctrl.ShowNewEED = true;
                        }
                        else if (ctrl.IsEditable) {
                            ctrl.ShowNewEED = true;
                        }

                        var selectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var selectorPayload = {
                            filter: { ExcludedRoutingProductId: currentId },
                            selectedIds: newId
                        };

                        $scope.isLoadingDirective = true;

                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadPromiseDeferred);

                        selectorLoadPromiseDeferred.promise.finally(function () {
                            $scope.isLoadingDirective = false;
                        });

                        return selectorLoadPromiseDeferred.promise;
                    };

                    api.getChanges = function () {
                        var changes = null;

                        if (isDirty) {
                            var newRoutingProduct = getNewRoutingProduct();
                            var routingProductChange = getRoutingProductChange();

                            var changes = {
                                NewRoutingProduct: newRoutingProduct,
                                RoutingProductChange: routingProductChange
                            };
                        }

                        return changes;

                        function getNewRoutingProduct() {
                            var newRoutingProduct = null;
                            var selectorId = selectorAPI.getSelectedIds();

                            if (selectorId != null) {
                                newRoutingProduct = {
                                    RoutingProductId: selectorId,
                                    BED: ctrl.NewBED,
                                    EED: ctrl.NewEED
                                };
                            }

                            return newRoutingProduct;
                        }

                        function getRoutingProductChange() {
                            var routingProductChange = null;

                            if (selectorAPI.getSelectedIds() == null && !compareDates(ctrl.CurrentEED, ctrl.NewEED))
                            {
                                routingProductChange = {
                                    RoutingProductId: currentId,
                                    EED: ctrl.NewEED
                                };
                            }

                            return routingProductChange;

                            function compareDates(date1, date2) {
                                if (date1 != null && date1.getTime != undefined && date2 != null && date2.getTime != undefined)
                                    return (date1.getTime() != date2.getTime());
                                else if (date1 == null && date2 == null)
                                    return true;
                                else
                                    return false;
                            }
                        }
                    };

                    api.isDirty = function () {
                        return isDirty;
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }
        }
    }]);
