(function (appControllers) {

    "use strict";

    currencyManagementController.$inject = ['$scope', 'VRCommon_CurrencyService'];

    function currencyManagementController($scope, VRCommon_CurrencyService) {
        var gridAPI;

        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter)
            };
            $scope.addNewCurrency = addNewCurrency;
        }

        function load() {
        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
                Symbol: $scope.symbol,
            };
           
        }

        function addNewCurrency() {
            var onCurrencyAdded = function (currencyObj) {
                gridAPI.onCurrencyAdded(currencyObj);
            };

            VRCommon_CurrencyService.addCurrency(onCurrencyAdded);
        }

    }

    appControllers.controller('VRCommon_CurrencyManagementController', currencyManagementController);
})(appControllers);