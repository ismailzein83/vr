(function (appControllers) {

    "use strict";

    salePricingRuleManagementController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService','WhS_Be_PricingRuleTypeEnum'];

    function salePricingRuleManagementController($scope, WhS_BE_SalePricingRuleAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService, WhS_Be_PricingRuleTypeEnum) {
        var gridAPI;
        defineScope();

        function defineScope() {
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
            //$scope.isGettingData = true;
            //if (carrierAccountDirectiveAPI == undefined || pricingProductsDirectiveAPI == undefined)
            //    return;

            //UtilsService.waitMultipleAsyncOperations([loadPricingProducts, loadCarrierAccounts]).then(function () {
            //}).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //    $scope.isGettingData = false;
            //}).finally(function () {
            //    $scope.isGettingData = false;
            //});
        }
        function loadPricingProducts() {
            return pricingProductsDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
            return carrierAccountDirectiveAPI.load();
        }

        function getFilterObject() {
            var data = {
            };
            return data;
        }

        function AddNewSalePricingRule(value) {
            var onSalePricingRuleAdded = function (salePricingRuleObj) {
                if ( gridAPI != undefined)
                    gridAPI.onSalePricingRuleAdded(salePricingRuleObj);
            };

            WhS_BE_MainService.addSalePricingRule(onSalePricingRuleAdded,value);
        }
    }

    appControllers.controller('WhS_BE_SalePricingRuleManagementController', salePricingRuleManagementController);
})(appControllers);