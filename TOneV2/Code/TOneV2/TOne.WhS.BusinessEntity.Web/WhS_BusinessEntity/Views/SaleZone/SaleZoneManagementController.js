(function (appControllers) {

    "use strict";

    saleZoneManagementController.$inject = ['$scope'];

    function saleZoneManagementController($scope) {
        var gridAPI;
        var sellingDirectiveApi;
        var countryDirectiveApi;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject()
                return gridAPI.loadGrid(filter);
            };
            $scope.onSellingNumberReady = function (api) {
                sellingDirectiveApi = api;
                api.load();
            }
            $scope.onCountryReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid(filter);
            }
          
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleZoneManagementController', saleZoneManagementController);
})(appControllers);