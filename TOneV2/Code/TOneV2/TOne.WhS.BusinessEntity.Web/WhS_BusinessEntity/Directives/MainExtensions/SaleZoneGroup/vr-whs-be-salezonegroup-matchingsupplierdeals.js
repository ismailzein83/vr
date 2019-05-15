'use strict';

app.directive('vrWhsBeSalezonegroupMatchingsupplierdeals', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_CodeZoneMatchAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_CodeZoneMatchAPIService) {

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
            var selectedSaleZoneIds;
            var selectedSupplierDealIds;

            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dealDefinitionSelectionChangedPromiseDeferred;
            var onDeselectDealDefinitionLoadDeferred;

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

                    var _promises = [saleZoneSelectorReadyDeferred.promise];

                    if (onDeselectDealDefinitionLoadDeferred != undefined)
                        _promises.push(onDeselectDealDefinitionLoadDeferred.promise);

                    $scope.scopeModel.showSaleZoneSelector = true;

                    UtilsService.waitMultiplePromises(_promises).then(function () {
                        onDeselectDealDefinitionLoadDeferred = undefined;

                        var saleZoneSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                                    SupplierDealIds: dealDefinitionSelectorAPI.getSelectedIds()
                                }]
                            },
                            showSellingNumberPlanIfMultiple: true,
                            sellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                            selectedIds: selectedSaleZoneIds
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isSaleZoneSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorAPI, saleZoneSelectorPayload, setLoader, dealDefinitionSelectionChangedPromiseDeferred);
                    });
                };

                $scope.scopeModel.onDeselectDealDefinition = function (deSelectedSupplierDeal) {

                    var selectedSupplierDealIds = dealDefinitionSelectorAPI.getSelectedIds();

                    if (selectedSupplierDealIds != undefined && selectedSupplierDealIds.length == 1 && saleZoneSelectorAPI != undefined) {
                        saleZoneSelectorAPI.clearSelectedSaleZones();
                        selectedSaleZoneIds = undefined;
                        return;
                    }

                    var index = selectedSupplierDealIds.indexOf(deSelectedSupplierDeal.DealId);
                    if (index != -1) {
                        selectedSupplierDealIds.splice(index, 1);
                    }

                    var obj = {
                        SelectedSupplierDealIds: selectedSupplierDealIds,
                        SelectedSaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                        SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId()
                    };

                    onDeselectDealDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                    return WhS_Routing_CodeZoneMatchAPIService.GetSaleZonesMatchingSpecificDeals(obj).then(function (response) {
                        selectedSaleZoneIds = response;
                        onDeselectDealDefinitionLoadDeferred.resolve();
                    });
                };

                $scope.scopeModel.onSaleZoneSelectionChanged = function (selectedZones) {
                    if (selectedZones == undefined)
                        return;

                    selectedSaleZoneIds = [];
                    for (var i = 0; i < selectedZones.length; i++) {
                        selectedSaleZoneIds.push(selectedZones[i].SaleZoneId);
                    }
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
                        if (saleZoneGroupSettings != undefined) {
                            selectedSupplierDealIds = saleZoneGroupSettings.SupplierDealIds;
                            selectedSaleZoneIds = saleZoneGroupSettings.ZoneIds;
                        }
                    }

                    var promises = [];

                    var loadDealDefinitionSelectorPromise = loadDealDefinitionSelector(selectedSupplierDealIds);
                    promises.push(loadDealDefinitionSelectorPromise);

                    if (saleZoneGroupSettings != undefined) {
                        dealDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                        var loadSaleZoneSelectorPromise = loadSaleZoneSelector(sellingNumberPlanId, selectedSaleZoneIds);
                        promises.push(loadSaleZoneSelectorPromise);
                    }

                    function loadSaleZoneSelector(sellingNumberPlanId, saleZoneIds) {
                        var loadSaleZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        var _promises = [saleZoneSelectorReadyDeferred.promise];

                        if (dealDefinitionSelectionChangedPromiseDeferred != undefined)
                            _promises.push(dealDefinitionSelectionChangedPromiseDeferred.promise);

                        UtilsService.waitMultiplePromises(_promises).then(function () {
                            dealDefinitionSelectionChangedPromiseDeferred = undefined;
                            var saleZoneSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.Routing.MainExtensions.SaleZoneMatchingSupplierDealFilter,TOne.WhS.Routing.MainExtensions",
                                        SupplierDealIds: selectedSupplierDealIds != undefined ? selectedSupplierDealIds : dealDefinitionSelectorAPI.getSelectedIds()
                                    }]
                                },
                                showSellingNumberPlanIfMultiple: true,
                                sellingNumberPlanId: sellingNumberPlanId,
                                selectedIds: saleZoneIds
                            };
                            VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, loadSaleZoneSelectorPromiseDeferred);
                        });

                        return loadSaleZoneSelectorPromiseDeferred.promise;
                    }

                    function loadDealDefinitionSelector(selectedIds) {
                        var loadDealDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dealDefinitionSelectorReadyDeferred.promise.then(function () {

                            var dealDefinitionPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "TOne.WhS.Deal.MainExtensions.DealDefinitionFilter.CostDealFilter,TOne.WhS.Deal.MainExtensions"
                                    }],
                                    DealStatuses: ["Active", "InActive"]
                                },
                                selectedIds: selectedIds
                            };
                            VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionPayload, loadDealDefinitionSelectorPromiseDeferred);
                        });

                        return loadDealDefinitionSelectorPromiseDeferred.promise;
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