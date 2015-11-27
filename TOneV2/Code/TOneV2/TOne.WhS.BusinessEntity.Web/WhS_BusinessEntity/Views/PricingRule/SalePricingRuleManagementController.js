(function (appControllers) {

    "use strict";

    salePricingRuleManagementController.$inject = ['$scope', 'UtilsService', 'WhS_Be_PricingRuleTypeEnum', 'VRUIUtilsService', 'WhS_BE_SalePricingRuleService'];

    function salePricingRuleManagementController($scope, UtilsService, WhS_Be_PricingRuleTypeEnum, VRUIUtilsService, WhS_BE_SalePricingRuleService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();
        function defineScope() {
            $scope.selectedPricingRuleTypes = [];
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

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

                var payload = {
                    sellingNumberPlanId: selectedItem.SellingNumberPlanId,
                }

                var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            }



            $scope.addMenuActions = [{
                name: "Rate Type Rule",
                clicked:function () {
                    return AddNewSalePricingRule(WhS_Be_PricingRuleTypeEnum.RateType.value);
                } 
            },
            {
                name: "Tariff Rule",
                clicked: function () {
                    return AddNewSalePricingRule(WhS_Be_PricingRuleTypeEnum.Tariff.value);
                }
            },
             {
                 name: "Extra Charge",
                 clicked: function () {
                     return AddNewSalePricingRule(WhS_Be_PricingRuleTypeEnum.ExtraCharge.value);
                 }
             }];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.AddNewSalePricingRule = AddNewSalePricingRule;
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([definePricingRuleTypes,loadCustomersSection, loadSellingNumberPlanSection, loadSaleZoneSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }
        function loadCustomersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }

        function loadSellingNumberPlanSection() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }

        function loadSaleZoneSection() {
            return saleZoneReadyPromiseDeferred.promise;
        }

        function getFilterObject() {
            var data = {
                RuleTypes: UtilsService.getPropValuesFromArray($scope.selectedPricingRuleTypes, "value"),
                Description: $scope.description,
                CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }
        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
        function AddNewSalePricingRule(value) {
            var onPricingRuleAdded = function (salePricingRuleObj) {
                    gridAPI.onPricingRuleAdded(salePricingRuleObj);
            };

            WhS_BE_SalePricingRuleService.addSalePricingRule(onPricingRuleAdded, value);
        }
    }

    appControllers.controller('WhS_BE_SalePricingRuleManagementController', salePricingRuleManagementController);
})(appControllers);