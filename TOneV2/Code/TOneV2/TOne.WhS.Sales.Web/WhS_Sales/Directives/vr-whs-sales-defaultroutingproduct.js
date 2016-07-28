"use strict";

app.directive("vrWhsSalesDefaultroutingproduct", ["WhS_BE_SalePriceListOwnerTypeEnum", "UtilsService", "VRUIUtilsService",
function (WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var defaultRoutingProduct = new DefaultRoutingProduct(ctrl, $scope);
            defaultRoutingProduct.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/DefaultRoutingProductTemplate.html"
    };

    function DefaultRoutingProduct(ctrl, $scope) {
        this.initializeController = initializeController;

        var defaultItem;
        var isFirstSelectionEvent;
        var isStateLoaded;

        var selectorAPI;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            ctrl.onSelectionChanged = function ()
            {
                var selectedId = selectorAPI.getSelectedIds();

                if (selectedId == undefined && isFirstSelectionEvent == true) {
                    isFirstSelectionEvent = false;
                    return;
                }

                if (isStateLoaded === false) {
                    isStateLoaded = true;
                    return;
                }

                defaultItem.IsDirty = true;
                defaultItem.onChange();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                isFirstSelectionEvent = true;
                isStateLoaded = undefined;

                if (payload != undefined) {
                    defaultItem = payload;
                    ctrl.CurrentName = defaultItem.IsCurrentRoutingProductEditable === false ? defaultItem.CurrentRoutingProductName + " (Inherited)" : defaultItem.CurrentRoutingProductName;
                    if (defaultItem.NewRoutingProductId != undefined)
                        isStateLoaded = false;
                }

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectedIds;
                if (defaultItem.NewRoutingProductId)
                    selectedIds = [defaultItem.NewRoutingProductId];
                else if (defaultItem.CurrentRoutingProductId && defaultItem.RoutingProductChangeEED)
                    selectedIds = [-1];

                var selectorPayload = {
                    filter: { ExcludedRoutingProductId: defaultItem.CurrentRoutingProductId, AssignableToOwnerType: defaultItem.OwnerType, AssignableToOwnerId: defaultItem.OwnerId },
                    selectedIds: selectedIds,
                    defaultItems: (defaultItem.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value && defaultItem.IsCurrentRoutingProductEditable) ?
                        [{ RoutingProductId: -1, Name: "(Reset To Default)" }] : null
                };
                
                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);

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
                            BED: UtilsService.getDateFromDateTime(new Date()),
                            EED: null
                        };
                    }
                }

                function setDefaultRoutingProductChange(defaultChanges) {
                    var selectedId = selectorAPI.getSelectedIds();

                    defaultChanges.DefaultRoutingProductChange = (selectedId && selectedId == -1) ? {
                        DefaultRoutingProductId: defaultItem.CurrentRoutingProductId,
                        EED: UtilsService.getDateFromDateTime(new Date())
                    } : null;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
