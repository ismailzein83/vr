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

        var zoneItem;
        var counter = 0;
        var currentId;
        var newId;
        var selectorAPI;
        var selectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initCtrl() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyDeferred.resolve();
            };

            ctrl.onSelectionChange = function () {
                counter++;

                if ((!newId && counter > 1) || (newId && counter > 2))
                    zoneItem.IsDirty = true;
            };

            selectorReadyDeferred.promise.then(function () {
                getAPI();
            });
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    zoneItem = payload;
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

            api.applyChanges = function (zoneItemChanges) {
                if (zoneItem.IsDirty) {
                    setNewRoutingProduct(zoneItemChanges);
                    setRoutingProductChange(zoneItemChanges);
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function setNewRoutingProduct(zoneItemChanges) {
            zoneItemChanges.NewRoutingProduct = null;
            var selectedId = selectorAPI.getSelectedIds();

            if (selectedId != null) {
                zoneItemChanges.NewRoutingProduct = {
                    ZoneId: zoneItemChanges.ZoneId,
                    ZoneRoutingProductId: selectedId,
                    BED: new Date(),
                    EED: null
                };
            }
        }

        function setRoutingProductChange(zoneItemChanges) {
            zoneItemChanges.RoutingProductChange = null;
        }
    }
}]);
