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

            $scope.showSellingNumberPlan = false;

            var ctor = new selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService);
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

    function selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
        
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

            $scope.onSellingNumberPlanSelectionChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                if (selectedSellingNumberPlanId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                    var payload = {
                        filter: { SellingNumberPlanId: selectedSellingNumberPlanId },
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                }
            }

            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                $scope.showSellingNumberPlan = payload.filter.SaleZoneFilterSettings.RoutingProductId == null;

                var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

                sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                    var sellingNumberPlanPayload;

                    if (payload.filter.SellingNumberPlanId != undefined) {
                        sellingNumberPlanPayload = {
                            selectedIds: payload.filter.SellingNumberPlanId
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                });

                promises.push(loadSellingNumberPlanPromiseDeferred.promise);

                var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSaleZonePromiseDeferred.promise);

                saleZoneReadyPromiseDeferred.promise.then(function () {
                    var saleZonePayload;

                    if (payload.filter.SellingNumberPlanId != undefined) {

                        sellingNumberPlanParameter = payload.filter.SellingNumberPlanId;

                        saleZonePayload = {
                            filter: payload.filter,
                            selectedIds: (payload.saleZoneGroupSettings != undefined)? payload.saleZoneGroupSettings.ZoneIds: undefined
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, saleZonePayload, loadSaleZonePromiseDeferred);
                });

                return UtilsService.waitMultiplePromises(promises);
            }

            api.getData = function () {
                return {
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
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