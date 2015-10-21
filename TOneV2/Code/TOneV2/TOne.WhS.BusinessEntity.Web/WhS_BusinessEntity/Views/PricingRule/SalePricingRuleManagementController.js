(function (appControllers) {

    "use strict";

    salePricingRuleManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService','WhS_Be_PricingRuleTypeEnum'];

    function salePricingRuleManagementController($scope, WhS_BE_MainService, UtilsService, WhS_Be_PricingRuleTypeEnum) {
        var gridAPI;
        defineScope();
        load();
        function defineScope() {
            $scope.selectedPricingRuleTypes = [];
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addMenuActions = [{
                name: "TOD Rule",
                clicked:function () {
                    return AddNewSalePricingRule(WhS_Be_PricingRuleTypeEnum.TOD.value);
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
            definePricingRuleTypes();
        }
        function loadPricingProducts() {
            return pricingProductsDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
            return carrierAccountDirectiveAPI.load();
        }

        function getFilterObject() {
            var data = {
                RuleTypes: UtilsService.getPropValuesFromArray($scope.selectedPricingRuleTypes, "value"),
                Description:$scope.description
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

            WhS_BE_MainService.addSalePricingRule(onPricingRuleAdded, value);
        }
    }

    appControllers.controller('WhS_BE_SalePricingRuleManagementController', salePricingRuleManagementController);
})(appControllers);