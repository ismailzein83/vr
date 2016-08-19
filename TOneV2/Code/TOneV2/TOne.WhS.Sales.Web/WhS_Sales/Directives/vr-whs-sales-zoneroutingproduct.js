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

            api.applyChanges = function () {
                setNewRoutingProduct()
                setRoutingProductChange();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function setNewRoutingProduct()
        {
            var selectedId = selectorAPI.getSelectedIds();

            if (selectedId && selectedId != -1)
            {
                zoneItem.NewRoutingProductId = selectedId;
                zoneItem.NewRoutingProductBED = UtilsService.getDateFromDateTime(new Date());
                zoneItem.NewRoutingProductEED = null;
            }
            else {
                zoneItem.NewRoutingProductId = null;
                zoneItem.NewRoutingProductBED = null;
                zoneItem.NewRoutingProductEED = null;
            }
        }
        function setRoutingProductChange()
        {
            var selectedId = selectorAPI.getSelectedIds();
            zoneItem.RoutingProductChangeEED = (selectedId && selectedId == -1) ? UtilsService.getDateFromDateTime(new Date()) : null;
        }
    }
}]);
