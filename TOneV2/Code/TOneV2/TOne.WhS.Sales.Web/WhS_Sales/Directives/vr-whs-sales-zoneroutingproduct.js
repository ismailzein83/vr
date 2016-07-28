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
            zoneRoutingProduct.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/ZoneRoutingProductTemplate.html"
    };

    function ZoneRoutingProduct(ctrl, $scope)
    {
        this.initializeController = initializeController;

        var zoneItem;
        var isFirstSelectionEvent;
        var isStateLoaded;

        var selectorAPI;

        function initializeController()
        {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            ctrl.onSelectionChanged = function ()
            {
                var selectedId = selectorAPI.getSelectedIds();

                if (selectedId == undefined && isFirstSelectionEvent) {
                    isFirstSelectionEvent = false;
                    return;
                }

                if (isStateLoaded === false) {
                    isStateLoaded = true;
                    return;
                }

                zoneItem.IsDirty = true;
                zoneItem.refreshZoneItem(zoneItem);
            };
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload)
            {
                isFirstSelectionEvent = true;
                isStateLoaded = undefined;

                var selectedIds;

                if (payload != undefined)
                {
                    zoneItem = payload;
                    ctrl.CurrentName = zoneItem.IsCurrentRoutingProductEditable === false ? zoneItem.CurrentRoutingProductName + " (Inherited)" : zoneItem.CurrentRoutingProductName;

                    if (zoneItem.NewRoutingProductId != undefined) {
                        isStateLoaded = false;
                        selectedIds = zoneItem.NewRoutingProductId;
                    }
                    else if (zoneItem.CurrentRoutingProductId != undefined && zoneItem.RoutingProductChangeEED != undefined)
                        selectedIds = -1;
                }

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectorPayload = {
                    filter: { ExcludedRoutingProductId: zoneItem.CurrentRoutingProductId, AssignableToZoneId: zoneItem.ZoneId },
                    selectedIds: selectedIds,
                    defaultItems: (zoneItem.IsCurrentRoutingProductEditable === true) ? [{ RoutingProductId: -1, Name: "(Reset To Default)" }] : null
                };

                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);

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

            if (selectedId && selectedId != -1) {
                zoneItemChanges.NewRoutingProduct = {
                    ZoneId: zoneItemChanges.ZoneId,
                    ZoneRoutingProductId: selectedId,
                    BED: UtilsService.getDateFromDateTime(new Date()),
                    EED: null
                };
            }
        }
        function setRoutingProductChange(zoneItemChanges) {
            var selectedId = selectorAPI.getSelectedIds();

            zoneItemChanges.RoutingProductChange = (selectedId && selectedId == -1) ? {
                ZoneId: zoneItem.ZoneId,
                ZoneRoutingProductId: zoneItem.CurrentRoutingProductId,
                EED: UtilsService.getDateFromDateTime(new Date())
            } : null;
        }
    }
}]);
