(function (appControllers) {

    "use strict";

    setSupplierRuleManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService'];

    function setSupplierRuleManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService) {
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
            $scope.AddNewSupplierRule = AddNewSupplierRule;
        }

        function load() {
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
            };
            return data;
        }
     
        function AddNewSupplierRule() {
            var onSetSupplierRuleAdded = function (supplierRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onSetSupplierRuleAdded(supplierRuleObj);
            };

            WhS_CDRProcessing_MainService.addSetSupplierRule(onSetSupplierRuleAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_SetSupplierRuleManagementController', setSupplierRuleManagementController);
})(appControllers);