(function (appControllers) {

    "use strict";

    setCustomerRuleManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService'];

    function setCustomerRuleManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.AddNewCustomerRule = AddNewCustomerRule;
        }

        function load() {
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
            };
            return data;
        }

        function AddNewCustomerRule() {
            var onSetCustomerRuleAdded = function (customerRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onSetCustomerRuleAdded(customerRuleObj);
            };

            WhS_CDRProcessing_MainService.addSetCustomerRule(onSetCustomerRuleAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_SetCustomerRuleManagementController', setCustomerRuleManagementController);
})(appControllers);