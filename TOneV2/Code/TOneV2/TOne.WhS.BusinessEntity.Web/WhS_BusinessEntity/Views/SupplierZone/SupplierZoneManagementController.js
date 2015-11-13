(function (appControllers) {

    "use strict";

    supplierZoneManagementController.$inject = ['$scope'];

    function supplierZoneManagementController($scope) {
        var gridAPI;
        var supplierDirectiveApi;
        var countryDirectiveApi;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                api.load({});
            }
            $scope.onCountryReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
            }
          
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
                SupplierId: supplierDirectiveApi.getSelectedIds(),
                EffectiveOn: $scope.effectiveOn,
                Countries: countryDirectiveApi.getIdsData().length > 0 ? countryDirectiveApi.getIdsData() : null
            };
           
        }

    }

    appControllers.controller('WhS_BE_SupplierZoneManagementController', supplierZoneManagementController);
})(appControllers);