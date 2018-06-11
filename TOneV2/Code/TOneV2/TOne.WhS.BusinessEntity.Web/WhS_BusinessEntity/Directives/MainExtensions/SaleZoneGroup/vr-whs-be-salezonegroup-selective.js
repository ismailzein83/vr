'use strict';

app.directive('vrWhsBeSalezonegroupSelective', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                sellingnumberplanid: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SelectiveSaleZonesDirectiveTemplate.html';
            }
        };

        function selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
            this.initializeController = initializeController;

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneDirectiveAPI;
            var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanParameter;

            function initializeController() {
                $scope.showSellingNumberPlan = false;

                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                };

                $scope.onSaleZoneDirectiveReady = function (api) {
                    saleZoneDirectiveAPI = api;
                    saleZoneReadyPromiseDeferred.resolve();
                };

                $scope.onSellingNumberPlanSelectItem = function (selectedItem) {
                    if (selectedItem != undefined) {

                        sellingNumberPlanParameter = selectedItem;

                        var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value; };

                        var payload = {
                            sellingNumberPlanId: selectedItem.SellingNumberPlanId
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var sellingNumberPlanId;
                    var saleZoneGroupSettings;
                    var saleZoneFilterSettings;

                    if (payload != undefined) {
                        sellingNumberPlanId = payload.sellingNumberPlanId;
                        saleZoneGroupSettings = payload.saleZoneGroupSettings;
                        saleZoneFilterSettings = payload.saleZoneFilterSettings;
                    }

                    if (sellingNumberPlanId == undefined || (saleZoneFilterSettings != undefined && saleZoneFilterSettings.RoutingProductId == undefined)) {
                        $scope.showSellingNumberPlan = true;
                        var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadSellingNumberPlanPromiseDeferred.promise);

                        sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                            var sellingNumberPlanPayload;

                            if (saleZoneGroupSettings != undefined) {
                                sellingNumberPlanPayload = {
                                    selectedIds: saleZoneGroupSettings.SellingNumberPlanId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                        });
                    }

                    var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadSaleZonePromiseDeferred.promise);

                    saleZoneReadyPromiseDeferred.promise.then(function () {
                        sellingNumberPlanParameter = sellingNumberPlanId != undefined ? sellingNumberPlanId : saleZoneGroupSettings != undefined ? saleZoneGroupSettings.SellingNumberPlanId : undefined;

                        var saleZonePayload = {
                            filter: { SaleZoneFilterSettings: saleZoneFilterSettings != undefined ? saleZoneFilterSettings : undefined },
                            sellingNumberPlanId: sellingNumberPlanParameter,
                            selectedIds: saleZoneGroupSettings != undefined ? saleZoneGroupSettings.ZoneIds : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, saleZonePayload, loadSaleZonePromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanId: sellingNumberPlanParameter != undefined && sellingNumberPlanParameter > 0 ? sellingNumberPlanParameter : sellingNumberPlanDirectiveAPI.getSelectedIds(),
                        ZoneIds: saleZoneDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);