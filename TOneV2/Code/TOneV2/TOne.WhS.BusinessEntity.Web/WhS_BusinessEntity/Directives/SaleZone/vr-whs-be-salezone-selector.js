﻿'use strict';
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

            return '<span  ng-show="ctrl.isSellingNumberPlanVisible">'
                   + ' <vr-whs-be-sellingnumberplan-selector  normal-col-num="{{ctrl.normalColNum}}" on-ready="ctrl.onSellingNumberReady" isrequired="ctrl.isrequired && ctrl.isSellingNumberPlanVisible"'
                   + ' onselectionchanged="ctrl.onSellingNumberPlanSelectionchanged"></vr-whs-be-sellingnumberplan-selector>'
                   + ' </span>'
                   + ' <vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<span vr-disabled="ctrl.isdisabled"><vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  limitcharactercount="ctrl.limitcharactercount"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  onblurdropdown="ctrl.onblurdropdown" '
                   + '  datasource="ctrl.search"'
                   + '  datavaluefield="SaleZoneId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isSaleZoneRequired()"'
                   + '  onselectitem="ctrl.onselectitem"'
                   + '  ondeselectitem="ctrl.ondeselectitem"'
                   + '  ' + label
                   + '  >'
                   + '</vr-select></span>'
                   + '</vr-columns>';
        }

        function saleZoneCtor(saleZoneSelectorCtrl, $scope, attrs) {

            var filter;
            var availableZoneIds;
            var excludedZoneIds;

            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingDirectiveApi;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanId;
            var oldsellingNumberPlanId;

            var availableSaleZones = [];

            var payloadSellingNumberPlanId;

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


                saleZoneSelectorCtrl.onSellingNumberPlanSelectionchanged = function () {
                    if (selectorApi != undefined) {
                        selectorApi.clearDataSource();
                        var oldsellingNumberPlanId = sellingNumberPlanId;

                        sellingNumberPlanId = sellingDirectiveApi.getSelectedIds();
                        if (sellingNumberPlanId == undefined)
                            sellingNumberPlanId = oldsellingNumberPlanId;
                    }
                };


                saleZoneSelectorCtrl.search = function (nameFilter) {                
                    if (sellingNumberPlanId == undefined)
                        return function () { };

                    if (sellingDirectiveApi != undefined && sellingDirectiveApi.getSelectedIds() == undefined && payloadSellingNumberPlanId==undefined) {
                          return function () {  };
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
                        return sellingDirectiveApi.getSelectedIds() != undefined;
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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        payloadSellingNumberPlanId = payload.sellingNumberPlanId;
                        filter = payload.filter;
                        availableZoneIds = payload.availableZoneIds;
                        excludedZoneIds = payload.excludedZoneIds;
                    }

                    if (payloadSellingNumberPlanId != undefined) {

                        var promises = [];


                        sellingNumberPlanId = payload.sellingNumberPlanId;

                        saleZoneSelectorCtrl.isSellingNumberPlanVisible = false;
                        if (selectedIds != undefined) {
                            var input = {
                                SaleZoneIds: selectedIds,
                                SellingNumberPlanId: payload.sellingNumberPlanId,
                                SaleZoneFilterSettings: {
                                    RoutingProductId: (filter != undefined && filter.SaleZoneFilterSettings != undefined) ? filter.SaleZoneFilterSettings.RoutingProductId : undefined
                                }
                            };
                            promises.push(GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input));
                        }

                        if (filter != undefined && filter.CountryIds != undefined && filter.CountryIds.length == 1) {
                            saleZoneSelectorCtrl.limitcharactercount = 0;
                            promises.push(selectorApi.loadDataSource("").then(function (res) {
                            }));
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }

                    else {
                        saleZoneSelectorCtrl.isSellingNumberPlanVisible = true;

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

                                    var payloadDirective = {
                                        selectedIds: selectedSellingNumberPlanIds
                                    };

                                    VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, payloadDirective, loadSellingNumberPlanPromiseDeferred);

                                    loadSellingNumberPlanPromiseDeferred.promise.then(function () {
                                        var input = {
                                            SaleZoneIds: selectedSaleZoneIds,
                                            SaleZoneFilterSettings: { RoutingProductId: undefined }
                                        };

                                        GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input).then(function () {
                                            setSelectedSaleZonesPromiseDeferred.resolve();
                                        });

                                    })


                                });

                                promises.push(loadSaleZonePromise);

                                UtilsService.waitMultiplePromises(promises).then(function () {
                                    loadSellingNumberPlanSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadSellingNumberPlanSectionPromiseDeferred.reject(error);
                                });

                            });

                            return loadSellingNumberPlanSectionPromiseDeferred.promise

                        }

                        else {
                            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                            sellingReadyPromiseDeferred.promise.then(function () {
                                var payloadDirective;
                                VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, payloadDirective, loadSellingNumberPlanPromiseDeferred);
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

                if (saleZoneSelectorCtrl.onReady != null)
                    saleZoneSelectorCtrl.onReady(api);

                return api;
            }

            this.initializeController = initializeController;
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