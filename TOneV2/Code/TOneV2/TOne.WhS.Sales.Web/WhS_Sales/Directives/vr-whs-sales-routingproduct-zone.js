"use strict";

app.directive("vrWhsSalesRoutingproductZone", ["UtilsService", "VRUIUtilsService", "VRDateTimeService", "WhS_Sales_RatePlanUtilsService",
    function (UtilsService, VRUIUtilsService, VRDateTimeService, WhS_Sales_RatePlanUtilsService) {
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

        function ZoneRoutingProduct(ctrl, $scope) {
            this.initializeController = initializeController;

            var zoneItem;

            var currentServiceViewerAPI;           
            var currentServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

            var selectorAPI;
            var selectorReadyDeferred = UtilsService.createPromiseDeferred();

            var firstSelectionEventDeferred = UtilsService.createPromiseDeferred();
            var rpSelectedDeferred;

            var isLoaded;

            function initializeController() {
                ctrl.onCurrentServiceViewerReady = function (api) {
                    currentServiceViewerAPI = api;
                    currentServiceViewerReadyDeferred.resolve();
                };

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyDeferred.resolve();
                };

                ctrl.onSelectionChanged = function () {

                    if (firstSelectionEventDeferred != undefined) {
                        firstSelectionEventDeferred = undefined;
                        return;
                    }

                    if (rpSelectedDeferred != undefined) {
                        rpSelectedDeferred = undefined;
                        return;
                    }
                    zoneItem.IsDirty = true;
                    zoneItem.refreshZoneItem(zoneItem);
                };

                ctrl.isSwitchVisible = function () {

                    if (!isLoaded) {
                        return false;
                    }

                    if (zoneItem != undefined && zoneItem.NewRate != null && selectorAPI.getSelectedIds() != undefined) {
                        return true;
                    }
                    else {
                        ctrl.followRateDate = false;
                        return false;
                    }
                };

                ctrl.initializeSwitchValue = function () {
                    if (ctrl.followRateDate == undefined)
                        ctrl.followRateDate = true;
                };

                UtilsService.waitMultiplePromises([currentServiceViewerReadyDeferred.promise, selectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    isLoaded = false;

                    var promises = [];
                    firstSelectionEventDeferred = UtilsService.createPromiseDeferred();

                    var selectedRoutingProductId;

                    if (payload != undefined) {

                        zoneItem = payload.zoneItem;

                        ctrl.isCountryEnded = zoneItem.IsCountryEnded;
                        ctrl.isZonePendingClosed = zoneItem.IsZonePendingClosed;

                        ctrl.currentName = zoneItem.CurrentRoutingProductName;
                        if (zoneItem.IsCurrentRoutingProductEditable === false)
                            ctrl.currentName += ' (Inherited)';

                        if (zoneItem.NewRoutingProduct != null) {
                            selectedRoutingProductId = zoneItem.NewRoutingProduct.ZoneRoutingProductId;
                            rpSelectedDeferred = UtilsService.createPromiseDeferred();
                            ctrl.followRateDate = zoneItem.NewRoutingProduct.ApplyNewNormalRateBED;
                        }
                        else if (zoneItem.ResetRoutingProduct != null) {
                            selectedRoutingProductId = -1;
                            rpSelectedDeferred = UtilsService.createPromiseDeferred();
                            ctrl.followRateDate = zoneItem.ResetRoutingProduct.ApplyNewNormalRateBED;
                        }
                    }

                    var loadCurrentServicesPromise = loadCurrentServices();
                    promises.push(loadCurrentServicesPromise);

                    var loadSelectorPromise = loadSelector(selectedRoutingProductId);
                    promises.push(loadSelectorPromise);

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        isLoaded = true;
                    });
                };

                api.applyChanges = function () {
                    setNewRoutingProduct();
                    setRoutingProductChange();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadCurrentServices() {
                var currentServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();

                var currentServiceViewerPayload = { selectedIds: zoneItem.CurrentServiceIds };
                VRUIUtilsService.callDirectiveLoad(currentServiceViewerAPI, currentServiceViewerPayload, currentServiceViewerLoadDeferred);

                return currentServiceViewerLoadDeferred.promise;
            }
            function loadSelector(selectedIds) {

                var selectorLoadDeferred = UtilsService.createPromiseDeferred();

                var selectorPayload = {
                    selectedIds: selectedIds
                };

                selectorPayload.filter = {
                    ExcludedRoutingProductId: zoneItem.CurrentRoutingProductId,
                    AssignableToZoneId: zoneItem.ZoneId
                };

                if (zoneItem.IsCurrentRoutingProductEditable === true) {
                    selectorPayload.defaultItems = [{
                        RoutingProductId: -1,
                        Name: '(Reset To Default)'
                    }];
                }

                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                return selectorLoadDeferred.promise;
            }

            function setNewRoutingProduct() {
                var selectedId = selectorAPI.getSelectedIds();

                if (selectedId && selectedId != -1) {
                    zoneItem.NewRoutingProduct = {
                        ZoneId: zoneItem.ZoneId,
                        ZoneRoutingProductId: selectedId,
                        BED: setNewRoutingProductBED(),
                        EED: null,
                        ApplyNewNormalRateBED: ctrl.followRateDate
                    };
                }
                else {
                    zoneItem.NewRoutingProduct = null;
                }
            }
            function setNewRoutingProductBED() {
                var zoneBED = UtilsService.createDateFromString(zoneItem.ZoneBED);
                var countryBED = (zoneItem.CountryBED != undefined) ? UtilsService.createDateFromString(zoneItem.CountryBED) : undefined;
                var today = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
                var dates = [];
                dates.push(zoneBED);
                dates.push(countryBED);
                dates.push(today);
                return WhS_Sales_RatePlanUtilsService.getMaxDate(dates);
            }
            function setRoutingProductChange() {
                var selectedId = selectorAPI.getSelectedIds();

                if (selectedId && selectedId == -1) {
                    zoneItem.ResetRoutingProduct = {
                        ZoneId: zoneItem.ZoneId,
                        ZoneRoutingProductId: zoneItem.CurrentRoutingProductId,
                        EED: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                        ApplyNewNormalRateBED: ctrl.followRateDate,
                    };
                }
                else {
                    zoneItem.ResetRoutingProduct = null;
                }
            }
        }
    }]);