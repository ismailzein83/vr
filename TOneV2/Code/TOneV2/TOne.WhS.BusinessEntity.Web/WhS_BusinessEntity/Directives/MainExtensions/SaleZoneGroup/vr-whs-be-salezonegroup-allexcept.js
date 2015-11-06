'use strict';
app.directive('vrWhsBeSalezonegroupAllexcept', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                sellingnumberplanid: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $scope.showSellingNumberPlan = false;

                var ctor = new allExceptCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SelectiveSaleZonesDirectiveTemplate.html';
            }

        };

        function allExceptCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService) {

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneDirectiveAPI;
            var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanParameter;

            function initializeController() {

                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                }

                $scope.onSaleZoneDirectiveReady = function (api) {
                    saleZoneDirectiveAPI = api;
                    saleZoneReadyPromiseDeferred.resolve();
                }

                $scope.onSellingNumberPlanSelectItem = function (selectedItem) {
                    if (selectedItem != undefined) {
                        var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                        var payload = {
                            filter: { SellingNumberPlanId: selectedItem.SellingNumberPlanId },
                        }

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var saleZoneInfoFilter;
                    var saleZoneGroupSettings;

                    if (payload != undefined) {
                        saleZoneInfoFilter = payload.saleZoneInfoFilter;
                        saleZoneGroupSettings = payload.saleZoneGroupSettings;
                    }

                    if (saleZoneInfoFilter == undefined || saleZoneInfoFilter.SaleZoneFilterSettings == undefined) {
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
                        var saleZonePayload;

                        sellingNumberPlanParameter = saleZoneInfoFilter != undefined ? saleZoneInfoFilter.SellingNumberPlanId : undefined;

                        saleZonePayload = {
                            filter: {
                                SellingNumberPlanId: sellingNumberPlanParameter,
                                SaleZoneFilterSettings: saleZoneInfoFilter != undefined ? saleZoneInfoFilter.SaleZoneFilterSettings : undefined
                            },
                            selectedIds: saleZoneGroupSettings != undefined ? saleZoneGroupSettings.ZoneIds : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, saleZonePayload, loadSaleZonePromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.AllExceptSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanId: sellingNumberPlanParameter != undefined ? sellingNumberPlanParameter : sellingNumberPlanDirectiveAPI.getSelectedIds(),
                        ZoneIds: saleZoneDirectiveAPI.getSelectedIds()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);