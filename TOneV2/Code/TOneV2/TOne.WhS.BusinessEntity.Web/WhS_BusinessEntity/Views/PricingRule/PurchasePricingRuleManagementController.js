(function (appControllers) {

    "use strict";

    purchasePricingRuleManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'WhS_Be_PricingRuleTypeEnum'];

    function purchasePricingRuleManagementController($scope, WhS_BE_MainService, UtilsService, WhS_Be_PricingRuleTypeEnum) {
        var gridAPI;
        defineScope();

        function defineScope() {
            $scope.selectedPricingRuleTypes = [];
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addMenuActions = [{
                name: "TOD Rule",
                clicked: function () {
                    return AddNewPurchasePricingRule(WhS_Be_PricingRuleTypeEnum.TOD.value);
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
            definePricingRuleTypes();
        }
        function getFilterObject() {
            var data = {
                RuleTypes: UtilsService.getPropValuesFromArray($scope.selectedPricingRuleTypes, "value"),
                Description: $scope.description
            };
            return data;
        }

        function AddNewPurchasePricingRule(value) {
            var onPricingRuleAdded = function (purchasePricingRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onPricingRuleAdded(purchasePricingRuleObj);
            };

            WhS_BE_MainService.addPurchasePricingRule(onPricingRuleAdded, value);
        }
        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
    }

    appControllers.controller('WhS_BE_PurchasePricingRuleManagementController', purchasePricingRuleManagementController);
})(appControllers);