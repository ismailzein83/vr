﻿'use strict';
app.directive('vrWhsBeSalezoneSelector', ['WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@'
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
                }
            },
            template: function (element, attrs) {
                return getBeSaleZoneTemplate(attrs);
            }

        };


        function getBeSaleZoneTemplate(attrs) {
            var label = "Sale Zone";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Sale Zones";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-show="ctrl.isSellingNumberPlanVisible">'
                   + ' <vr-whs-be-sellingnumberplan-selector on-ready="ctrl.onSellingNumberReady"  onselectionchanged="ctrl.onSellingNumberPlanSelectionchanged"></vr-whs-be-sellingnumberplan-selector>'
                   + ' </vr-columns>'
                   + ' <vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '  <vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.search"'
                   + '  datavaluefield="SaleZoneId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  vr-disabled="ctrl.isdisabled"'
                   + '  label="' + label + '"'
                   + '  entityName="' + label + '">'
                   + '</vr-select>'
                   + '</vr-columns>'
        }

        function saleZoneCtor(saleZoneSelectorCtrl, $scope, attrs) {
            var filter;
            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingDirectiveApi;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanId;
            var oldsellingNumberPlanId;


            function initializeController() {

                saleZoneSelectorCtrl.selectedvalues;

                if (attrs.ismultipleselection != undefined)
                    saleZoneSelectorCtrl.selectedvalues = [];


                saleZoneSelectorCtrl.onSellingNumberReady = function (api) {
                    sellingDirectiveApi = api;
                    sellingReadyPromiseDeferred.resolve();
                }


                saleZoneSelectorCtrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    selectorReadyPromiseDeferred.resolve();
                }


                saleZoneSelectorCtrl.onSellingNumberPlanSelectionchanged = function () {
                    selectorApi.clearDataSource();
                    var oldsellingNumberPlanId = sellingNumberPlanId;

                    sellingNumberPlanId = sellingDirectiveApi.getSelectedIds();
                    if (sellingNumberPlanId == undefined)
                        sellingNumberPlanId = oldsellingNumberPlanId;
                }


                saleZoneSelectorCtrl.search = function (nameFilter) {
                    if (sellingNumberPlanId == undefined)
                        return function () { };

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter);
                }


                UtilsService.waitMultiplePromises([sellingReadyPromiseDeferred.promise, selectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorApi.clearDataSource();
                    var selectedIds;
                    var payloadSellingNumberPlanId;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        payloadSellingNumberPlanId = payload.sellingNumberPlanId;
                    }

                    if (payloadSellingNumberPlanId != undefined) {

                        sellingNumberPlanId = payload.sellingNumberPlanId;

                        saleZoneSelectorCtrl.isSellingNumberPlanVisible = false;
                        if (selectedIds != undefined) {
                            var input = {
                                SaleZoneIds: selectedIds,
                                SellingNumberPlanId: payload.sellingNumberPlanId,
                                SaleZoneFilterSettings: { RoutingProductId: (filter != undefined && filter.SaleZoneFilterSettings != undefined) ? filter.SaleZoneFilterSettings.RoutingProductId : undefined }
                            };
                            return GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input);
                        }
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

                            return loadSellingNumberPlanSectionPromiseDeferred.promise;

                        }

                        else {
                            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                            sellingReadyPromiseDeferred.promise.then(function () {
                                var payloadDirective;
                                VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, payloadDirective, loadSellingNumberPlanPromiseDeferred);
                            });
                            return loadSellingNumberPlanPromiseDeferred.promise;
                        }
                    }
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, saleZoneSelectorCtrl);
                }

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