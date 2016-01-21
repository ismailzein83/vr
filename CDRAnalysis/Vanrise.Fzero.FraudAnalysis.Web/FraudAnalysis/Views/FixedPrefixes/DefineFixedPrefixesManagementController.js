(function (appControllers) {

    "use strict";

    defineFixedPrefixesManagementController.$inject = ['$scope', 'Fzero_FraudAnalysis_MainService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','Fzero_FraudAnalysis_DefineFixedPrefixesAPIService'];

    function defineFixedPrefixesManagementController($scope, Fzero_FraudAnalysis_MainService, UtilsService, VRNotificationService, VRUIUtilsService, Fzero_FraudAnalysis_DefineFixedPrefixesAPIService) {
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
            $scope.AddNewFixedPrefix = AddNewFixedPrefix;
        }

        function load() {
          
        }

        function getFilterObject() {
            var data = {
                Name: $scope.name
            };
            return data;
        }
       

        function AddNewFixedPrefix() {
            var onFixedPrefixAdded = function (fixedPrefixObj) {
                if (gridAPI != undefined)
                    gridAPI.onFixedPrefixAdded(fixedPrefixObj);
            };

            Fzero_FraudAnalysis_MainService.addNewFixedPrefix(onFixedPrefixAdded);
        }
    }

    appControllers.controller('Fzero_FraudAnalysis_DefineFixedPrefixesManagementController', defineFixedPrefixesManagementController);
})(appControllers);