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
                    ctrl.CurrentName = (!defaultItem.IsCurrentRoutingProductEditable) ? defaultItem.CurrentRoutingProductName + " (Inherited)" : defaultItem.CurrentRoutingProductName;
                    //ctrl.IsEditable = defaultItem.IsCurrentRoutingProductEditable == null ? false : defaultItem.IsCurrentRoutingProductEditable;
                }

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectorPayload = {
                    filter: { ExcludedRoutingProductId: defaultItem.CurrentRoutingProductId, AssignableToOwnerType: defaultItem.OwnerType, AssignableToOwnerId: defaultItem.OwnerId },
                    selectedIds: defaultItem.NewRoutingProductId
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
                    
                    if (selectedId != null) {
                        defaultChanges.NewDefaultRoutingProduct = {
                            DefaultRoutingProductId: selectedId,
                            BED: new Date(),
                            EED: null
                        };
                    }
                }

                function setDefaultRoutingProductChange(defaultChanges) {
                    defaultChanges.DefaultRoutingProductChange = null;
                }
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
