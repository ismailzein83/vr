(function (appControllers) {

    "use strict";

    pricingRuleManagementController.$inject = ['$scope', 'WhS_BE_PricingRuleAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function pricingRuleManagementController($scope, WhS_BE_PricingRuleAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
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
            $scope.AddNewPricingRule = AddNewPricingRule;
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

        function AddNewPricingRule() {
            var onPricingRuleAdded = function (pricingRuleObj) {
                if ( gridAPI != undefined)
                  gridAPI.onPricingRuleAdded(pricingRuleObj);
            };

            WhS_BE_MainService.addPricingRule(onPricingRuleAdded);
        }
    }

    appControllers.controller('WhS_BE_PricingRuleManagementController', pricingRuleManagementController);
})(appControllers);