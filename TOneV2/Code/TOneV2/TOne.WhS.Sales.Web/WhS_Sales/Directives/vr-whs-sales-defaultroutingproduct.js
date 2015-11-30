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

        var isDirty = false;
        var counter = 0;
        var currentId;
        var newId;
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
                    
                if ((!newId && counter > 1) || (newId && counter > 2))
                    isDirty = true;
            };

            selectorReadyPromiseDeferred.promise.then(function () {
                getAPI();
            });
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                isDirty = false;

                if (payload != undefined) {
                    currentId = payload.CurrentRoutingProductId;
                    ctrl.CurrentName = payload.CurrentRoutingProductName;
                    newId = payload.NewRoutingProductId;
                }

                //if (ctrl.IsEditable == null) ctrl.IsEditable = false;

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectorPayload = {
                    filter: { ExcludedRoutingProductId: currentId },
                    selectedIds: newId
                };

                $scope.isLoading = true;

                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);

                selectorLoadDeferred.promise.finally(function () {
                    $scope.isLoading = false;
                });

                return selectorLoadDeferred.promise;
            };

            api.applyChanges = function (changes) {
                changes.DefaultChanges = null;

                if (isDirty) {
                    changes.DefaultChanges = {
                        NewDefaultRoutingProduct: getNewDefaultRoutingProduct(),
                        DefaultRoutingProductChange: getDefaultRoutingProductChange()
                    };
                }

                function getNewDefaultRoutingProduct() {
                    var newRoutingProduct = null;
                    var selectedId = selectorAPI.getSelectedIds();

                    if (selectedId != null) {
                        newRoutingProduct = {
                            DefaultRoutingProductId: selectedId,
                            BED: new Date(),
                            EED: null
                        };
                    }

                    return newRoutingProduct;
                }

                function getDefaultRoutingProductChange() {
                    return null;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
