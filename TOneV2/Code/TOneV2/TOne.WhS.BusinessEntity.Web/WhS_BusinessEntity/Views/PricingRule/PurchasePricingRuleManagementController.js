(function (appControllers) {

    "use strict";

    purchasePricingRuleManagementController.$inject = ['$scope', 'UtilsService', 'WhS_Be_PricingRuleTypeEnum', 'VRUIUtilsService', 'WhS_BE_PurchasePricingRuleService'];

    function purchasePricingRuleManagementController($scope, UtilsService, WhS_Be_PricingRuleTypeEnum, VRUIUtilsService, WhS_BE_PurchasePricingRuleService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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


            $scope.addMenuActions = [{
                name: "Rate Type Rule",
                clicked: function () {
                    return AddNewPurchasePricingRule(WhS_Be_PricingRuleTypeEnum.RateType.value);
                }
            },
            {
                name: "Tariff Rule",
                clicked: function () {
                    return AddNewPurchasePricingRule(WhS_Be_PricingRuleTypeEnum.Tariff.value);
                }
            },
             {
                 name: "Extra Charge",
                 clicked: function () {
                     return AddNewPurchasePricingRule(WhS_Be_PricingRuleTypeEnum.ExtraCharge.value);
                 }
             }];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.AddNewPurchasePricingRule = AddNewPurchasePricingRule;
        }

        function load() {
            $scope.isLoadingFilterData = true;
            return UtilsService.waitMultipleAsyncOperations([definePricingRuleTypes, loadSuppliersSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }
        function getFilterObject() {
            var data = {
                RuleTypes: UtilsService.getPropValuesFromArray($scope.selectedPricingRuleTypes, "value"),
                Description: $scope.description,
                SupplierIds: carrierAccountDirectiveAPI.getSelectedIds(),
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }
        function loadSuppliersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }

        function AddNewPurchasePricingRule(value) {
            var onPricingRuleAdded = function (purchasePricingRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onPricingRuleAdded(purchasePricingRuleObj);
            };

            WhS_BE_PurchasePricingRuleService.addPurchasePricingRule(onPricingRuleAdded, value);
        }
        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
    }

    appControllers.controller('WhS_BE_PurchasePricingRuleManagementController', purchasePricingRuleManagementController);
})(appControllers);