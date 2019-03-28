'use strict';

app.directive('vrWhsBeSalezonegroupMatchingsupplierdeals', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                sellingnumberplanid: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new saleZoneMatchingSupplierDealsDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SaleZoneMatchingSupplierDealsTemplate.html';
            }
        };
         
        function saleZoneMatchingSupplierDealsDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var saleZoneGroupSettings;

            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dealDefinitionSelectionChangedPromiseDeferred;

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showSaleZoneSelector = false;

                $scope.scopeModel.onDealDefinitionSelectorReady = function (api) {
                    dealDefinitionSelectorAPI = api;
                    dealDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDealDefinitionSelectionChanged = function (selectedDeal) {
                    if (selectedDeal == undefined || selectedDeal.length == 0) {
                        $scope.scopeModel.showSaleZoneSelector = false;
                        return;
                    }

                    $scope.scopeModel.showSaleZoneSelector = true;

                    saleZoneSelectorReadyDeferred.promise.then(function () {

                        var saleZoneSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                                    SupplierDealIds: dealDefinitionSelectorAPI.getSelectedIds()
                                }]
                            },
                            showSellingNumberPlanIfMultiple: true
                        };

                        var setLoader = function (value) {
                            $scope.scopeModel.isSaleZoneSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorAPI, saleZoneSelectorPayload, setLoader, dealDefinitionSelectionChangedPromiseDeferred);
                    });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var sellingNumberPlanId;

                    if (payload != undefined) {
                        sellingNumberPlanId = payload.sellingNumberPlanId;
                        saleZoneGroupSettings = payload.saleZoneGroupSettings;
                    }

                    var promises = [];

                    var loadDealDefinitionSelectorPromise = loadDealDefinitionSelector();
                    promises.push(loadDealDefinitionSelectorPromise);

                    if (saleZoneGroupSettings != undefined) {
                        dealDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                        var loadSaleZoneSelectorPromise = loadSaleZoneSelector(sellingNumberPlanId, saleZoneGroupSettings.ZoneIds);
                        promises.push(loadSaleZoneSelectorPromise);
                    }

                    function loadDealDefinitionSelector() {
                        var loadDealDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dealDefinitionSelectorReadyDeferred.promise.then(function () {

                            var dealDefinitionPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.Deal.MainExtensions.DealDefinitionFilter.CostDealFilter,TOne.WhS.Deal.MainExtensions"
                                    }]
                                },
                                selectedIds: saleZoneGroupSettings != undefined ? saleZoneGroupSettings.SupplierDealIds : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionPayload, loadDealDefinitionSelectorPromiseDeferred);
                        });

                        return loadDealDefinitionSelectorPromiseDeferred.promise;
                    }

                    function loadSaleZoneSelector(sellingNumberPlanId, saleZoneIds) {
                        var loadSaleZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([saleZoneSelectorReadyDeferred.promise, dealDefinitionSelectionChangedPromiseDeferred.promise]).then(function () {
                            dealDefinitionSelectionChangedPromiseDeferred = undefined;

                            var saleZoneSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                                        SupplierDealIds: dealDefinitionSelectorAPI.getSelectedIds()
                                    }]
                                },
                                sellingNumberPlanId: sellingNumberPlanId,
                                selectedIds: saleZoneIds,
                                showSellingNumberPlanIfMultiple: true
                            };
                            VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, loadSaleZoneSelectorPromiseDeferred);
                        });

                        return loadSaleZoneSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SaleZoneGroupMatchingSupplierDeals, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                        ZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                        SupplierDealIds: dealDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);