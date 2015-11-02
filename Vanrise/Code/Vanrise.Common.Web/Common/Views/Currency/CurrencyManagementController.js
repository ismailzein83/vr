(function (appControllers) {

    "use strict";

    currencyManagementController.$inject = ['$scope', 'VRCommon_CurrencyService'];

    function currencyManagementController($scope, VrCommon_CurrencyService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
              
            };

            $scope.onGridReady = function (api) {
              
            }
           
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
            };
           
        }

        function addNewCurrency() {
           
        }

    }

    appControllers.controller('VRCommon_CurrencyManagementController', currencyManagementController);
})(appControllers);