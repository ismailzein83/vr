"use strict";

app.directive("vrWhsSalesDefaultroutingproduct", ["WhS_Sales_SalePriceListOwnerTypeEnum", "UtilsService", "VRUIUtilsService",
function (WhS_Sales_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var defaultRoutingProduct = new DefaultRoutingProduct(ctrl, $scope);
            defaultRoutingProduct.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/DefaultRoutingProductTemplate.html"
    };

    function DefaultRoutingProduct(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var defaultItem;
        var counter = 0;
        var selectorAPI;
        var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initCtrl() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyPromiseDeferred.resolve();
            };

            ctrl.onSelectionChange = function (item) {
                counter++;
                var selectedId = selectorAPI.getSelectedIds;
                    
                if ((!defaultItem.NewRoutingProductId && counter > 1) || (defaultItem.NewRoutingProductId && counter > 2)) {
                    defaultItem.IsDirty = true;

                    if (defaultItem.onChange && typeof (defaultItem.onChange) == "function")
                        defaultItem.onChange();
                }
            };

            selectorReadyPromiseDeferred.promise.then(function () {
                getAPI();
            });
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload) {
                    defaultItem = payload;
                    ctrl.CurrentName = defaultItem.IsCurrentRoutingProductEditable === false ? defaultItem.CurrentRoutingProductName + " (Inherited)" : defaultItem.CurrentRoutingProductName;
                }

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectedIds;
                if (defaultItem.NewRoutingProductId)
                    selectedIds = [defaultItem.NewRoutingProductId];
                else if (defaultItem.NewRoutingProductEED)
                    selectedIds = [-1];

                var selectorPayload = {
                    filter: { ExcludedRoutingProductId: defaultItem.CurrentRoutingProductId, AssignableToOwnerType: defaultItem.OwnerType, AssignableToOwnerId: defaultItem.OwnerId },
                    selectedIds: selectedIds,
                    defaultItems: (defaultItem.OwnerType == WhS_Sales_SalePriceListOwnerTypeEnum.Customer.value && defaultItem.IsCurrentRoutingProductEditable) ?
                        [{ RoutingProductId: -1, Name: "(Reset To Default)" }] : null
                };
                
                $scope.isLoading = true;

                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);

                selectorLoadDeferred.promise.finally(function () {
                    $scope.isLoading = false;
                });

                return selectorLoadDeferred.promise;
            };

            api.applyChanges = function (defaultChanges) {
                setNewDefaultRoutingProduct(defaultChanges);
                setDefaultRoutingProductChange(defaultChanges);

                function setNewDefaultRoutingProduct(defaultChanges) {
                    var selectedId = selectorAPI.getSelectedIds();
                    
                    if (selectedId && selectedId != -1) {
                        defaultChanges.NewDefaultRoutingProduct = {
                            DefaultRoutingProductId: selectedId,
                            BED: new Date(),
                            EED: null
                        };
                    }
                }

                function setDefaultRoutingProductChange(defaultChanges) {
                    var selectedId = selectorAPI.getSelectedIds();

                    defaultChanges.DefaultRoutingProductChange = (selectedId && selectedId == -1) ? {
                        DefaultRoutingProductId: defaultItem.CurrentRoutingProductId,
                        EED: new Date()
                    } : null;
                }
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
