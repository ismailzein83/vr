"use strict";

app.directive("vrWhsSalesZoneroutingproduct", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var zoneRoutingProduct = new ZoneRoutingProduct(ctrl, $scope);
                zoneRoutingProduct.initCtrl();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/ZoneRoutingProductTemplate.html"
        };

        function ZoneRoutingProduct(ctrl, $scope) {
            this.initCtrl = initCtrl;

            function initCtrl() {
                var counter = 0;
                var isDirty = false;

                var currentId;
                var newId;
                
                var selectorAPI;
                var selectorReadyDeferred = UtilsService.createPromiseDeferred();

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyDeferred.resolve();
                };

                ctrl.onSelectionChange = function () {
                    counter++;
                    if ((newId == null && counter > 1) || (newId != null && counter > 2)) isDirty = true;
                };

                selectorReadyDeferred.promise.then(function () {
                    getAPI();
                });

                function getAPI() {
                    var api = {};

                    api.load = function (payload) {
                        if (payload != undefined) {
                            currentId = payload.CurrentRoutingProductId;
                            ctrl.CurrentName = payload.CurrentRoutingProductName;
                            newId = payload.NewRoutingProductId;
                        }

                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                        selectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: { ExcludedRoutingProductId: currentId },
                                selectedIds: newId
                            };

                            $scope.isLoading = true;
                            VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                            selectorLoadDeferred.promise.finally(function () {
                                $scope.isLoading = false;
                            });
                        });

                        return selectorLoadDeferred.promise;
                    };

                    api.getChanges = function () {
                        var changes = null;

                        if (isDirty) {
                            var changes = {
                                NewRoutingProduct: getNewRoutingProduct()
                            };
                        }

                        return changes;

                        function getNewRoutingProduct() {
                            var newRoutingProduct = null;
                            var selectedId = selectorAPI.getSelectedIds();

                            if (selectedId != null) {
                                newRoutingProduct = {
                                    RoutingProductId: selectedId,
                                    BED: new Date()
                                };
                            }

                            return newRoutingProduct;
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
