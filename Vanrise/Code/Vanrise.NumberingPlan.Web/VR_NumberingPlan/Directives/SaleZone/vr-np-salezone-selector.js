'use strict';

app.directive('vrNpSalezoneSelector', ['Vr_NP_SaleZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Vr_NP_SaleZoneAPIService, UtilsService, VRUIUtilsService) {

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
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.label = "Sale Zone";
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.label = "Sale Zones";
                }

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
            var multipleselection = "";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            return '<span  ng-show="ctrl.isSellingNumberPlanVisible">'
                   + ' <vr-np-sellingnumberplan-selector  normal-col-num="{{ctrl.normalColNum}}"   on-ready="ctrl.onSellingNumberReady"'
                   + ' onselectionchanged="ctrl.onSellingNumberPlanSelectionchanged"></vr-np-sellingnumberplan-selector>'
                   + ' </span>'
                   + ' <vr-columns colnum="{{ctrl.normalColNum}}">'
                   + ' <span vr-disabled="ctrl.isdisabled"> <vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  onblurdropdown="ctrl.onblurdropdown" '
                   + '  datasource="ctrl.search"'
                   + '  datavaluefield="SaleZoneId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isSaleZoneRequired()"'
                   + '  onselectitem="ctrl.onselectitem"'
                   + '  ondeselectitem="ctrl.ondeselectitem"'
                   + '  label="{{ctrl.label}}">'
                   + '</vr-select> </span>'
                   + '</vr-columns>';
        }

        function saleZoneCtor(saleZoneSelectorCtrl, $scope, attrs) {
            this.initializeController = initializeController;

            var filter;
            var availableZoneIds;
            var excludedZoneIds;
            var sellingNumberPlanId;
            var oldsellingNumberPlanId;

            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingDirectiveApi;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                saleZoneSelectorCtrl.onSellingNumberReady = function (api) {
                    sellingDirectiveApi = api;
                    sellingReadyPromiseDeferred.resolve();
                };

                saleZoneSelectorCtrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    selectorReadyPromiseDeferred.resolve();
                };

                saleZoneSelectorCtrl.onSellingNumberPlanSelectionchanged = function () {

                    selectorApi.clearDataSource();
                    var oldsellingNumberPlanId = sellingNumberPlanId;

                    sellingNumberPlanId = sellingDirectiveApi.getSelectedIds();
                    if (sellingNumberPlanId == undefined)
                        sellingNumberPlanId = oldsellingNumberPlanId;
                };

                saleZoneSelectorCtrl.search = function (nameFilter) {
                    if (sellingNumberPlanId == undefined)
                        return function () { };

                    var getEffectiveOnly = true;

                    if (filter != undefined) {
                        if (filter.GetEffectiveOnly == undefined) {
                            filter.GetEffectiveOnly = getEffectiveOnly;
                        }
                    }
                    else {
                        filter = { GetEffectiveOnly: getEffectiveOnly };
                    }

                    filter.availableZoneIds = availableZoneIds;
                    filter.excludedZoneIds = excludedZoneIds;

                    var serializedFilter = UtilsService.serializetoJson(filter);
                    return Vr_NP_SaleZoneAPIService.GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter);
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
                    var payloadSellingNumberPlanId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        payloadSellingNumberPlanId = payload.sellingNumberPlanId;
                        filter = payload.filter;
                        availableZoneIds = payload.availableZoneIds;
                        excludedZoneIds = payload.excludedZoneIds;

                        if (payload.customLabel != undefined) {
                            saleZoneSelectorCtrl.label = payload.customLabel;
                        }
                    }

                    if (payloadSellingNumberPlanId != undefined) {

                        sellingNumberPlanId = payload.sellingNumberPlanId;

                        saleZoneSelectorCtrl.isSellingNumberPlanVisible = false;
                        if (selectedIds != undefined) {
                            var input = {
                                SaleZoneIds: selectedIds,
                                SellingNumberPlanId: payload.sellingNumberPlanId
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


                                var loadSaleZonePromise = Vr_NP_SaleZoneAPIService.GetSellingNumberPlanIdBySaleZoneIds(selectedSaleZoneIds).then(function (response) {

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
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, saleZoneSelectorCtrl);
                };

                if (saleZoneSelectorCtrl.onReady != null)
                    saleZoneSelectorCtrl.onReady(api);

                return api;
            }
        }

        function GetSaleZonesInfo(attrs, saleZoneSelectorCtrl, selectedIds, input) {
            saleZoneSelectorCtrl.datasource = [];
            return Vr_NP_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                angular.forEach(response, function (item) {
                    saleZoneSelectorCtrl.datasource.push(item);
                });
                if (selectedIds != undefined)
                    VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, saleZoneSelectorCtrl);
            });
        }

        return directiveDefinitionObject;
    }]);