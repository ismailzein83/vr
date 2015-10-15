(function (appControllers) {

    "use strict";

    salePricingRuleManagementController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function salePricingRuleManagementController($scope, WhS_BE_SalePricingRuleAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        defineScope();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

         
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

        function AddNewSalePricingRule() {
            var onSalePricingRuleAdded = function (salePricingRuleObj) {
                if ( gridAPI != undefined)
                  gridAPI.onSalePricingRuleAdded(salePricingRuleObj);
            };

            WhS_BE_MainService.addSalePricingRule(onSalePricingRuleAdded);
        }
    }

    appControllers.controller('WhS_BE_SalePricingRuleManagementController', salePricingRuleManagementController);
})(appControllers);