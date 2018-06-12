'use strict';

app.directive('vrWhsBeSalezoneSelector', ['WhS_BE_SaleZoneAPIService', 'VRCommon_EntityFilterEffectiveModeEnum', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, VRCommon_EntityFilterEffectiveModeEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                onblurdropdown: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                normalColNum: '@',
                hidelabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new saleZoneCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, saleZoneSelectorCtrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getBeSaleZoneTemplate(attrs);
            }
        };

        function getBeSaleZoneTemplate(attrs) {

            var label;
            if (attrs.hidelabel == undefined)
                label = 'label="Sale Zone"';

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                if (attrs.hidelabel == undefined)
                    label = 'label="Sale Zones"';

                multipleselection = "ismultipleselection";
            }

            return '<span ng-show="ctrl.isSellingNumberPlanVisible" vr-disabled="ctrl.isSellingNumberPlanDisabled">' +
                       ' <vr-whs-be-sellingnumberplan-selector on-ready="ctrl.onSellingNumberReady" isrequired="ctrl.isrequired && ctrl.isSellingNumberPlanVisible" normal-col-num="{{ctrl.normalColNum}}"' +
                            ' onselectionchanged="ctrl.onSellingNumberPlanSelectionchanged" selectedvalues="ctrl.selectedSellingNumberPlan" onselectitem="ctrl.onselectSNP" ondeselectitem="ctrl.ondeselectSNP">' +
                       '</vr-whs-be-sellingnumberplan-selector>' +
                   '</span>' +
                   '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                     '<span vr-disabled="ctrl.isdisabled"> ' +
                        '<vr-select on-ready="ctrl.onSelectorReady"' +
                           '  onselectionchanged="ctrl.onselectionchanged"' +
                           '  onblurdropdown="ctrl.onblurdropdown" ' +
                           '  selectedvalues="ctrl.selectedvalues"' +
                           '  datasource="ctrl.search"' +
                           '  datavaluefield="SaleZoneId"' +
                           '  datatextfield="Name"' + multipleselection + '  ' + label +
                           '  limitcharactercount="ctrl.limitcharactercount"' +
                           '  isrequired="ctrl.isSaleZoneRequired()"' +
                           '  onselectitem="ctrl.onselectitem"' +
                           '  ondeselectitem="ctrl.ondeselectitem">' +
                        '</vr-select>' +
                     '</span>' +
                   '</vr-columns>';
        }

        function saleZoneCtor(saleZoneSelectorCtrl, $scope, attrs) {
            this.initializeController = initializeController;

            var availableSaleZones = [];
            var areSaleZonesRequired = true;

            var filter;
            var availableZoneIds;
            var excludedZoneIds;

            var sellingNumberPlanId;
            var oldSellingNumberPlanId;

            var payloadSellingNumberPlanId;
            var genericUIContext;

            var sellingDirectiveApi;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {

                saleZoneSelectorCtrl.selectedvalues;
                if (attrs.ismultipleselection != undefined)
                    saleZoneSelectorCtrl.selectedvalues = [];

                saleZoneSelectorCtrl.onSellingNumberReady = function (api) {
                    sellingDirectiveApi = api;
                    sellingReadyPromiseDeferred.resolve();
                };

                saleZoneSelectorCtrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    selectorReadyPromiseDeferred.resolve();
                };

                saleZoneSelectorCtrl.onSellingNumberPlanSelectionchanged = function (selectedItem) {

                    if (selectedItem != undefined && selectorApi != undefined) {
                        selectorApi.clearDataSource();
                        sellingNumberPlanId = selectedItem.SellingNumberPlanId;
                    }
                };

                saleZoneSelectorCtrl.ondeselectSNP = function (deselectedItem) {
                    selectorApi.clearDataSource();
                    sellingNumberPlanId = undefined;
                };

                saleZoneSelectorCtrl.search = function (nameFilter) {
                    if (sellingNumberPlanId == undefined)
                        return function () { };

                    if (sellingDirectiveApi != undefined && sellingDirectiveApi.getSelectedIds() == undefined && payloadSellingNumberPlanId == undefined) {
                        return function () { };
                    }

                    if (filter == undefined)
                        filter = {};

                    if (filter.EffectiveMode == undefined)
                        filter.EffectiveMode = VRCommon_EntityFilterEffectiveModeEnum.Current.value;

                    filter.availableZoneIds = availableZoneIds;
                    filter.excludedZoneIds = excludedZoneIds;

                    var serializedFilter = UtilsService.serializetoJson(filter);
                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter).then(function (response) {
                        availableSaleZones.length = 0;
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++)
                                availableSaleZones.push(response[i]);
                        }
                        return response;
                    });
                };

                saleZoneSelectorCtrl.isSaleZoneRequired = function () {
                    if (saleZoneSelectorCtrl.isSellingNumberPlanVisible)
                        return sellingDirectiveApi.getSelectedIds() != undefined && areSaleZonesRequired;
                    else
                        return saleZoneSelectorCtrl.isrequired;
                };

                UtilsService.waitMultiplePromises([sellingReadyPromiseDeferred.promise, selectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();

                    var selectedIds;
                    var showSellingNumberPlanIfMultiple;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        payloadSellingNumberPlanId = payload.sellingNumberPlanId;
                        filter = payload.filter;
                        availableZoneIds = payload.availableZoneIds;
                        excludedZoneIds = payload.excludedZoneIds;
                        genericUIContext = payload.genericUIContext;
                        showSellingNumberPlanIfMultiple = payload.showSellingNumberPlanIfMultiple;
                    }

                    if (payloadSellingNumberPlanId != undefined) {
                        var loadSaleZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        loadSellingNumberPlanSelector(payloadSellingNumberPlanId).then(function () {
                            var _promises = [];

                            if (selectedIds != undefined) {
                                var input = {
                                    SaleZoneIds: selectedIds,
                                    SellingNumberPlanId: payloadSellingNumberPlanId,
                                    SaleZoneFilterSettings: {
                                        RoutingProductId: (filter != undefined && filter.SaleZoneFilterSettings != undefined) ? filter.SaleZoneFilterSettings.RoutingProductId : undefined
                                    }
                                };
                                _promises.push(GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input));
                            }

                            if (filter != undefined && filter.CountryIds != undefined && filter.CountryIds.length == 1) {
                                saleZoneSelectorCtrl.limitcharactercount = 0;
                                _promises.push(selectorApi.loadDataSource("").then(function (res) {
                                }));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadSaleZoneSelectorPromiseDeferred.resolve();
                            }).catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                                loadSaleZoneSelectorPromiseDeferred.reject(error);
                            });
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            loadSaleZoneSelectorPromiseDeferred.reject(error);
                        });

                        return loadSaleZoneSelectorPromiseDeferred.promise.then(function () {
                            saleZoneSelectorCtrl.isSellingNumberPlanVisible = (!showSellingNumberPlanIfMultiple || sellingDirectiveApi.hasSingleItem()) ? false : true;
                        });
                    }
                    else {
                        if (genericUIContext != undefined && genericUIContext.getFields != undefined && typeof (genericUIContext.getFields) == "function") {
                            var fields = genericUIContext.getFields();
                            if (fields != undefined) {
                                for (var x = 0; x < fields.length; x++) {
                                    var currentField = fields[x];
                                    if (currentField != undefined && currentField.FieldType != undefined && currentField.FieldType.BusinessEntityDefinitionId != undefined
                                        && currentField.FieldType.BusinessEntityDefinitionId.toUpperCase() == "BA5A57BD-1F03-440F-A469-463A48762B8F") {

                                        genericUIContext.onvaluechanged = function (field, selectedValue) {
                                            if (field.FieldType.BusinessEntityDefinitionId.toUpperCase() == "BA5A57BD-1F03-440F-A469-463A48762B8F") {
                                                areSaleZonesRequired = true;

                                                if (selectedValue == undefined)
                                                    return;

                                                if (selectedValue instanceof Array) {
                                                    switch (selectedValue.length) {
                                                        case 0:
                                                            saleZoneSelectorCtrl.isSellingNumberPlanDisabled = false;
                                                            saleZoneSelectorCtrl.isdisabled = false;
                                                            break;

                                                        default:
                                                            areSaleZonesRequired = false;
                                                            saleZoneSelectorCtrl.isSellingNumberPlanDisabled = true;
                                                            var hasDifferentSNPs = false;

                                                            var newSellingNumberPlanId = selectedValue[0].SellingNumberPlanId;

                                                            if (selectedValue.length > 1) {
                                                                for (var x = 1; x < selectedValue.length; x++) {
                                                                    var currentValue = selectedValue[x];
                                                                    if (currentValue.SellingNumberPlanId != newSellingNumberPlanId) {
                                                                        hasDifferentSNPs = true;
                                                                    }
                                                                }
                                                            }

                                                            if (hasDifferentSNPs) {
                                                                saleZoneSelectorCtrl.isdisabled = true;
                                                                saleZoneSelectorCtrl.selectedSellingNumberPlan = undefined;
                                                                selectorApi.clearDataSource();
                                                            }
                                                            else {
                                                                saleZoneSelectorCtrl.isdisabled = false;
                                                                var selectedSellingNumberPlan = saleZoneSelectorCtrl.selectedSellingNumberPlan;
                                                                if (selectedSellingNumberPlan != undefined) {

                                                                    if (selectedSellingNumberPlan.SellingNumberPlanId == newSellingNumberPlanId) {

                                                                    } else {
                                                                        sellingDirectiveApi.setSelectedIds(newSellingNumberPlanId);
                                                                        selectorApi.clearDataSource();
                                                                    }
                                                                }
                                                                else {
                                                                    sellingDirectiveApi.setSelectedIds(newSellingNumberPlanId);
                                                                    selectorApi.clearDataSource();
                                                                }
                                                            }

                                                            break;
                                                    }
                                                }
                                            }
                                        };

                                        break;
                                    }
                                }
                            }
                        }

                        if (selectedIds != undefined) {

                            var selectedSaleZoneIds = [];
                            if (attrs.ismultipleselection != undefined)
                                selectedSaleZoneIds = selectedIds;
                            else
                                selectedSaleZoneIds.push(selectedIds);

                            var loadSellingNumberPlanSectionPromiseDeferred = UtilsService.createPromiseDeferred();

                            sellingReadyPromiseDeferred.promise.then(function () {

                                var promises = [];

                                var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(loadSellingNumberPlanPromiseDeferred.promise);

                                var setSelectedSaleZonesPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(setSelectedSaleZonesPromiseDeferred.promise);

                                var loadSaleZonePromise = WhS_BE_SaleZoneAPIService.GetSellingNumberPlanIdBySaleZoneIds(selectedSaleZoneIds).then(function (response) {

                                    var selectedSellingNumberPlanIds = [];
                                    for (var i = 0 ; i < response.length; i++) {
                                        if (selectedSellingNumberPlanIds.indexOf(response[i].SellingNumberPlanId) < 0)
                                            selectedSellingNumberPlanIds.push(response[i].SellingNumberPlanId);
                                    }

                                    var sellingDirectivePayload = {
                                        selectedIds: selectedSellingNumberPlanIds,
                                        selectifsingleitem: true
                                    };
                                    VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, sellingDirectivePayload, loadSellingNumberPlanPromiseDeferred);

                                    loadSellingNumberPlanPromiseDeferred.promise.then(function () {
                                        var input = {
                                            SaleZoneIds: selectedSaleZoneIds,
                                            SaleZoneFilterSettings: { RoutingProductId: undefined }
                                        };

                                        GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input).then(function () {
                                            setSelectedSaleZonesPromiseDeferred.resolve();
                                        });
                                    });
                                });
                                promises.push(loadSaleZonePromise);

                                UtilsService.waitMultiplePromises(promises).then(function () {
                                    loadSellingNumberPlanSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadSellingNumberPlanSectionPromiseDeferred.reject(error);
                                });
                            });

                            return loadSellingNumberPlanSectionPromiseDeferred.promise.then(function () {
                                saleZoneSelectorCtrl.isSellingNumberPlanVisible = sellingDirectiveApi.hasSingleItem() ? false : true;
                            });
                        }
                        else {
                            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

                            sellingReadyPromiseDeferred.promise.then(function () {
                                var sellingDirectivePayload = { selectifsingleitem: true };
                                VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, sellingDirectivePayload, loadSellingNumberPlanPromiseDeferred);
                            });

                            return loadSellingNumberPlanPromiseDeferred.promise.then(function () {
                                saleZoneSelectorCtrl.isSellingNumberPlanVisible = sellingDirectiveApi.hasSingleItem() ? false : true;
                            });
                        }
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, saleZoneSelectorCtrl);
                };

                api.getAvailableSaleZones = function () {
                    if (availableSaleZones.length > 0) {
                        var availableZones = [];
                        for (var i = 0; i < availableSaleZones.length; i++)
                            availableZones.push(availableSaleZones[i]);
                        return availableZones;
                    }
                };

                api.getSellingNumberPlanId = function () {
                    return sellingNumberPlanId;
                };

                if (saleZoneSelectorCtrl.onReady != null)
                    saleZoneSelectorCtrl.onReady(api);

                return api;
            }

            function loadSellingNumberPlanSelector(sellingNumberPlanId) {
                var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

                sellingReadyPromiseDeferred.promise.then(function () {
                    var sellingDirectivePayload = {
                        selectedIds: sellingNumberPlanId,
                        selectifsingleitem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, sellingDirectivePayload, loadSellingNumberPlanPromiseDeferred);
                });

                return loadSellingNumberPlanPromiseDeferred.promise;
            }
        }

        function GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input) {

            saleZoneSelectorCtrl.datasource = [];
            return WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                angular.forEach(response, function (item) {
                    saleZoneSelectorCtrl.datasource.push(item);
                });
                if (selectedIds != undefined)
                    VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, saleZoneSelectorCtrl);
            });
        }

        return directiveDefinitionObject;
    }]);