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

            $scope.onSelectSellingNumberPlan = function (selectedItem) {
                $scope.showSaleZoneSelector = true;
                var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                var payload = {
                    filter: { SellingNumberPlanId: selectedItem.SellingNumberPlanId },
                }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            }

            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                
                console.log(payload);

                if (payload != undefined && payload.filter.sellingNumberPlanId == undefined)
                {
                    $scope.showSellingNumberPlan = true;

                    var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

                    sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                        var sellingNumberPlanPayload;

                        if (payload != undefined)
                        {
                            sellingNumberPlanPayload = {
                                selectedIds: payload.SellingNumberPlanId
                            };
                        }
                            
                        VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                    });

                    promises.push(loadSellingNumberPlanPromiseDeferred.promise);

                    var subPromisesArray = [];

                    subPromisesArray.push(loadSellingNumberPlanPromiseDeferred.promise);
                    subPromisesArray.push(saleZoneReadyPromiseDeferred.promise);

                    UtilsService.waitMultiplePromises(subPromisesArray).then(loadSaleZoneSelector);
                }
                else
                {
                    saleZoneReadyPromiseDeferred.promise.then(loadSaleZoneSelector);
                }

                var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSaleZonePromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);

                function loadSaleZoneSelector() {
                    var saleZonePayload;

                    if (payload != undefined) {
                        sellingNumberPlanParameter = payload.filter.sellingNumberPlanId;
                        $scope.showSaleZoneSelector = true;

                        saleZonePayload = {
                            filter: { SellingNumberPlanId: sellingNumberPlanParameter != undefined ? sellingNumberPlanParameter : payload.SellingNumberPlanId },
                            selectedIds: payload != undefined ? payload.ZoneIds : undefined
                        };
                    }
                    
                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, saleZonePayload, loadSaleZonePromiseDeferred);
                }
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